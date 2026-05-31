using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private int damage = 1;
    [SerializeField] private bool hitsEnemies = true; // true = player bullet, false = enemy bullet

    private Rigidbody2D rb;
    private Vector2 direction = Vector2.up;

    private void Awake() { rb = GetComponent<Rigidbody2D>(); }

    public void Launch(Vector2 dir)
    {
        direction = dir.sqrMagnitude < 0.0001f ? Vector2.up : dir.normalized;
        if (rb != null) rb.linearVelocity = direction * speed;

        // Kenney lasers point up by default, so offset by -90°
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall")) { Destroy(gameObject); return; }

        if (hitsEnemies && other.CompareTag("Enemy"))
        {
            var hp = other.GetComponent<EnemyHealth>();
            if (hp != null) hp.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (!hitsEnemies && other.CompareTag("Player"))
        {
            var pc = other.GetComponent<PlayerController>();
            if (pc != null) pc.ApplyHit();
            Destroy(gameObject);
        }
    }
}
