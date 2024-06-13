using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FirstTime : MonoBehaviour
{

    [SerializeField] protected GameObject panel;
    [SerializeField] protected GameObject[] tutorialMasks1;
    [SerializeField] protected GameObject[] tutorialMasks2;
     
    [SerializeField] protected Button ContinueButton1;
    [SerializeField] protected Button ContinueButton2;

    private GameObject actualPanel;
    private int ActualNumPanel = 0;
    private int MaxNumPanel = 0;

    public int ActualNumPanel2 = -1;
    public int MaxNumPanel2 = 0;

    public bool secondLoopFinish = false;

    public bool playing = true;
    // Start is called before the first frame update
    void Awake()
    {
        if (GameManager.Instance.firstGame)
        {
            panel.SetActive(true);
            MaxNumPanel = tutorialMasks1.Length;
            MaxNumPanel2 = tutorialMasks2.Length;
            actualPanel = tutorialMasks1[0];
            actualPanel.SetActive(true);
            ContinueButton1.onClick.AddListener(NextMessage);
            ContinueButton2.onClick.AddListener(ContinueGame);
            GameManager.Instance.ChangeState(GameState.Paused);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ContinueGame()
    {
        panel.SetActive(false);
        GameManager.Instance.ResumeGame();
    }

    public void NextMessage()
    {
        ActualNumPanel++;
        if (ActualNumPanel < MaxNumPanel)
        {
            actualPanel.SetActive(false);
            
            actualPanel = tutorialMasks1[ActualNumPanel];
            actualPanel.SetActive(true);
        }
        else
        {
            actualPanel.SetActive(false);
            ContinueButton1.gameObject.SetActive(false);
            ContinueButton2.gameObject.SetActive(true);
            GameManager.Instance.firstGame = false;
            ContinueTutorial();
        }

        

    }

    public void ContinueTutorial()
    {

        GameManager.Instance.ChangeState(GameState.Paused);
        panel.SetActive(true);

        ActualNumPanel2++;
            if (ActualNumPanel2 < MaxNumPanel2)
            {
                actualPanel.SetActive(false);

                actualPanel = tutorialMasks2[ActualNumPanel2];
                actualPanel.SetActive(true);


        }

        if (ActualNumPanel2 >= MaxNumPanel)
        {
            GameManager.Instance.tutorialSecondLoopFinished = true;
        }

    }


}
