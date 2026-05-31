using UnityEngine;

/// <summary>
/// Keeps a target number of crystals present in the arena at all times.
/// When the player collects one (it gets destroyed), this respawns a
/// replacement at a random free position, creating a continuous collect loop.
/// </summary>
public class CrystalSpawner : MonoBehaviour
{
    [SerializeField] private GameObject crystalPrefab;
    [SerializeField] private int targetOnMap = 8;

    [Header("Spawn area (arena bounds, inside the walls)")]
    [SerializeField] private Vector2 areaMin = new Vector2(-6.5f, -4f);
    [SerializeField] private Vector2 areaMax = new Vector2(6.5f, 4f);
    [SerializeField] private float minDistanceFromPlayer = 1.5f;

    private Transform playerTf;

    private void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) playerTf = p.transform;

        // Fill the arena to the target immediately
        TopUp();
    }

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsGameActive) return;
        TopUp();
    }

    private void TopUp()
    {
        if (crystalPrefab == null) return;
        int current = GameObject.FindGameObjectsWithTag("Coletavel").Length;
        for (int i = current; i < targetOnMap; i++)
            SpawnOne();
    }

    private void SpawnOne()
    {
        Vector3 pos = Vector3.zero;
        for (int attempt = 0; attempt < 10; attempt++)
        {
            pos = new Vector3(
                Random.Range(areaMin.x, areaMax.x),
                Random.Range(areaMin.y, areaMax.y),
                0f);
            if (playerTf == null ||
                Vector2.Distance(pos, playerTf.position) >= minDistanceFromPlayer)
                break;
        }
        Instantiate(crystalPrefab, pos, Quaternion.identity);
    }
}
