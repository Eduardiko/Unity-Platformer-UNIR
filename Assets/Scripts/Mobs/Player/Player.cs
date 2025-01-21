using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.Experimental;

public enum PlayerUpgrades {
    ADD_JUMP,
    DASH
}

public class Player : MonoBehaviour
{
    // Stats
    [SerializeField] private float movementSpeed = 30f;
    [SerializeField] private float maximumXSpeed = 35f;
    [SerializeField] private float maximumYSpeed = 15f;
    [SerializeField] private float shadowXSpeed = 0f;

    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashOverflowDuration = 0.5f;
    private float dashStartTime = 0f;

    [SerializeField] private float jumpOverflowDuration = 1.5f;
    private float jumpStartTime = 0f;

    [SerializeField] private float searchInteractableRadius = 5f;
    [SerializeField] private float searchEnemyRadius = 10f;
    [SerializeField] private float groundCheckDistance = 0.6f;

    private int maxAirJumps = 0;
    private int availableAirJumps = 0;


    // Continous Input Actions
    private Rigidbody2D playerRigidBody;
    private PlayerInput playerInput;
    private InputAction moveAction;


    //References
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private GameObject interactableCrossHair;
    [SerializeField] private GameObject enemyCrossHair;

    private bool isDashUnlocked = false;
    private bool isGrounded = false;
    private bool canWallJump = false;

    private Vector2 lastInputDirection = Vector2.zero; 
    private Vector2 touchingWallFrom = Vector2.zero;

    private Interactable nearestInteractable = null;
    private Enemy nearestEnemy = null;
    private Checkpoint lastCheckpoint = null;
    private Coroutine coyoteCoroutine = null;

    public int AvailableAirJumps { get => availableAirJumps; set => availableAirJumps = value; }
    public int MaxAirJumps { get => maxAirJumps; set => maxAirJumps = value; }
    public bool IsDashUnlocked { get => isDashUnlocked; set => isDashUnlocked = value; }
    public Rigidbody2D PlayerRigidBody { get => playerRigidBody; set => playerRigidBody = value; }
    public Checkpoint LastCheckpoint { get => lastCheckpoint; set => lastCheckpoint = value; }

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerRigidBody = GetComponent<Rigidbody2D>();

        moveAction = playerInput.actions.FindAction("Move");

        StartCoroutine(UpdateShadowXSpeed());

        interactableCrossHair.SetActive(false);
        enemyCrossHair.SetActive(false);
    }

    private void Update()
    {
        PerformRayCastChecks();
        SetNearestInteractable();
        SetNearestEnemy();
    }

    void FixedUpdate()
    {
        ApplyVelocityCapWithDecay();
        ReadContinuousInputs();
    }

    public void ReadContinuousInputs()
    {
        Vector2 direction = moveAction.ReadValue<Vector2>();

        if (lastInputDirection != direction && direction.magnitude != 0 && touchingWallFrom.magnitude == 0 && lastInputDirection.x * playerRigidBody.velocity.x >= 0)
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x/4, playerRigidBody.velocity.y);

        if(direction.magnitude > 0)
            lastInputDirection = direction;

        if (Mathf.Abs(playerRigidBody.velocity.x) < maximumXSpeed)
            playerRigidBody.AddForce(new Vector2(direction.x * movementSpeed, 0), ForceMode2D.Force);

    }

    public void UpgradePlayer(PlayerUpgrades upgradeWith)
    {

        switch (upgradeWith)
        {
            case PlayerUpgrades.ADD_JUMP:
                maxAirJumps++;
                availableAirJumps++;
                break;
            case PlayerUpgrades.DASH:
                break;
            default:
                break;
        }

        AudioManager.Instance.PlaySFX(1);

        //AudioManager.Instance.PlaySFX(2, 0.5f);
    }

    private void Jump()
    {
        jumpStartTime = Time.time;


        if (isGrounded)
        {
            if(playerRigidBody.velocity.x == 0f)
                playerRigidBody.AddForce(new Vector2(0, 15f + shadowXSpeed / 2), ForceMode2D.Impulse);
            else
                playerRigidBody.AddForce(new Vector2(0, 15f + Mathf.Abs(playerRigidBody.velocity.x) / 2), ForceMode2D.Impulse);
            
            AudioManager.Instance.PlaySFX(0);
        }
        else
        {
            if(canWallJump && touchingWallFrom.magnitude != 0)
            {
                //Wall Jump
                // Reset Falling Speed
                if (playerRigidBody.velocity.y < 0f)
                    playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 0f);

                playerRigidBody.AddForce(new Vector2(15f * -touchingWallFrom.x, 25f), ForceMode2D.Impulse);
                AudioManager.Instance.PlaySFX(0);

            }
            else if(availableAirJumps > 0)
            {
                //Air Jump
                // Reset Falling Speed
                if (playerRigidBody.velocity.y < 0f)
                    playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 0f);

                availableAirJumps--;
                playerRigidBody.AddForce(new Vector2(0, 15f), ForceMode2D.Impulse);
                AudioManager.Instance.PlaySFX(0);

            }
        }
    }

    private void Dash()
    {
        playerRigidBody.AddForce(new Vector2(dashForce * lastInputDirection.x, 0f), ForceMode2D.Impulse);
        dashStartTime = Time.time;
        AudioManager.Instance.PlaySFX(3);
    }

    public void Die()
    {
        if (gameObject == null)
            return;

        lastCheckpoint.ResetArea();
        playerRigidBody.velocity = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
            Die();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
            canWallJump = true;
    }

    private void PerformRayCastChecks()
    {
        bool rayCastGrounded = Physics2D.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        if (rayCastGrounded)
        {
            isGrounded = true;

            if (coyoteCoroutine != null)
            {
                StopCoroutine(coyoteCoroutine);
                coyoteCoroutine = null;
            }
        }
        else if (isGrounded)
        {
            if (coyoteCoroutine == null)
                coyoteCoroutine = StartCoroutine(CoyoteTime());

        } else if(coyoteCoroutine != null)
        {
            StopCoroutine(coyoteCoroutine);
            coyoteCoroutine = null;
        }

        Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);

        if(isGrounded)
            availableAirJumps = maxAirJumps;

        if (Physics2D.Raycast(transform.position, Vector3.right, groundCheckDistance, groundLayer))
        {
            touchingWallFrom = Vector2.right;
        } else if (Physics2D.Raycast(transform.position, Vector3.left, groundCheckDistance, groundLayer))
        {
            touchingWallFrom = Vector2.left;
        } else
        {
            canWallJump = false;
            touchingWallFrom = Vector2.zero;
        } 

        Debug.DrawRay(transform.position, Vector3.right * groundCheckDistance, Color.green);
        Debug.DrawRay(transform.position, Vector3.left * groundCheckDistance, Color.green);

    }

    private void SetNearestInteractable()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, searchInteractableRadius, interactableLayer);

        if (colliders.Length == 0)
        {
            nearestInteractable = null;
            interactableCrossHair.SetActive(false);
            return;
        }
        
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D collider in colliders)
        {
            if (collider.tag == "buttonDoor")
                continue;

            Interactable interactable = collider.GetComponent<Interactable>();

            float distance = (transform.position - collider.transform.position).magnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestInteractable = interactable;
            }
        }

        if(nearestInteractable != null)
        {
            interactableCrossHair.SetActive(true);
            interactableCrossHair.transform.position = nearestInteractable.transform.position;
        }
    }

    private void SetNearestEnemy()
    {
        if(nearestEnemy != null && nearestEnemy.Health > 0)
        {
            enemyCrossHair.SetActive(true);
            enemyCrossHair.transform.position = nearestEnemy.transform.position;
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, searchEnemyRadius, enemyLayer);
        
        if (colliders.Length == 0)
        {
            nearestEnemy = null;
            enemyCrossHair.SetActive(false);
            return;
        }

        float closestDistance = Mathf.Infinity;

        foreach (Collider2D collider in colliders)
        {
            Enemy enemy = collider.GetComponent<Enemy>();

            if (enemy == null)
                return;

            float distance = (transform.position - collider.transform.position).magnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestEnemy = enemy;
            }
        }
    }

    private void ApplyVelocityCapWithDecay()
    {
        if (Mathf.Abs(playerRigidBody.velocity.x) > maximumXSpeed)
        {
            float elapsedTime = Time.time - dashStartTime;

            if (elapsedTime < dashOverflowDuration)
            {
                float direction = Mathf.Sign(playerRigidBody.velocity.x);
                float newVelocityX = Mathf.Lerp(playerRigidBody.velocity.x, direction * maximumXSpeed, elapsedTime / dashOverflowDuration);
                playerRigidBody.velocity = new Vector2(newVelocityX, playerRigidBody.velocity.y);
            }
        }

        if (playerRigidBody.velocity.y > maximumYSpeed)
        {
            float elapsedTime = Time.time - jumpStartTime;

            if (elapsedTime < jumpOverflowDuration)
            {
                float direction = Mathf.Sign(playerRigidBody.velocity.y);
                float newVelocityY = Mathf.Lerp(playerRigidBody.velocity.y, direction * maximumYSpeed, elapsedTime / jumpOverflowDuration);
                playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, newVelocityY);
            }
        }

    }

    IEnumerator UpdateShadowXSpeed()
    {
        while (true)
        {
            // Store the current X velocity of the Rigidbody with a delay
            shadowXSpeed = playerRigidBody.velocity.x;

            // Wait for 0.3 seconds before updating again
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator CoyoteTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.15f);

            isGrounded = false;
        }
    }

    public void ActionJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            Jump();
    }

    public void ActionDash(InputAction.CallbackContext context)
    {
        if (context.performed && isDashUnlocked)
            Dash();
    }

    public void ActionInteract(InputAction.CallbackContext context)
    {
        if (context.performed && nearestInteractable != null)
        {
            nearestInteractable.Interact();
            AudioManager.Instance.PlaySFX(5);

        }
    }

    public void ActionAttack(InputAction.CallbackContext context)
    {
        if (context.performed && nearestEnemy != null)
        {
            // Reset Falling Speed
            if (playerRigidBody.velocity.y < 0f)
                playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 0f);

            playerRigidBody.AddForce(new Vector2(0f, 15f), ForceMode2D.Impulse);

            nearestEnemy.ApplyDamage();

            enemyCrossHair.SetActive(false);

            AudioManager.Instance.PlaySFX(4);
        }
    }

    public void ActionGoToTitleSceen(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SceneChanger[] sceneChangers = FindObjectsOfType<SceneChanger>();
            sceneChangers[0].ChangeScene("TitleScreen");
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the search radius in the editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, searchInteractableRadius);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, searchEnemyRadius);

    }

}
