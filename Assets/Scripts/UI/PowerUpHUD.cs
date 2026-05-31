using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerUpHUD : MonoBehaviour
{
    public static PowerUpHUD Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Image fillBar;

    private float duration;
    private float timer;
    private bool active;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
        if (label != null) label.text = "";
        if (fillBar != null) fillBar.fillAmount = 0f;
    }

    public void Show(PowerUpType type, float dur)
    {
        active = true;
        duration = dur;
        timer = dur;
        if (label != null) label.text = TypeName(type);
        if (fillBar != null) fillBar.fillAmount = 1f;
    }

    private void Update()
    {
        if (!active) return;
        timer -= Time.deltaTime;
        if (fillBar != null && duration > 0f) fillBar.fillAmount = Mathf.Clamp01(timer / duration);
        if (timer <= 0f)
        {
            active = false;
            if (label != null) label.text = "";
            if (fillBar != null) fillBar.fillAmount = 0f;
        }
    }

    private string TypeName(PowerUpType t)
    {
        switch (t)
        {
            case PowerUpType.O2: return "O2 +15s";
            case PowerUpType.RapidFire: return "TIRO RAPIDO";
            case PowerUpType.Shield: return "ESCUDO";
            case PowerUpType.Speed: return "VELOCIDADE";
        }
        return "";
    }
}
