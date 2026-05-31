using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public static PowerUpSpawner Instance { get; private set; }

    [SerializeField] private GameObject[] powerUpPrefabs; // O2, Rapid, Shield, Speed

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void SpawnRandom(Vector3 position)
    {
        if (powerUpPrefabs == null || powerUpPrefabs.Length == 0) return;
        var prefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
        if (prefab != null) Instantiate(prefab, position, Quaternion.identity);
    }
}
