using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Invincibility")]
    [SerializeField] private float invincibilityDuration = 1.5f;
    [SerializeField] private float blinkRate = 12f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCol;
    private Vector2 moveInput;
    private bool isInvincible;
    private float invincibilityTimer;

    private static readonly Collider2D[] _overlapBuffer = new Collider2D[8];

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCol = GetComponent<CircleCollider2D>();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsGameActive)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.MovePosition(rb.position + moveInput.normalized * moveSpeed * Time.fixedDeltaTime);

        // Manual overlap check for enemies — more reliable than trigger callbacks
        // between dynamic and kinematic bodies
        if (!isInvincible)
        {
            int count = Physics2D.OverlapCircleNonAlloc(rb.position, circleCol.radius * 0.9f, _overlapBuffer);
            for (int i = 0; i < count; i++)
            {
                if (_overlapBuffer[i] != null && _overlapBuffer[i].CompareTag("Enemy"))
                {
                    TakeEnemyDamage();
                    break;
                }
            }
        }
    }

    private void Update()
    {
        if (!isInvincible) return;
        invincibilityTimer -= Time.deltaTime;
        spriteRenderer.enabled = Mathf.Sin(invincibilityTimer * blinkRate) > 0f;
        if (invincibilityTimer <= 0f)
        {
            isInvincible = false;
            spriteRenderer.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coletavel"))
        {
            AudioManager.Instance?.PlayCollect();
            GameManager.Instance?.CollectCrystal();
            Destroy(other.gameObject);
        }
    }

    private void TakeEnemyDamage()
    {
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
        AudioManager.Instance?.PlayDamage();
        GameManager.Instance?.TakeDamage();
    }
}
