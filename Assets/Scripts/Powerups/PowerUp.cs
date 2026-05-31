using System.Collections;
using UnityEngine;

public enum PowerUpType { O2, RapidFire, Shield, Speed }

[RequireComponent(typeof(Collider2D))]
public class PowerUp : MonoBehaviour
{
    [SerializeField] private PowerUpType type;
    [SerializeField] private float lifetime = 8f;
    [SerializeField] private float o2Bonus = 15f;
    [SerializeField] private float effectDuration = 5f;

    public PowerUpType Type => type;

    private void Start() { Destroy(gameObject, lifetime); }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var pc = other.GetComponent<PlayerController>();
        Apply(pc);
        AudioManager.Instance?.PlayPowerUp();
        if (PowerUpHUD.Instance != null) PowerUpHUD.Instance.Show(type, effectDuration);
        Destroy(gameObject);
    }

    private void Apply(PlayerController pc)
    {
        switch (type)
        {
            case PowerUpType.O2:
                GameManager.Instance?.AddTime(o2Bonus);
                break;
            case PowerUpType.RapidFire:
                if (pc != null) pc.StartCoroutine(RapidFireRoutine(pc));
                break;
            case PowerUpType.Shield:
                if (pc != null) pc.GrantShield();
                break;
            case PowerUpType.Speed:
                if (pc != null) pc.StartCoroutine(SpeedRoutine(pc));
                break;
        }
    }

    // Coroutines run on the PlayerController so they survive this object's Destroy.
    private IEnumerator RapidFireRoutine(PlayerController pc)
    {
        pc.SetFireRateMultiplier(3f);
        yield return new WaitForSeconds(effectDuration);
        pc.ResetFireRate();
    }

    private IEnumerator SpeedRoutine(PlayerController pc)
    {
        pc.SetSpeedMultiplier(2f);
        yield return new WaitForSeconds(effectDuration);
        pc.ResetSpeed();
    }
}
