using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private float timeLimit = 60f;
    [SerializeField] private int totalCrystals = 10;
    [SerializeField] private int startingLives = 3;

    public float TimeRemaining { get; private set; }
    public int Score { get; private set; }
    public int Lives { get; private set; }
    public int CrystalsCollected { get; private set; }
    public int TotalCrystals => totalCrystals;
    public bool IsGameActive { get; private set; }

    public System.Action<float> OnTimeChanged;
    public System.Action<int> OnScoreChanged;
    public System.Action<int> OnLivesChanged;
    public System.Action OnGameOver;
    public System.Action OnVictory;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        TimeRemaining = timeLimit;
        Lives = startingLives;
        Score = 0;
        CrystalsCollected = 0;
        IsGameActive = true;

        OnTimeChanged?.Invoke(TimeRemaining);
        OnScoreChanged?.Invoke(Score);
        OnLivesChanged?.Invoke(Lives);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if (!IsGameActive) return;

        TimeRemaining -= Time.deltaTime;
        if (TimeRemaining < 0f) TimeRemaining = 0f;
        OnTimeChanged?.Invoke(TimeRemaining);

        if (TimeRemaining <= 0f)
            TriggerGameOver();
    }

    public void CollectCrystal()
    {
        if (!IsGameActive) return;
        CrystalsCollected++;
        Score += 10;
        OnScoreChanged?.Invoke(Score);
        if (CrystalsCollected >= totalCrystals)
            TriggerVictory();
    }

    public void TakeDamage()
    {
        if (!IsGameActive) return;
        Lives = Mathf.Max(0, Lives - 1);
        OnLivesChanged?.Invoke(Lives);
        if (Lives <= 0)
            TriggerGameOver();
    }

    private void TriggerVictory()
    {
        if (!IsGameActive) return;
        IsGameActive = false;
        WriteGameOverData(true);
        OnVictory?.Invoke();
        Invoke(nameof(LoadGameOver), 1.5f);
    }

    private void TriggerGameOver()
    {
        if (!IsGameActive) return;
        IsGameActive = false;
        WriteGameOverData(false);
        OnGameOver?.Invoke();
        Invoke(nameof(LoadGameOver), 1.5f);
    }

    private void WriteGameOverData(bool victory)
    {
        GameOverData.IsVictory = victory;
        GameOverData.FinalScore = Score;
        GameOverData.TimeRemaining = TimeRemaining;
        GameOverData.CrystalsCollected = CrystalsCollected;
        GameOverData.TotalCrystals = totalCrystals;
    }

    private void LoadGameOver() => SceneManager.LoadScene("GameOver");
}
