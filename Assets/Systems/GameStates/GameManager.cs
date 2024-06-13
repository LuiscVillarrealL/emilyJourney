using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    Dictionary<string, int> ScenesDictionary = new Dictionary<string, int>
    {
        { "MainMenu", 0 },
        { "MainScene", 1 },
         { "Upgrade", 2 },
         { "Pregame", 3 },
         { "Results", 4 },
         { "GameOver", 5 }
    };

    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }

    public float ActualStress = 0;
    public float LastTime = 0;
    public AIStat stressStat;

    public bool firstGame = true;

    public bool tutorialSecondLoopFinished = false;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager Initialized and set to DontDestroyOnLoad");

            // Initialize BlackboardManager
            if (BlackboardManager.Instance == null)
            {
                GameObject blackboardManagerObject = new GameObject("BlackboardManager");
                blackboardManagerObject.AddComponent<BlackboardManager>();
                DontDestroyOnLoad(blackboardManagerObject);
                Debug.Log("BlackboardManager Initialized and set to DontDestroyOnLoad");
            }

        }
        else
        {
            Debug.Log("Duplicate GameManager detected. Destroying this instance.");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Debug.Log("GameManager Start called. Changing state to MainMenu.");
        ChangeState(GameState.MainMenu);
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log($"Game state changed to {newState}");

        switch (newState)
        {
            case GameState.MainMenu:
                HandleMainMenu();
                break;
            case GameState.Playing:
                HandlePlaying();
                break;
            case GameState.Upgrading:
                HandleUpgrades();
                break;
            case GameState.Result:
                HandleResults();
                break;
            case GameState.Paused:
                HandlePaused();
                break;
            case GameState.GameOver:
                HandleGameOver();
                break;
            case GameState.Pregame:
                HandlePregame();
                break;
            default:
                Debug.LogWarning($"Unhandled game state: {newState}");
                break;
        }
    }

    private void HandleMainMenu()
    {
        Debug.Log("Loading MainMenu scene.");
        SceneManager.LoadScene("MainMenu");
        // Additional logic for main menu state
    }

    private void HandlePlaying()
    {

       
            if(ScenesDictionary["MainScene"] != SceneManager.GetActiveScene().buildIndex)
        {
            // BlackboardManager.Instance.setSt
            Debug.Log("Loading GameScene scene.");
            SceneManager.LoadScene(ScenesDictionary["MainScene"], LoadSceneMode.Single);
        }
        else
        {
            Debug.Log("Resuming game from paused state.");
            Time.timeScale = 1f;
        }





    }

    private void HandlePaused()
    {
        Debug.Log("Game paused.");
        Time.timeScale = 0f;
        // Show pause menu UI
    }

    private void HandleGameOver()
    {
        Debug.Log("Game over.");
        SceneManager.LoadScene(ScenesDictionary["GameOver"], LoadSceneMode.Single);
    }

    private void HandleUpgrades()
    {
        Debug.Log("Entering upgrade state.");
        SceneManager.LoadScene(ScenesDictionary["Upgrade"], LoadSceneMode.Single);
        // Handle upgrade logic
    }

    private void HandlePregame()
    {
        Debug.Log("Entering upgrade state.");
        SceneManager.LoadScene(ScenesDictionary["Pregame"], LoadSceneMode.Single);
        // Handle upgrade logic
    }

    private void HandleResults()
    {
        CountdownTimer Timer = FindObjectOfType<CountdownTimer>();

        LastTime = Timer.totalTime;

        Debug.Log("Entering result state.");
        SceneManager.LoadScene(ScenesDictionary["Results"], LoadSceneMode.Single);
        // Handle results logic
    }

    public void ResumeGame()
    {
        if (CurrentState == GameState.Paused)
        {

            ChangeState(GameState.Playing);
        }
    }


    public void QuitGame()
    {
        Debug.Log("Quitting game.");
        Application.Quit();
    }
}
