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

    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireCooldown = 0.3f;
    [SerializeField] private Transform muzzle; // optional; defaults to transform

    [Header("Power-up Visuals")]
    [SerializeField] private Sprite shieldSprite;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCol;
    private Vector2 moveInput;
    private bool isInvincible;
    private float invincibilityTimer;

    // shooting
    private float fireTimer;
    private float fireRateMultiplier = 1f;
    private InputAction attackAction;

    // power-up state
    private float baseMoveSpeed;
    private bool hasShield;
    private GameObject shieldVisual;

    private static readonly Collider2D[] _overlapBuffer = new Collider2D[8];

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCol = GetComponent<CircleCollider2D>();
        baseMoveSpeed = moveSpeed;

        var playerInput = GetComponent<PlayerInput>();
        if (playerInput != null && playerInput.actions != null)
            attackAction = playerInput.actions.FindAction("Attack");
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
        // Shooting
        if (GameManager.Instance != null && GameManager.Instance.IsGameActive)
        {
            fireTimer -= Time.deltaTime;
            bool firing = attackAction != null && attackAction.IsPressed();
            if (firing && fireTimer <= 0f)
            {
                Fire();
                fireTimer = fireCooldown / fireRateMultiplier;
            }
        }

        // Invincibility blink
        if (!isInvincible) return;
        invincibilityTimer -= Time.deltaTime;
        spriteRenderer.enabled = Mathf.Sin(invincibilityTimer * blinkRate) > 0f;
        if (invincibilityTimer <= 0f)
        {
            isInvincible = false;
            spriteRenderer.enabled = true;
        }
    }

    private void Fire()
    {
        if (projectilePrefab == null) return;
        Vector3 spawnPos = muzzle != null ? muzzle.position : transform.position;
        var go = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        var proj = go.GetComponent<Projectile>();
        if (proj != null) proj.Launch(Vector2.up);
        AudioManager.Instance?.PlayShoot();
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

    /// <summary>Called by enemy projectiles when they hit the player.</summary>
    public void ApplyHit()
    {
        TakeEnemyDamage();
    }

    private void TakeEnemyDamage()
    {
        if (isInvincible) return;

        if (hasShield)
        {
            hasShield = false;
            ShowShield(false);
            isInvincible = true;
            invincibilityTimer = invincibilityDuration;
            AudioManager.Instance?.PlayShieldBreak();
            return;
        }

        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
        AudioManager.Instance?.PlayDamage();
        GameManager.Instance?.TakeDamage();
        ScreenShake.Shake(0.3f, 0.3f);
    }

    // ── Power-up hooks ──
    public void SetFireRateMultiplier(float m) { fireRateMultiplier = Mathf.Max(0.1f, m); }
    public void ResetFireRate() { fireRateMultiplier = 1f; }
    public void SetSpeedMultiplier(float m) { moveSpeed = baseMoveSpeed * m; }
    public void ResetSpeed() { moveSpeed = baseMoveSpeed; }
    public void GrantShield() { hasShield = true; ShowShield(true); }

    private void ShowShield(bool on)
    {
        if (on)
        {
            if (shieldVisual == null)
            {
                shieldVisual = new GameObject("Shield");
                shieldVisual.transform.SetParent(transform, false);
                var sr = shieldVisual.AddComponent<SpriteRenderer>();
                sr.sprite = shieldSprite != null ? shieldSprite : spriteRenderer.sprite;
                sr.color = new Color(0.3f, 0.6f, 1f, 0.45f);
                sr.sortingOrder = 7;
                shieldVisual.transform.localScale = Vector3.one * 1.7f;
            }
            shieldVisual.SetActive(true);
        }
        else if (shieldVisual != null)
        {
            shieldVisual.SetActive(false);
        }
    }
}
