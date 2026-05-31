using UnityEngine;

/// <summary>
/// Spawns enemies in waves whose frequency, size and shooter ratio scale with
/// GameManager.ElapsedTime, so the arena gets steadily more dangerous.
/// </summary>
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [SerializeField] private GameObject patrolPrefab;
    [SerializeField] private GameObject shooterPrefab;

    [Header("Difficulty ramp (scales with elapsed time)")]
    [SerializeField] private float startInterval = 5f;
    [SerializeField] private float minInterval = 1.5f;
    [SerializeField] private float rampDuration = 90f;
    [SerializeField] private int baseMaxEnemies = 6;
    [SerializeField] private int maxEnemiesCap = 20;
    [SerializeField] private float shootersStartAt = 15f;

    private float waveTimer;

    private static readonly Vector2[] spawnPoints =
    {
        new Vector2(-6f, 3.5f), new Vector2(6f, 3.5f),
        new Vector2(-6f, -3.5f), new Vector2(6f, -3.5f)
    };

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start() { waveTimer = startInterval; }

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsGameActive) return;

        waveTimer -= Time.deltaTime;
        if (waveTimer <= 0f)
        {
            SpawnWave();
            waveTimer = CurrentInterval();
        }
    }

    private float Elapsed => GameManager.Instance != null ? GameManager.Instance.ElapsedTime : 0f;

    private float Progress => Mathf.Clamp01(Elapsed / rampDuration);

    private float CurrentInterval() => Mathf.Lerp(startInterval, minInterval, Progress);

    private int CurrentMaxEnemies()
    {
        int extra = Mathf.FloorToInt(Elapsed / 15f) * 2;
        return Mathf.Min(baseMaxEnemies + extra, maxEnemiesCap);
    }

    private void SpawnWave()
    {
        int alive = GameObject.FindGameObjectsWithTag("Enemy").Length;
        int max = CurrentMaxEnemies();
        if (alive >= max) return;

        float shooterChance = Elapsed < shootersStartAt ? 0f : Mathf.Lerp(0.2f, 0.5f, Progress);
        int perWave = 2 + Mathf.FloorToInt(Progress * 2f); // 2..4 enemies per wave

        for (int i = 0; i < perWave && alive < max; i++)
        {
            bool shooter = shooterPrefab != null && Random.value < shooterChance;
            var prefab = shooter ? shooterPrefab : patrolPrefab;
            SpawnAt(prefab, spawnPoints[Random.Range(0, spawnPoints.Length)]);
            alive++;
        }
    }

    private void SpawnAt(GameObject prefab, Vector2 pos)
    {
        if (prefab != null) Instantiate(prefab, pos, Quaternion.identity);
    }

    // Alive count is polled by tag, so this is now a no-op (kept for callers).
    public void NotifyEnemyKilled() { }
}
