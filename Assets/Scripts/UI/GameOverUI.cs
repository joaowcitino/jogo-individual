using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI crystalsText;

    [Header("Background swap (optional)")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Sprite victoryBackground;
    [SerializeField] private Sprite defeatBackground;

    private void Start()
    {
        if (backgroundImage != null)
        {
            var bg = GameOverData.IsVictory ? victoryBackground : defeatBackground;
            if (bg != null) backgroundImage.sprite = bg;
        }

        if (resultText != null)
        {
            resultText.text = GameOverData.IsVictory ? "MISSAO CONCLUIDA!" : "FALHA NA MISSAO";
            resultText.color = GameOverData.IsVictory
                ? new Color(0f, 1f, 0.4f)
                : new Color(1f, 0.3f, 0.1f);
        }

        if (scoreText != null)
            scoreText.text = $"Score Final: {GameOverData.FinalScore}";

        if (timeText != null)
            timeText.text = $"O2 Restante: {Mathf.CeilToInt(GameOverData.TimeRemaining)}s";

        if (crystalsText != null)
            crystalsText.text = $"Cristais: {GameOverData.CrystalsCollected}/{GameOverData.TotalCrystals}";
    }

    public void OnRestartButton() => SceneManager.LoadScene("Game");
    public void OnMainMenuButton() => SceneManager.LoadScene("MainMenu");
}
