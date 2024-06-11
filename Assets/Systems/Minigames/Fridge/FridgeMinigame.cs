using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FridgeMinigame : MinigameBase
{
    public GameObject fridgePanel;
    public RectTransform fridgeContent;
    public Button arrowButton;
    public float scrollSpeed = 100f;
    public float downScrollSpeed = 100f;
    [SerializeField]
    private bool isScrollingUp = false;
    private bool pressed = false;
    private float maxYPosition;


    void Start()
    {       
        //arrowButton.onClick.AddListener(OnArrowButtonClicked);
        maxYPosition = fridgeContent.anchoredPosition.y;

       // arrowButton.onClick.AddListener(() => isScrollingUp = true);
    }

    void Update()
    {
        if (isScrollingUp)
        {
            ScrollUp();
        }
        else
        {
            ScrollDown();
        }
    }

    public void ButtonBeingPressed()
    {
        Debug.Log($"ButtonBeingPressed");
        if (!pressed)
        {
            isScrollingUp = true;
            pressed = true;
        }
        
    }

    public void ButtonBeingReleased()
    {
        Debug.Log($"ButtonBeingReleased");
        isScrollingUp = false;
        pressed = false;
    }


    private void ScrollDown()
    {
        fridgeContent.anchoredPosition += Vector2.up * downScrollSpeed * Time.deltaTime;
        if (fridgeContent.anchoredPosition.y >= maxYPosition)
        {
            fridgeContent.anchoredPosition = new Vector2(fridgeContent.anchoredPosition.x, maxYPosition);
        }
    }

    private void ScrollUp()
    {
        fridgeContent.anchoredPosition -= Vector2.up * scrollSpeed * Time.deltaTime;
        if (fridgeContent.anchoredPosition.y <= maxYPosition * -1)
        {
            fridgeContent.anchoredPosition = new Vector2(fridgeContent.anchoredPosition.x, maxYPosition * -1);
        }
    }

    public void OnItemClicked(GameObject item)
    {
        item.SetActive(false);
        CheckIfAllItemsCollected();
    }

    private void CheckIfAllItemsCollected()
    {
        foreach (Transform item in fridgeContent)
        {
            if (item.gameObject.activeSelf)
            {
                return;
            }
        }
        EndMinigame();
    }

    private void EndMinigame()
    {
        Debug.Log($"Game ended");
        CompleteMinigame();
        //fridgePanel.SetActive(false);
        // Trigger any additional end of minigame logic here
    }
}
