using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.Experimental;

public enum PlayerUpgrades {
    DOUBLE_JUMP,
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
    private float maximumSpeed = 35f;
    [HideInInspector] public int health = 0;

    private Vector2 lastInputDirection = Vector2.zero;

    float easeOutSpeedTime = 1.5f;
    float elapsedEaseOutSpeedTime = 0;

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

        if (timeToShoot > 0)
            timeToShoot -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        ReadContinuousInputs();
    }

    public void ReadContinuousInputs()
    {
        Vector2 direction = moveAction.ReadValue<Vector2>();

        if (lastInputDirection != direction)
            playerRigidBody.velocity = playerRigidBody.velocity / 2;

        lastInputDirection = direction;

        playerRigidBody.AddForce(new Vector2(direction.x * movementSpeed, 0), ForceMode2D.Force);
        //playerRigidBody.velocity = new Vector2(direction.x * movementSpeed, playerRigidBody.velocity.y);

        if (playerRigidBody.velocity.x >= maximumSpeed)
            playerRigidBody.velocity = new Vector2(maximumSpeed, playerRigidBody.velocity.y);
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
            case PlayerUpgrades.DOUBLE_JUMP:
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

}
