using System;
using UnityEngine;
using UnityEngine.UI;

public class StoveMinigame : MonoBehaviour
{
    public GameObject minigamePanel;
    public RectTransform panObjects;
    public RectTransform player; // Reference to the star RectTransform
    public RectTransform bar; // Reference to the vertical bar RectTransform
    public Button balanceButton; // Reference to the balance Button

    public float moveSpeed = 200f; // Speed at which the star moves
    public float gravitySpeed = 100f; // Speed at which the star falls when not clicking

    private bool isBalancing = false;

    public RectTransform stoveImage;
    //public float scrollSpeedUp = 200f;
    //public float scrollSpeedDown = 100f;
    private float maxYPosition;

    public Button leftButton; // Reference to the left button
    public Button rightButton; // Reference to the right button
    public bool isLeftButtonLastPressed = false;
    public float buttonChangeInterval = 0.5f; // Interval to change button colors


    public int shakes = 0;
    public int neededShakes = 6;
    public float panMoveAmount = 10f;

    public HeightLevel currentHeightLevel = HeightLevel.Middle; // Initial height level

    [Serializable]
    public enum HeightLevel
    {
        Bottom,
        LowerMiddle,
        Middle
    }


    private void Start()
    {
        maxYPosition = stoveImage.anchoredPosition.y;
        // Add listeners to the button
        balanceButton.onClick.AddListener(OnBalanceButtonPressed);

        leftButton.onClick.AddListener(OnLeftButtonPressed);
        rightButton.onClick.AddListener(OnRightButtonPressed);
    }

    private void Update()
    {

        if (shakes >= neededShakes)
        {
            EndMinigame();
        }


        if (isBalancing)
        {
            MoveStarUp();
            ScrollUp();
            
            
        }
        else
        {
            MoveStarDown();
            ScrollDown();
        }

        // Keep the star within the bounds of the vertical bar
        ClampStarPosition();

        // Change button colors periodically
        //buttonChangeTimer += Time.deltaTime;
        //if (buttonChangeTimer >= buttonChangeInterval)
        //{
        //    buttonChangeTimer = 0f;
        //    ChangeButtonColors();
        //}

        ChangeButtonColors();
    }

    private void EndMinigame()
    {
        Debug.Log($"Minigame ended");
        minigamePanel.SetActive( false );
    }

    private void OnLeftButtonPressed()
    {
        if (!isLeftButtonLastPressed)
        {
            isLeftButtonLastPressed = true;
            MovePanLeft();
            shakes++;
        }

    }

    private void OnRightButtonPressed()
    {
        if (isLeftButtonLastPressed)
        {
            isLeftButtonLastPressed = false;
            MovePanRight();
            shakes++;
        }

    }

    private void MovePanLeft()
    {
        Vector2 newPosition = panObjects.anchoredPosition;
        newPosition.x -= panMoveAmount;
        panObjects.anchoredPosition = newPosition;
    }

    private void MovePanRight()
    {
        Vector2 newPosition = panObjects.anchoredPosition;
        newPosition.x += panMoveAmount;
        panObjects.anchoredPosition = newPosition;
    }

    private void ScrollDown()
    {
        stoveImage.anchoredPosition += Vector2.up * gravitySpeed * Time.deltaTime;
        if (stoveImage.anchoredPosition.y >= maxYPosition)
        {
            stoveImage.anchoredPosition = new Vector2(stoveImage.anchoredPosition.x, maxYPosition);
        }
    }

    private void ScrollUp()
    {
        stoveImage.anchoredPosition -= Vector2.up * moveSpeed * Time.deltaTime;
        if (stoveImage.anchoredPosition.y <= maxYPosition * -1)
        {
            stoveImage.anchoredPosition = new Vector2(stoveImage.anchoredPosition.x, maxYPosition * -1);
        }
    }

    private void OnBalanceButtonPressed()
    {
        isBalancing = true;

        Invoke("ResetBalancing", 0.5f); // Short delay to simulate button hold
    }

    private void ResetBalancing()
    {
        isBalancing = false;
    }

    private void MoveStarUp()
    {
        player.anchoredPosition += Vector2.up * moveSpeed * Time.deltaTime;
    }

    private void MoveStarDown()
    {
        player.anchoredPosition -= Vector2.up * gravitySpeed * Time.deltaTime;
    }

    private void ChangeButtonColors()
    {
        if (!isLeftButtonLastPressed)
        {
            leftButton.GetComponent<Image>().color = Color.yellow;
            rightButton.GetComponent<Image>().color = Color.black;
        }
        else
        {
            leftButton.GetComponent<Image>().color = Color.black;
            rightButton.GetComponent<Image>().color = Color.yellow;
        }

    }

    private void ClampStarPosition()
    {
        float minY = GetMinYForCurrentHeightLevel();
        float maxY = bar.rect.yMax - player.rect.height / 2;

        Vector2 clampedPosition = player.anchoredPosition;
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);
        player.anchoredPosition = clampedPosition;
    }

    private float GetMinYForCurrentHeightLevel()
    {
        switch (currentHeightLevel)
        {
            case HeightLevel.Bottom:
                return bar.rect.yMin + player.rect.height / 2;
            case HeightLevel.LowerMiddle:
                return bar.rect.yMin + bar.rect.height / 3.5f;
            case HeightLevel.Middle:
                return bar.rect.yMin + bar.rect.height / 2;
            default:
                return bar.rect.yMin + bar.rect.height / 3;
        }
    }

    public void SetHeightLevel(int level)
    {
        currentHeightLevel = (HeightLevel)level;
    }


}
