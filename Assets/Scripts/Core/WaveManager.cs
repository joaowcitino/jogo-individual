using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [SerializeField] private GameObject patrolPrefab;
    [SerializeField] private GameObject shooterPrefab;
    [SerializeField] private float waveInterval = 20f;
    [SerializeField] private int maxEnemies = 8;

    private float waveTimer;
    private int aliveEnemies;

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

    private void Start()
    {
        waveTimer = waveInterval;
        aliveEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsGameActive) return;

        waveTimer -= Time.deltaTime;
        if (waveTimer <= 0f)
        {
            SpawnWave();
            waveTimer = waveInterval;
        }
    }

    private void SpawnWave()
    {
        if (aliveEnemies >= maxEnemies) return;
        SpawnAt(patrolPrefab, spawnPoints[Random.Range(0, spawnPoints.Length)]);
        if (aliveEnemies < maxEnemies)
            SpawnAt(shooterPrefab, spawnPoints[Random.Range(0, spawnPoints.Length)]);
    }

    private void SpawnAt(GameObject prefab, Vector2 pos)
    {
        if (prefab == null) return;
        Instantiate(prefab, pos, Quaternion.identity);
        aliveEnemies++;
    }

    public void NotifyEnemyKilled()
    {
        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);
    }
}
