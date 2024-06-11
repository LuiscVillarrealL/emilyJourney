using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StairMinigame : MinigameBase
{
    public Image rampImage;
    public Image[] playerSteps; // Player images for each step on stairs
    public Image playerRamp; // Player image for ramp mode
    public Button downButton; // Button to move down
    public TMP_Text stepsCounterText; // Text to display steps counter
    public bool isStairsMode = true; // Toggle between stairs and ramp mode

    public float rampMoveSpeed = 200f; // Speed at which the player moves down the ramp
    public float alphaSpeed = 2f; // Speed of alpha transition
    public float requiredClicksPerSecond = 5f; // Required clicks per second to move down the stairs

    private int currentStep = 0;
    private bool isButtonHeld = false;
    private float initialYPosition;
    private float targetYPosition;
    private float clickCount = 0f;
    private float lastClickTime = 0f;

    public Image targetPosition;

    private void Start()
    {
        if (!isStairsMode)
        {
            initialYPosition = playerRamp.rectTransform.anchoredPosition.y;
            targetYPosition = rampImage.rectTransform.rect.height;
        }


        //downButton.onClick.AddListener(OnDownButtonPressed);
        //downButton.onClick.AddListener(OnDownButtonReleased);

       
    }

    private void Update()
    {
        if (isStairsMode)
        {
            if (isButtonHeld)
            {
                clickCount++;
                float clickSpeed = clickCount / (Time.time - lastClickTime);

                if (clickSpeed >= requiredClicksPerSecond)
                {
                    FadeInNextStep();
                    clickCount = 0;
                    lastClickTime = Time.time;
                }
            }
        }
        else
        {
            if (isButtonHeld)
            {
                MovePlayerDownRamp();
            }
        }


            
    }

    public void OnDownButtonPressed()
    {
        isButtonHeld = true;

        if (isStairsMode)
        {
            lastClickTime = Time.time;
            clickCount++;
        }
    }

    public void OnDownButtonReleased()
    {
        isButtonHeld = false;
    }

    private void FadeInNextStep()
    {
        if (currentStep < playerSteps.Length - 1)
        {
            playerSteps[currentStep].color = new Color(0, 0, 0, Mathf.Lerp(playerSteps[currentStep].color.a, 0, alphaSpeed * Time.deltaTime));
            playerSteps[currentStep + 1].color = new Color(0, 0, 0, Mathf.Lerp(playerSteps[currentStep + 1].color.a, 1, alphaSpeed * Time.deltaTime));

            if (playerSteps[currentStep].color.a <= 0.1f)
            {
                playerSteps[currentStep].color = new Color(0, 0, 0, 0);
                currentStep++;

                if (currentStep >= playerSteps.Length - 1)
                {
                    EndMinigame();
                }
            }
        }
    }

    private void MovePlayerDownRamp()
    {
        Vector2 position = playerRamp.rectTransform.anchoredPosition;
        Vector2 target = targetPosition.rectTransform.anchoredPosition;
        position = Vector2.MoveTowards(position, target, rampMoveSpeed * Time.deltaTime);
        playerRamp.rectTransform.anchoredPosition = position;

        if (position == target)
        {
            EndMinigame();
        }
    }

    private void EndMinigame()
    {
        Debug.Log("Minigame completed!");
        CompleteMinigame();
    }


}
