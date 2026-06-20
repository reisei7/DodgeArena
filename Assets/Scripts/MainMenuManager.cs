using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panele")]
    public GameObject mainPanel;
    public GameObject modePanel;
    public GameObject bulletSelectPanel;

    [Header("Bullet Select - referencje")]
    public BulletSelectScreen bulletSelectScreen;
    public TMPro.TextMeshProUGUI selectHeaderText;

    private int currentSelectingPlayer = 1;
    private bool isVsAI = false;

    void Start()
    {
        ShowMainPanel();
    }

    public void ShowMainPanel()
    {
        mainPanel.SetActive(true);
        modePanel.SetActive(false);
        bulletSelectPanel.SetActive(false);
    }

    public void ShowModePanel()
    {
        mainPanel.SetActive(false);
        modePanel.SetActive(true);
        bulletSelectPanel.SetActive(false);
    }

    public void SelectMode1v1()
    {
        isVsAI = false;
        GameSettings.Instance.isVsAI = false;
        currentSelectingPlayer = 1;
        ShowBulletSelectScreen();
    }

    public void SelectModeVsAI()
    {
        isVsAI = true;
        GameSettings.Instance.isVsAI = true;
        currentSelectingPlayer = 1;
        ShowBulletSelectScreen();
    }

    private void ShowBulletSelectScreen()
    {
        mainPanel.SetActive(false);
        modePanel.SetActive(false);
        bulletSelectPanel.SetActive(true);

        if (selectHeaderText != null)
            selectHeaderText.text = $"Gracz {currentSelectingPlayer} — wybierz typ pocisku";
    }

    // Wywoływane przez przycisk Confirm na ekranie wyboru pocisku
    public void ConfirmBulletSelection()
    {
        BulletType chosen = bulletSelectScreen.GetSelectedType();

        if (currentSelectingPlayer == 1)
        {
            GameSettings.Instance.player1BulletType = chosen;

            if (isVsAI)
            {
                // vs AI: tylko gracz 1 wybiera, od razu start gry
                StartGame();
            }
            else
            {
                // 1v1: teraz kolej gracza 2
                currentSelectingPlayer = 2;
                bulletSelectPanel.SetActive(false);
                bulletSelectPanel.SetActive(true); // reset wyboru (OnEnable w BulletSelectScreen czyści zaznaczenie)

                if (selectHeaderText != null)
                    selectHeaderText.text = $"Gracz {currentSelectingPlayer} — wybierz typ pocisku";
            }
        }
        else if (currentSelectingPlayer == 2)
        {
            GameSettings.Instance.player2BulletType = chosen;
            StartGame();
        }
    }

    private void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

        public void GoBackFromModePanel()
    {
        ShowMainPanel();
    }

    public void GoBackFromBulletSelect()
    {
        if (currentSelectingPlayer == 2)
        {
            // Cofnij gracza 2 do wyboru gracza 1
            currentSelectingPlayer = 1;
            bulletSelectPanel.SetActive(false);
            bulletSelectPanel.SetActive(true);

            if (selectHeaderText != null)
                selectHeaderText.text = $"Gracz {currentSelectingPlayer} — wybierz typ pocisku";
        }
        else
        {
            // Cofnij do wyboru trybu
            ShowModePanel();
        }
    }
        public void ExitGame()
    {
        Application.Quit();

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }
    public void PlayClickSound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayUiClick();
    }
}