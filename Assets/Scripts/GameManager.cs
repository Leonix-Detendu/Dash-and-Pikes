using UnityEngine;
using UnityEngine.Pool;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager instance;
    public bool gameIsPaused;
    public int score;
    public bool gameOver;
    private bool hasDisplayedGameOver;
    [SerializeField] private GameObject canvas;
    private UIManager UImanager;


    private void Update()
    {
        if (gameOver && !hasDisplayedGameOver)
        {
            UImanager.DisplayGameOver();
            PauseGame();
            hasDisplayedGameOver = true;
        }
    }

    private void Awake()
    {
        instance = this;
        UImanager = canvas.GetComponent<UIManager>();
        InitializeGame();
    }

    public void PauseGame()
    {
        gameIsPaused = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    public void ResumeGame()
    {
        gameIsPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    public void InitializeGame()
    {
        gameIsPaused = false;
        gameOver = false;
        hasDisplayedGameOver = false;
        score = 0;
        ResumeGame();
    }
}
