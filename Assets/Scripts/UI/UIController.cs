using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    [Header("HUD Text")]
    [SerializeField] private TextMeshProUGUI o2Text;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI crystalsText;

    [Header("Low O2 Warning")]
    [SerializeField] private float lowO2Threshold = 15f;

    private GameManager gm;

    private void Start()
    {
        gm = GameManager.Instance;
        if (gm == null) { Debug.LogWarning("UIController: GameManager not found"); return; }

        gm.OnTimeChanged += UpdateTime;
        gm.OnScoreChanged += UpdateScore;
        gm.OnLivesChanged += UpdateLives;

        UpdateTime(gm.TimeRemaining);
        UpdateScore(gm.Score);
        UpdateLives(gm.Lives);
        UpdateCrystals(0);
    }

    private void OnDestroy()
    {
        if (gm == null) return;
        gm.OnTimeChanged -= UpdateTime;
        gm.OnScoreChanged -= UpdateScore;
        gm.OnLivesChanged -= UpdateLives;
    }

    private void UpdateTime(float time)
    {
        if (o2Text == null) return;
        int secs = Mathf.CeilToInt(time);
        o2Text.text = $"O2: {secs}s";
        o2Text.color = time <= lowO2Threshold
            ? new Color(1f, 0.2f + 0.8f * Mathf.PingPong(Time.time * 3f, 1f), 0f)
            : new Color(0f, 1f, 0.8f);
    }

    private void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
        UpdateCrystals(gm != null ? gm.CrystalsCollected : 0);
    }

    private void UpdateLives(int lives)
    {
        if (livesText == null) return;
        string h = "";
        for (int i = 0; i < 3; i++)
            h += i < lives ? "♥ " : "○ ";
        livesText.text = h.TrimEnd();
    }

    private void UpdateCrystals(int collected)
    {
        if (crystalsText == null || gm == null) return;
        crystalsText.text = $"Cristais: {collected}/{gm.TotalCrystals}";
    }
}
