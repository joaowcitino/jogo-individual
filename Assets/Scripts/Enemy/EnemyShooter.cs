using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyShooter : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float fireInterval = 2f;
    [SerializeField] private float fireIntervalFast = 1f;
    [SerializeField] private float fastAfter = 60f;
    [SerializeField] private float repositionRange = 3f;
    [SerializeField] private GameObject enemyProjectilePrefab;

    private Rigidbody2D rb;
    private Transform playerTf;
    private float fireTimer;
    private Vector2 target;

    private void Awake() { rb = GetComponent<Rigidbody2D>(); }

    private void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) playerTf = p.transform;
        target = rb.position;
        fireTimer = fireInterval;
    }

    private void FixedUpdate()
    {
        if (Vector2.Distance(rb.position, target) < 0.1f)
            target = rb.position + Random.insideUnitCircle * repositionRange;

        target.x = Mathf.Clamp(target.x, -6.5f, 6.5f);
        target.y = Mathf.Clamp(target.y, -4f, 4f);
        rb.MovePosition(Vector2.MoveTowards(rb.position, target, moveSpeed * Time.fixedDeltaTime));
    }

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsGameActive) return;

        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            FireAtPlayer();
            fireTimer = GameManager.Instance.ElapsedTime >= fastAfter ? fireIntervalFast : fireInterval;
        }
    }

    private void FireAtPlayer()
    {
        if (enemyProjectilePrefab == null || playerTf == null) return;
        var go = Instantiate(enemyProjectilePrefab, transform.position, Quaternion.identity);
        var proj = go.GetComponent<Projectile>();
        if (proj != null) proj.Launch(((Vector2)playerTf.position - rb.position).normalized);
        AudioManager.Instance?.PlayEnemyShoot();
    }
}
