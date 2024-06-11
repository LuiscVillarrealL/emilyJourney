using System;
using UnityEngine;
using UnityEngine.UI;

public class WashingDishMinigame : MinigameBase
{

    public GameObject minigamePanel;
    public RectTransform counter;
    public Slider energyBar;
    public float scrollSpeed = 1.0f;
    public float energyDepletionRate = 0.5f;
    public float energyRecoverRate = 0.5f;
    public Button ScrollButton;
    public bool isScrolling = false;
    private float maxYPosition;

    public Image dirtyDishImage; // The dirty dish image with alpha blending
    public Image cleanDishImage; // The clean dish image
    public float scrubbingThreshold = 1000f; // The amount of scrubbing needed to clean the dish

    private Vector2 lastMousePosition;
    private float scrubbingAmount;



    private void Start()
    {
        dirtyDishImage.color = new Color(1, 1, 1, 1); // Start fully opaque
        scrubbingAmount = 0;

        maxYPosition = counter.anchoredPosition.y;
        ScrollButton.onClick.AddListener(ScrollButtonPressed);
    }

    private void Update()
    {

        if (isScrolling && energyBar.value > 0)
        {
            ScrollUp();

        }
        else
        {
            ScrollDown();
        }

        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector2 currentMousePosition = Input.mousePosition;
            float distance = Vector2.Distance(currentMousePosition, lastMousePosition);

            if (distance > 0)
            {
                scrubbingAmount += distance;
                lastMousePosition = currentMousePosition;
                UpdateScrubbingEffect();
            }
        }
    }

    public void ScrollButtonPressed()
    {
        isScrolling = true;
    }


    private void ScrollDown()
    {
        isScrolling = false;
        energyBar.value += energyRecoverRate * Time.deltaTime;
        counter.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
        if (counter.anchoredPosition.y >= maxYPosition)
        {
            counter.anchoredPosition = new Vector2(counter.anchoredPosition.x, maxYPosition);
        }
    }

    private void ScrollUp()
    {

        energyBar.value -= energyDepletionRate * Time.deltaTime;
        counter.anchoredPosition -= Vector2.up * scrollSpeed * Time.deltaTime;
        if (counter.anchoredPosition.y <= maxYPosition * -1)
        {
            counter.anchoredPosition = new Vector2(counter.anchoredPosition.x, maxYPosition * -1);
        }
    }

    private void UpdateScrubbingEffect()
    {
        float progress = Mathf.Clamp01(scrubbingAmount / scrubbingThreshold);
        dirtyDishImage.color = new Color(1, 1, 1, 1 - progress);

        if (progress >= 1f)
        {
            
            // Add logic for what happens when the dish is fully cleaned

            EngMinigame();
        }
    }

    private void EngMinigame()
    {
        Debug.Log("Dish cleaned!");
        CompleteMinigame();
    }
}
