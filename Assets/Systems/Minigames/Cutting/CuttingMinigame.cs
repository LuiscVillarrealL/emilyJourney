using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[Serializable]
public class Foods
{
    public Sprite[] FoodSprites;
}

public class CuttingMinigame : MonoBehaviour
{

    public GameObject minigamePanel;
    public RectTransform counter;
    public Slider energyBar;
    public float scrollSpeed = 1.0f;
    public float cutSpeed = 1.0f;
    public float energyDepletionRate = 0.5f;
    public float energyRecoverRate = 0.5f;
    public GameObject cutButton;
    public Button ScrollButton;
    public bool isScrolling = false;
    private float maxYPosition;

    public Image[] CutFood;
    public Image[] CompleteImages;

    public Foods[] FoodSprites;

    public bool IsCutting = false;
    public bool cutInProgress = false;

    [SerializeField]
    private int foodNum = 0;
    [SerializeField]
    private int actualFood = 0;
    [SerializeField]
    private int foodSliceNum = 0;


    private void Start()
    {

        foodNum = FoodSprites.Length;

        StartNewFood();

        maxYPosition = counter.anchoredPosition.y;
        ScrollButton.onClick.AddListener(ScrollButtonPressed);
    }

    private void StartNewFood()
    {

       

        for (int i = 0; i < FoodSprites[actualFood].FoodSprites.Length; i++)
        {
            CompleteImages[i].sprite = FoodSprites[actualFood].FoodSprites[i];
            CutFood[i].sprite = FoodSprites[actualFood].FoodSprites[i];
            CompleteImages[i].gameObject.SetActive(true);
            CutFood[i].gameObject.SetActive(false);
        }
    }

    private void EndGame()
    {
        Debug.Log("Game Ended");
        minigamePanel.SetActive(false);
    }

    private void DoCut()
    {
        Debug.Log("Cut performed!");


        if (foodSliceNum <= CompleteImages.Length)
        {
            CutFood[foodSliceNum].sprite = FoodSprites[actualFood].FoodSprites[foodSliceNum];
            CompleteImages[foodSliceNum].gameObject.SetActive(false);
            CutFood[foodSliceNum].gameObject.SetActive(true);
            foodSliceNum++;

            if (foodSliceNum == CompleteImages.Length)
            {
                foodSliceNum = 0;
                actualFood++;

                if (actualFood >= FoodSprites.Length)
                {
                    EndGame();
                    return;
                }

                StartNewFood();
            }
        }
        
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

        if (IsCutting)
        {
            CutItem();
        }
        else
        {
            NotCutItem();
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



    public void CutButtonBeingPressed()
    {
        Debug.Log($"Cutting");

            IsCutting = true;


    }

    public void CutButtonBeingReleased()
    {
        Debug.Log($"Not Cutting");
        IsCutting = false;
    }


    private void CutItem()
    {
        if (cutButton.transform.rotation.z * 100 < 90f)
        {
            cutButton.transform.Rotate(0, 0, cutSpeed * Time.deltaTime);
        }
        else if (!cutInProgress)
        {
            cutInProgress = true;
            DoCut();
        }
    }

    private void NotCutItem()
    {
        if (cutButton.transform.rotation.z > 0)
        {
            cutButton.transform.Rotate(0, 0, -cutSpeed * Time.deltaTime);
        }
        else if (cutInProgress)
        {
            cutButton.transform.rotation = Quaternion.identity;
            cutInProgress = false;
        }
    }


}
