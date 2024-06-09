using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class MenuScript : MonoBehaviour
{


public void MainScene()
    {
        GameManager.Instance.ChangeState(GameState.Playing);
    }

    public void UgradeScene()
    {
        GameManager.Instance.ChangeState(GameState.Upgrading);
    }

    public void Update()
    {
        
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
