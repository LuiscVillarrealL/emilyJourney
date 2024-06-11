using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class MenuScript : MonoBehaviour
{


public void ToPreGameScene()
    {
        GameManager.Instance.ChangeState(GameState.Pregame);
    }

    public void MainScene()
    {
        GameManager.Instance.ChangeState(GameState.Playing);
    }

    public void GameOver()
    {
        GameManager.Instance.ChangeState(GameState.GameOver);
    }

    public void UgradeScene()
    {
        GameManager.Instance.ChangeState(GameState.Upgrading);
    }

    public void ResultsScene()
    {
        GameManager.Instance.ChangeState(GameState.Result);
    }

    public void Update()
    {
        
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
