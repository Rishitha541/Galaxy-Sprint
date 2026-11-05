using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("HUD Text")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Text coinText;

    [Header("Game Over Text")]
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text finalHighScoreText;

    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button[] restartButtons;
    [SerializeField] private Button[] quitButtons;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        WireButtons();
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged += UpdateGameplayUI;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged -= UpdateGameplayUI;
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.Instance.State == GameState.Playing)
            {
                GameManager.Instance.PauseGame();
            }
            else if (GameManager.Instance.State == GameState.Paused)
            {
                GameManager.Instance.ResumeGame();
            }
        }
    }

    public void ShowMainMenu()
    {
        SetPanel(mainMenuPanel, true);
        SetPanel(hudPanel, false);
        SetPanel(pausePanel, false);
        SetPanel(gameOverPanel, false);
    }

    public void ShowGameplay()
    {
        SetPanel(mainMenuPanel, false);
        SetPanel(hudPanel, true);
        SetPanel(pausePanel, false);
        SetPanel(gameOverPanel, false);
    }

    public void ShowPauseMenu()
    {
        SetPanel(mainMenuPanel, false);
        SetPanel(hudPanel, true);
        SetPanel(pausePanel, true);
        SetPanel(gameOverPanel, false);
    }

    public void ShowGameOver(int finalScore, int highScore)
    {
        SetPanel(mainMenuPanel, false);
        SetPanel(hudPanel, false);
        SetPanel(pausePanel, false);
        SetPanel(gameOverPanel, true);

        if (finalScoreText != null)
        {
            finalScoreText.text = $"Score: {finalScore}";
        }

        if (finalHighScoreText != null)
        {
            finalHighScoreText.text = $"High Score: {highScore}";
        }
    }

    public void UpdateGameplayUI(int score, int highScore, int coins)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }

        if (highScoreText != null)
        {
            highScoreText.text = $"High Score: {highScore}";
        }

        if (coinText != null)
        {
            coinText.text = $"Coins: {coins}";
        }
    }

    private void WireButtons()
    {
        startButton?.onClick.AddListener(() => GameManager.Instance?.StartGame());
        pauseButton?.onClick.AddListener(() => GameManager.Instance?.PauseGame());
        resumeButton?.onClick.AddListener(() => GameManager.Instance?.ResumeGame());

        if (restartButtons != null)
        {
            foreach (Button restartButton in restartButtons)
            {
                restartButton?.onClick.AddListener(() => GameManager.Instance?.RestartGame());
            }
        }

        if (quitButtons != null)
        {
            foreach (Button quitButton in quitButtons)
            {
                quitButton?.onClick.AddListener(() => GameManager.Instance?.QuitGame());
            }
        }
    }

    private static void SetPanel(GameObject panel, bool isActive)
    {
        if (panel != null)
        {
            panel.SetActive(isActive);
        }
    }
}
