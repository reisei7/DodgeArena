using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Ustawienia gry")]
    public int scoreToWin = 5;
    public float gameDuration = 120f;

    [Header("UI")]
    public TextMeshProUGUI scorePlayer1Text;
    public TextMeshProUGUI scorePlayer2Text;
    public TextMeshProUGUI timerText;
    public GameObject endGamePanel;
    public TextMeshProUGUI endGameText;

    private int scorePlayer1 = 0;
    private int scorePlayer2 = 0;
    private float timeLeft;
    private bool gameOver = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (GameOptions.Instance != null)
        {
            scoreToWin = GameOptions.Instance.scoreToWin;
            gameDuration = GameOptions.Instance.roundDuration;
            timeLeft = gameDuration;
        }
        else
        {
            timeLeft = gameDuration;
        }

        endGamePanel.SetActive(false);
        UpdateUI();
    }

    void Update()
    {
        if (gameOver) return;

        timeLeft -= Time.deltaTime;
        UpdateUI();

        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            EndGame();
        }
    }

    public void AddPoint(int playerNumber)
    {
        if (gameOver) return;

        if (playerNumber == 1) scorePlayer1++;
        else if (playerNumber == 2) scorePlayer2++;

        UpdateUI();

        if (scorePlayer1 >= scoreToWin || scorePlayer2 >= scoreToWin)
            EndGame();
    }

    void UpdateUI()
    {
        if (scorePlayer1Text != null) scorePlayer1Text.text = "P1: " + scorePlayer1;
        if (scorePlayer2Text != null) scorePlayer2Text.text = "P2: " + scorePlayer2;

        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(timeLeft);
            int min = seconds / 60;
            int sec = seconds % 60;
            timerText.text = string.Format("{0:0}:{1:00}", min, sec);
        }
    }

    void EndGame()
    {
        gameOver = true;
        endGamePanel.SetActive(true);

        if (scorePlayer1 > scorePlayer2)
            endGameText.text = "Wygrywa Gracz 1!";
        else if (scorePlayer2 > scorePlayer1)
            endGameText.text = "Wygrywa Gracz 2!";
        else
            endGameText.text = "Remis!";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}