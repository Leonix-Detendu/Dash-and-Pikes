using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject gameUI;

    [SerializeField] private GameObject tutorial;
    [SerializeField] private GameObject[] tutorialPages;
    private int indexOfCurrentTP;

    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Dropdown graphicsDropdown;
    [SerializeField] private Toggle fullScreenToggle;

    [SerializeField] private GameObject[] hearts;

    [SerializeField] private TextMeshProUGUI scoreText;

    private GameObject currentMenu;
    private GameObject lastMenu;

    private void Start()
    {
        // 1. Initialiser les sliders et toggles avec les réglages persistants
        InitializeSettingsUI();

        // 2. Gestion de l'affichage selon la scène
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Main Menu")
        {
            DisplayMainMenu();
            MaskSettingsMenu();
            MaskTutorial();
            ActualizeTutorialPage();

            InitializeSettingsUI();
        }
        else if (sceneName == "Game")
        {
            MaskPauseMenu();
            MaskSettingsMenu();
            MaskGameOver();
            DisplayGameUI();

            if (Settings.instance != null)
            {
                Settings.instance.ApplyAllSettings();
            }

            InitializeSettingsUI();
        }

        indexOfCurrentTP = 0;
    }

    private void InitializeSettingsUI()
    {
        if (Settings.instance == null) return;

        SettingsData data = Settings.instance.CurrentSettings;

        // Pré-remplir les valeurs sans déclencher les callbacks prématurément
        if (volumeSlider != null)
        {
            volumeSlider.value = data.volume;
            // On branche le slider sur la méthode du Settings global
            volumeSlider.onValueChanged.AddListener(Settings.instance.SetVolume);
        }

        if (graphicsDropdown != null)
        {
            graphicsDropdown.value = data.qualityIndex;
            graphicsDropdown.onValueChanged.AddListener(Settings.instance.SetQuality);
        }

        if (fullScreenToggle != null)
        {
            fullScreenToggle.isOn = data.isFullScreen;
            fullScreenToggle.onValueChanged.AddListener(Settings.instance.SetFullScreen);
        }
    }

    private void OnDestroy()
    {
        // Nettoyer les écouteurs d'événements à la fermeture de la scène
        if (Settings.instance != null)
        {
            if (volumeSlider != null) volumeSlider.onValueChanged.RemoveListener(Settings.instance.SetVolume);
            if (graphicsDropdown != null) graphicsDropdown.onValueChanged.RemoveListener(Settings.instance.SetQuality);
            if (fullScreenToggle != null) fullScreenToggle.onValueChanged.RemoveListener(Settings.instance.SetFullScreen);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name == "Game")
            {
                Pause();
            }
        }
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(1);
        GameManager.instance.InitializeGame();
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void ReturnToMainMenu() => SceneManager.LoadScene(0);

    public void DisplayMainMenu()
    {
        if (mainMenu != null) mainMenu.SetActive(true);
        currentMenu = mainMenu;
    }

    public void MaskMainMenu()
    {
        if (mainMenu != null) mainMenu.SetActive(false);
        lastMenu = mainMenu;
    }

    public void DisplaySettingsMenu()
    {
        if (settingsMenu != null) settingsMenu.SetActive(true);
        currentMenu = settingsMenu;
    }

    public void MaskSettingsMenu()
    {
        if (settingsMenu != null) settingsMenu.SetActive(false);
        lastMenu = settingsMenu;

        if (Settings.instance != null)
        {
            Settings.instance.SaveSettings();
        }
    }

    public void Resume()
    {
        if (GameManager.instance != null) GameManager.instance.ResumeGame();
        MaskPauseMenu();
    }

    public void Pause()
    {
        if (GameManager.instance != null) GameManager.instance.PauseGame();
        DisplayPauseMenu();
    }

    public void MaskPauseMenu()
    {
        if (pauseMenu != null) pauseMenu.SetActive(false);
        lastMenu = pauseMenu;
    }

    public void DisplayPauseMenu()
    {
        if (pauseMenu != null) pauseMenu.SetActive(true);
        currentMenu = pauseMenu;
    }

    public void Back()
    {
        GameObject tempMenu = lastMenu;
        if (lastMenu != null) lastMenu.SetActive(true);

        if (currentMenu != null)
        { 
            currentMenu.SetActive(false);
        }
        currentMenu = lastMenu;
        lastMenu = tempMenu;
    }

    public void ActualizeHealth( int nbOfHearts )
    {
        if (hearts != null && hearts.Length > 0)
        {
            for (int i = 0; i < hearts.Length; i++ )
            {
                if ( i >= nbOfHearts)
                {
                    hearts[i].SetActive(false);
                }
                else
                {
                    hearts[i].SetActive(true);
                }
            }
        }
    }

    public void ActualizeScore( int score )
    {
        scoreText.text = "Levels Completed " + score;
    }

    public void DisplayGameOver()
    {
        if (gameOver != null) gameOver.SetActive(true);
    }

    public void MaskGameOver()
    {
        if (gameOver != null) gameOver.SetActive(false);
    }

    public void DisplayGameUI()
    {
        if (gameUI != null) gameUI.SetActive(true);
    }

    public void DisplayTutorial()
    {
        if (tutorial != null) tutorial.SetActive(true);
        currentMenu = tutorial;
    }

    public void MaskTutorial()
    {
        if (tutorial != null) tutorial.SetActive(false);
        lastMenu = tutorial;
    }

    public void NextTutorialPage()
    {
        indexOfCurrentTP++;
        ActualizeTutorialPage();
    }

    public void PreviousTutorialPage()
    {
        indexOfCurrentTP--;
        ActualizeTutorialPage();
    }

    private void ActualizeTutorialPage()
    {
        for( int i = 0; i < tutorialPages.Length; i++ )
        {
            if ( i == indexOfCurrentTP)
            {
                tutorialPages[i].SetActive(true);
            }
            else
            {
                tutorialPages[i].SetActive(false);
            }
        }
    }
}