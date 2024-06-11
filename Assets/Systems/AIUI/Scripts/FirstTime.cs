using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FirstTime : MonoBehaviour
{

    [SerializeField] protected GameObject panel;
    [SerializeField] protected Button ContinueButton;
    // Start is called before the first frame update
    void Awake()
    {
        if (GameManager.Instance.firstGame)
        {
            panel.SetActive(true);
            ContinueButton.onClick.AddListener(ContinueGame);
            GameManager.Instance.ChangeState(GameState.Paused);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ContinueGame()
    {
        GameManager.Instance.firstGame = false;
        panel.SetActive(false);
        GameManager.Instance.ResumeGame();
    }
}
