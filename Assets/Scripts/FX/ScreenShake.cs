using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance { get; private set; }

    private Vector3 basePos;
    private float timer;
    private float magnitude;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
        basePos = transform.localPosition;
    }

    private void LateUpdate()
    {
        if (timer <= 0f) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            transform.localPosition = basePos;
            return;
        }
        transform.localPosition = basePos + (Vector3)(Random.insideUnitCircle * magnitude);
    }

    public static void Shake(float duration, float strength)
    {
        if (Instance == null) return;
        Instance.timer = duration;
        Instance.magnitude = strength;
    }
}
