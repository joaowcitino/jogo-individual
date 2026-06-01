using UnityEngine;

/// <summary>
/// Makes a dropped power-up eye-catching: gentle pulsing scale + slow spin,
/// so the player notices the benefit that fell from a destroyed ship.
/// </summary>
public class PowerUpVisual : MonoBehaviour
{
    [SerializeField] private float pulseAmount = 0.15f;
    [SerializeField] private float pulseSpeed = 4f;
    [SerializeField] private float spinSpeed = 90f;

    private Vector3 baseScale;

    private void Awake() { baseScale = transform.localScale; }

    private void Update()
    {
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = baseScale * pulse;
        transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
    }
}
