using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private Vector2 pointA = new Vector2(-3f, 0f);
    [SerializeField] private Vector2 pointB = new Vector2(3f, 0f);
    [SerializeField] private float speed = 2f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 currentTarget;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        currentTarget = pointB;
    }

    private void FixedUpdate()
    {
        Vector2 newPos = Vector2.MoveTowards(rb.position, currentTarget, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        if (Vector2.Distance(rb.position, currentTarget) < 0.05f)
            currentTarget = currentTarget == pointA ? pointB : pointA;

        if (spriteRenderer != null)
            spriteRenderer.flipX = currentTarget == pointA;
    }
}
