using UnityEngine;

/// <summary>
/// Spawns short-lived particle bursts at runtime. No prefab required —
/// the ParticleSystem is built and configured in code, then self-destructs.
/// </summary>
public static class FXManager
{
    public static void SpawnExplosion(Vector3 position)
    {
        SpawnBurst(position, new Color(1f, 0.6f, 0.1f), 10, 0.2f);
    }

    public static void SpawnSparkle(Vector3 position, Color color)
    {
        SpawnBurst(position, color, 6, 0.12f);
    }

    private static void SpawnBurst(Vector3 position, Color color, int count, float size)
    {
        var go = new GameObject("FXBurst");
        go.transform.position = position;

        var ps = go.AddComponent<ParticleSystem>();
        ps.Stop();

        var main = ps.main;
        main.duration = 0.4f;
        main.loop = false;
        main.startLifetime = 0.35f;
        main.startSpeed = 4f;
        main.startSize = size;
        main.startColor = color;
        main.stopAction = ParticleSystemStopAction.Destroy;

        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, (short)count) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.1f;

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Sprites/Default"));
        renderer.sortingOrder = 8;

        ps.Play();
    }
}
