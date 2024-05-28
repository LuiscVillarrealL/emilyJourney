using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ChangeState(GameState.MainMenu);
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;

        switch (newState)
        {
            case GameState.MainMenu:
                HandleMainMenu();
                break;
            case GameState.Playing:
                HandlePlaying();
                break;
            case GameState.Paused:
                HandlePaused();
                break;
            case GameState.GameOver:
                HandleGameOver();
                break;
            default:
                break;
        }
    }

    private void HandleMainMenu()
    {
        // Load main menu scene
        SceneManager.LoadScene("MainMenu");
        // Additional logic for main menu state
    }

    private void HandlePlaying()
    {
        // Start the game
        SceneManager.LoadScene("GameScene");
        // Additional logic for playing state
    }

    private void HandlePaused()
    {
        // Pause the game
        Time.timeScale = 0f;
        // Show pause menu UI
    }

    private void HandleGameOver()
    {
        // Handle game over logic
        // Show game over screen
    }

    private void HandleUpgrades()
    {
        // Handle game over logic
        // Show game over screen
    }

    private void HandleResults()
    {
        // Handle game over logic
        // Show game over screen
    }

    public void ResumeGame()
    {
        if (CurrentState == GameState.Paused)
        {
            Time.timeScale = 1f;
            ChangeState(GameState.Playing);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
