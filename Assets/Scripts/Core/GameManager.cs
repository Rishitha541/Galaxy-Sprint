using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Speed")]
    [SerializeField] private float startingSpeed = 8f;
    [SerializeField] private float maxSpeed = 24f;
    [SerializeField] private float speedIncreasePerSecond = 0.18f;

    [Header("Score")]
    [SerializeField] private float distanceScoreMultiplier = 1f;
    [SerializeField] private int coinScoreValue = 25;

    private const string HighScoreKey = "ENDLESS_RUNNER_HIGH_SCORE";

    public GameState State { get; private set; } = GameState.MainMenu;
    public float CurrentSpeed { get; private set; }
    public float DistanceTravelled { get; private set; }
    public int Coins { get; private set; }
    public int HighScore { get; private set; }

    public bool IsPlaying => State == GameState.Playing;
    public int Score => Mathf.FloorToInt(DistanceTravelled * distanceScoreMultiplier) + Coins * coinScoreValue;

    public event Action OnGameStarted;
    public event Action OnGamePaused;
    public event Action OnGameResumed;
    public event Action OnGameOver;
    public event Action<int, int, int> OnScoreChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        Time.timeScale = 1f;
    }

    private void Start()
    {
        CurrentSpeed = 0f;
        UIManager.Instance?.ShowMainMenu();
        NotifyScoreChanged();
    }

    private void Update()
    {
        if (!IsPlaying)
        {
            return;
        }

        CurrentSpeed = Mathf.Min(CurrentSpeed + speedIncreasePerSecond * Time.deltaTime, maxSpeed);
        DistanceTravelled += CurrentSpeed * Time.deltaTime;
        NotifyScoreChanged();
    }

    public void StartGame()
    {
        ResetRunStats();
        State = GameState.Playing;
        Time.timeScale = 1f;

        AudioManager.Instance?.PlayMusic();
        UIManager.Instance?.ShowGameplay();
        OnGameStarted?.Invoke();
        NotifyScoreChanged();
    }

    public void PauseGame()
    {
        if (State != GameState.Playing)
        {
            return;
        }

        State = GameState.Paused;
        Time.timeScale = 0f;

        AudioManager.Instance?.PauseMusic();
        UIManager.Instance?.ShowPauseMenu();
        OnGamePaused?.Invoke();
    }

    public void ResumeGame()
    {
        if (State != GameState.Paused)
        {
            return;
        }

        State = GameState.Playing;
        Time.timeScale = 1f;

        AudioManager.Instance?.ResumeMusic();
        UIManager.Instance?.ShowGameplay();
        OnGameResumed?.Invoke();
    }

    public void GameOver()
    {
        if (State == GameState.GameOver)
        {
            return;
        }

        State = GameState.GameOver;
        Time.timeScale = 1f;

        SaveHighScoreIfNeeded();
        AudioManager.Instance?.StopMusic();
        AudioManager.Instance?.PlayGameOver();
        UIManager.Instance?.ShowGameOver(Score, HighScore);
        OnGameOver?.Invoke();
        NotifyScoreChanged();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        SaveHighScoreIfNeeded();
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void AddCoin(int amount = 1)
    {
        if (!IsPlaying)
        {
            return;
        }

        Coins += Mathf.Max(1, amount);
        AudioManager.Instance?.PlayCoin();
        NotifyScoreChanged();
    }

    private void ResetRunStats()
    {
        CurrentSpeed = startingSpeed;
        DistanceTravelled = 0f;
        Coins = 0;
    }

    private void SaveHighScoreIfNeeded()
    {
        if (Score <= HighScore)
        {
            return;
        }

        HighScore = Score;
        PlayerPrefs.SetInt(HighScoreKey, HighScore);
        PlayerPrefs.Save();
    }

    private void NotifyScoreChanged()
    {
        OnScoreChanged?.Invoke(Score, HighScore, Coins);
        UIManager.Instance?.UpdateGameplayUI(Score, HighScore, Coins);
    }
}
