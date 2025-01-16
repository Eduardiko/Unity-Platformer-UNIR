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

    private Vector3 initPosition;
    private float timeToShoot = 0;
    private int phase = 1;
    private float movementSpeed = 30f;
    private float maximumXSpeed = 35f;
    private float maximumYSpeed = 15f;
    [HideInInspector] public int health = 0;

    private Vector2 lastInputDirection = Vector2.zero;

    float dashForce = 15f;
    float dashOverflowDuration = 0.5f;
    float dashStartTime = 0f;

    float jumpOverflowDuration = 1.5f;
    float jumpStartTime = 0f;

    float easeOutSpeedTime = 1.5f;
    float elapsedEaseOutSpeedTime = 0;

    private bool isGrounded = false;
    private float groundCheckDistance = 1f;
    public LayerMask groundLayer;

    private int maxAirJumps = 0;
    private int availableAirJumps = 0;

    public int AvailableAirJumps { get => availableAirJumps; set => availableAirJumps = value; }
    public int MaxAirJumps { get => maxAirJumps; set => maxAirJumps = value; }

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerRenderer = GetComponent<SpriteRenderer>();

        moveAction = playerInput.actions.FindAction("Move");
        shootAction = playerInput.actions.FindAction("Shoot");

        initPosition = transform.position;
    }

    private void Update()
    {
        PerformGroundCheck();

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

        if (lastInputDirection != direction && direction.magnitude != 0)
            playerRigidBody.velocity = new Vector2(0f, playerRigidBody.velocity.y);

        lastInputDirection = direction;

        if (Mathf.Abs(playerRigidBody.velocity.x) < maximumXSpeed)
            playerRigidBody.AddForce(new Vector2(direction.x * movementSpeed, 0), ForceMode2D.Force);

    }

    void PerformGroundCheck()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
        print(isGrounded);
        Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
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

    private void ApplyDamage(int ammount)
    {
        health-= ammount;

        if (health <= 0)
        {
            //AudioManager.Instance.PlaySFX();
            Die();
        }

        StartCoroutine(DisableColliderAndBlink(2f, 0.1f));
    }

    public virtual void Die()
    {
        if (gameObject == null)
            return;

        Destroy(gameObject);
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
            ApplyDamage(0);
    }

    public IEnumerator DisableColliderAndBlink(float duration, float blinkInterval)
    {
        if (playerCollider != null && playerRenderer != null)
        {
            transform.position = initPosition;

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
        if (playerRigidBody.velocity.y < 0f)
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 0f);

        jumpStartTime = Time.time;

        if (isGrounded)
        {
            availableAirJumps = maxAirJumps;
            playerRigidBody.AddForce(new Vector2(0, 15f + playerRigidBody.velocity.x / 2), ForceMode2D.Impulse);
        }
        else
        {
            availableAirJumps--;
            playerRigidBody.AddForce(new Vector2(0, 15f), ForceMode2D.Impulse);
        }
    }

    public void ActionJump(InputAction.CallbackContext context)
    {
        if (context.performed && (availableAirJumps > 0 || isGrounded))
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

}
