using System;
using UnityEngine;
using UnityEngine.UI;

public class ToiletMinigame : MinigameBase
{

    public GameObject MinigameScreen;

    public Image firstImage;
    public Image secondImage;
    public Slider progressBar;
    public Button arrowButton;

    public float fillAmountPerClick = 0.1f; // Adjust this value to control the amount filled per click
    private float currentFillAmount = 0f;

    public bool gettingInToilet = true;
    public bool gotInToilet = false;

    private void Start()
    {
        // Initialize the progress bar to empty
        progressBar.value = 100f;
        currentFillAmount = progressBar.value;

        // Set images to fully transparent initially
        SetImageAlpha(firstImage, 100f);
        SetImageAlpha(secondImage, 0f);

    }

    public void OnArrowButtonClick()
    {


        float alphaValue = 0;

        
        if (gettingInToilet)
        {
            currentFillAmount -= fillAmountPerClick;
            
        }
        else
        {
            
            currentFillAmount += fillAmountPerClick;
            
        }
        alphaValue = Mathf.Clamp01(currentFillAmount);
        SetImageAlpha(secondImage, 1f - alphaValue); // First image fades out
        SetImageAlpha(firstImage, alphaValue); // Second image fades in

        progressBar.value = currentFillAmount;

        if (gettingInToilet && currentFillAmount <= 0f)
        {
            TurnAround();
        }


        // If progress bar is full, complete the minigame
        if (!gettingInToilet && gotInToilet && currentFillAmount >= 1f)
        {
            CompleteMinigame();
        }
    }

    private void TurnAround()
    {
        gotInToilet = true;
        gettingInToilet = false;
        arrowButton.transform.rotation = new Quaternion(0, 0, 0 ,0);

    }

    private void SetImageAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }

}
