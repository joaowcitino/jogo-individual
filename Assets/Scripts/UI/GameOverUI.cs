using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI crystalsText;

    private void Start()
    {
        if (resultText != null)
        {
            resultText.text = GameOverData.IsVictory ? "✓ MISSAO CONCLUIDA!" : "✗ FALHA NA MISSAO";
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
