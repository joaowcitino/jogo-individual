using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHP = 1;
    [SerializeField] private int scoreValue = 25;
    [Range(0f, 1f)][SerializeField] private float dropChance = 0.4f;

    private int hp;

    private void Awake() { hp = maxHP; }

    public void TakeDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0) Die();
    }

    private void Die()
    {
        if (GameManager.Instance != null) GameManager.Instance.AddScore(scoreValue);
        AudioManager.Instance?.PlayExplosion();
        FXManager.SpawnExplosion(transform.position);

        if (WaveManager.Instance != null) WaveManager.Instance.NotifyEnemyKilled();
        if (Random.value < dropChance && PowerUpSpawner.Instance != null)
            PowerUpSpawner.Instance.SpawnRandom(transform.position);

        Destroy(gameObject);
    }
}
