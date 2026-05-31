using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private Vector2 pointA = new Vector2(-3f, 0f);
    [SerializeField] private Vector2 pointB = new Vector2(3f, 0f);
    [SerializeField] private float speed = 1.5f;

    [Header("Aggro (phase 2)")]
    [SerializeField] private float chaseSpeed = 2.5f;
    [SerializeField] private float aggroTime = 30f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 currentTarget;
    private Transform playerTf;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        currentTarget = pointB;
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) playerTf = p.transform;
    }

    private void FixedUpdate()
    {
        bool aggro = GameManager.Instance != null && GameManager.Instance.ElapsedTime >= aggroTime;

        if (aggro && playerTf != null)
        {
            // Phase 2: chase the player directly
            Vector2 chasePos = Vector2.MoveTowards(rb.position, (Vector2)playerTf.position, chaseSpeed * Time.fixedDeltaTime);
            rb.MovePosition(chasePos);
            if (spriteRenderer != null) spriteRenderer.color = new Color(1f, 0.4f, 0.3f);
        }
        else
        {
            // Phase 1: patrol A <-> B
            Vector2 newPos = Vector2.MoveTowards(rb.position, currentTarget, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);

            if (Vector2.Distance(rb.position, currentTarget) < 0.05f)
                currentTarget = currentTarget == pointA ? pointB : pointA;

            if (spriteRenderer != null)
                spriteRenderer.flipX = currentTarget == pointA;
        }
    }
}
