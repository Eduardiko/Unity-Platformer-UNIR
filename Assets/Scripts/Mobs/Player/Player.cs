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
    [SerializeField] private float timeBetweenBullets = 2f;

    protected Collider2D playerCollider;
    protected SpriteRenderer playerRenderer;
    protected Rigidbody2D playerRigidBody;

    // Continous Input Actions
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction shootAction;

    private Vector2 spawnPosition;
    private float timeToShoot = 0;
    private int phase = 1;
    private float movementSpeed = 30f;
    private float maximumXSpeed = 35f;
    private float maximumYSpeed = 15f;
    private float shadowXSpeed = 0f;
    [HideInInspector] public int health = 3;

    private Vector2 lastInputDirection = Vector2.zero;

    float dashForce = 15f;
    float dashOverflowDuration = 0.5f;
    float dashStartTime = 0f;

    float jumpOverflowDuration = 1.5f;
    float jumpStartTime = 0f;

    float easeOutSpeedTime = 1.5f;
    float elapsedEaseOutSpeedTime = 0;

    private bool isGrounded = false;
    private Vector2 touchingWallFrom = Vector2.zero;
    private float groundCheckDistance = 1f;
    public LayerMask groundLayer;

    private int maxAirJumps = 0;
    private int availableAirJumps = 0;

    private bool canWallJump = false;

    public float searchInteractableRadius = 5f;
    public LayerMask interactableLayer;
    Interactable nearestInteractable = null;
    
    public float searchEnemyRadius = 10f;
    public LayerMask enemyLayer;
    Enemy nearestEnemy = null;

    private Coroutine coyoteCoroutine = null;

    public int AvailableAirJumps { get => availableAirJumps; set => availableAirJumps = value; }
    public int MaxAirJumps { get => maxAirJumps; set => maxAirJumps = value; }
    public Vector2 SpawnPosition { get => spawnPosition; set => spawnPosition = value; }

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerRenderer = GetComponent<SpriteRenderer>();

        moveAction = playerInput.actions.FindAction("Move");
        shootAction = playerInput.actions.FindAction("Shoot");

        spawnPosition = transform.position;

        StartCoroutine(UpdateShadowXSpeed());
    }

    private void Update()
    {
        PerformRayCastChecks();
        SetNearestInteractable();
        SetNearestEnemy();

        if (timeToShoot > 0)
            timeToShoot -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        ApplyVelocityCapWithDecay();
        ReadContinuousInputs();
    }

    public void ReadContinuousInputs()
    {
        Vector2 direction = moveAction.ReadValue<Vector2>();

        if (lastInputDirection != direction && direction.magnitude != 0 && touchingWallFrom.magnitude == 0)
            playerRigidBody.velocity = new Vector2(0f, playerRigidBody.velocity.y);

        lastInputDirection = direction;

        if (Mathf.Abs(playerRigidBody.velocity.x) < maximumXSpeed)
            playerRigidBody.AddForce(new Vector2(direction.x * movementSpeed, 0), ForceMode2D.Force);

    }

    void PerformRayCastChecks()
    {
        bool rayCastGrounded = Physics2D.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        print(isGrounded);

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

    void ApplyVelocityCapWithDecay()
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

    public void ApplyDamage(int ammount)
    {
        health-= ammount;

        if (health <= 0)
        {
            //AudioManager.Instance.PlaySFX();
            Die();
        }
    }

    public void Die()
    {
        if (gameObject == null)
            return;

        gameObject.transform.position = spawnPosition;
    }

    public void Heal(int ammount)
    {
        health+=ammount;
        //AudioManager.Instance.PlaySFX(3, 0.3f);
    }

    public void UpgradePlayer(PlayerUpgrades upgradeWith)
    {

        switch (upgradeWith)
        {
            case PlayerUpgrades.ADD_JUMP:
                maxAirJumps++;
                break;
            case PlayerUpgrades.DASH:
                break;
            default:
                break;
        }

        //AudioManager.Instance.PlaySFX(2, 0.5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
            ApplyDamage(1);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
            canWallJump = true;
    }

    public IEnumerator DisableColliderAndBlink(float duration, float blinkInterval)
    {
        if (playerCollider != null && playerRenderer != null)
        {
            transform.position = spawnPosition;

            playerCollider.enabled = false;

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                playerRenderer.enabled = !playerRenderer.enabled;

                yield return new WaitForSeconds(blinkInterval);

                elapsedTime += blinkInterval;
            }

            playerRenderer.enabled = true;
            playerCollider.enabled = true;
        }
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
        }
        else
        {
            if(canWallJump && touchingWallFrom.magnitude != 0)
            {
                // Reset Falling Speed
                if (playerRigidBody.velocity.y < 0f)
                    playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 0f);

                playerRigidBody.AddForce(new Vector2(15f * -touchingWallFrom.x, 25f), ForceMode2D.Impulse);
            }
            else if(availableAirJumps > 0)
            {
                // Reset Falling Speed
                if (playerRigidBody.velocity.y < 0f)
                    playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 0f);

                availableAirJumps--;
                playerRigidBody.AddForce(new Vector2(0, 15f), ForceMode2D.Impulse);
            }
        }
    }

    public void ActionJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            Jump();
    }

    private void Dash()
    {
        playerRigidBody.AddForce(new Vector2(dashForce * lastInputDirection.x, 0f), ForceMode2D.Impulse);
        dashStartTime = Time.time;
    }

    public void ActionDash(InputAction.CallbackContext context)
    {
        if (context.performed)
            Dash();
    }

    public void ActionInteract(InputAction.CallbackContext context)
    {
        if (context.performed && nearestInteractable != null)
            nearestInteractable.Interact();
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

    private void SetNearestInteractable()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, searchInteractableRadius, interactableLayer);

        if (colliders.Length == 0)
            nearestInteractable = null;
        
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D collider in colliders)
        {
            Interactable interactable = collider.GetComponent<Interactable>();

            float distance = (transform.position - collider.transform.position).magnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestInteractable = interactable;
            }
        }
    }

    private void SetNearestEnemy()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, searchEnemyRadius, enemyLayer);
        
        if (colliders.Length == 0)
            nearestEnemy = null;

        float closestDistance = Mathf.Infinity;

        foreach (Collider2D collider in colliders)
        {
            Enemy enemy = collider.GetComponent<Enemy>();

            float distance = (transform.position - collider.transform.position).magnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestEnemy = enemy;
            }

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
