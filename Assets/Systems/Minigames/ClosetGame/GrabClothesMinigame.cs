using System;
using UnityEngine;
using UnityEngine.UI;

public class GrabClothesMinigame : MinigameBase
{
    public GameObject minigamePanel;
    public RectTransform grabberArm; // Reference to the grabber arm RectTransform
    public RectTransform[] clothes; // References to the clothes RectTransforms
    public Button upButton; // Reference to the up button
    public Button grabButton; // Reference to the grab button
    public Slider energyBar; // Reference to the energy bar slider

    public RectTransform closet;

    public float moveSpeed = 200f; // Speed at which the grabber arm moves
    public float energyDepletionRate = 0.5f; // Rate at which energy depletes
    public float energyRecoverRate = 0.3f; // Rate at which energy recovers

    [SerializeField]
    private bool isMovingUp = false;
    private bool isGrabbing = false;
    private float minYPosition;
    private float maxYPosition;
    private int grabCount = 0;
    public int maxGrabCount = 5; // Number of times a cloth needs to be clicked to be grabbed
    private int clothNum = 0;
    private int clothMaxNum = 0;

    public Rigidbody2D clawRigidbody;
    private Vector2 initialClawPosition;

    private void Start()
    {
        minYPosition = grabberArm.anchoredPosition.y;
        maxYPosition = minYPosition * -1; // Adjust as needed

        clothMaxNum = clothes.Length;

        //upButton.onClick.AddListener(OnUpButtonPressed);
        //upButton.onClick.AddListener(OnUpButtonReleased);
        grabButton.onClick.AddListener(OnGrabButtonPressed);
    }

    private void Update()
    {
        if (isMovingUp && energyBar.value > 0)
        {
            ScrollUp();
        }
        else
        {
            isMovingUp = false;
            ScrollDown();
        }
    }

    public void OnUpButtonPressed()
    {
        isMovingUp = true;
    }


    private void ScrollDown()
    {


        energyBar.value += energyRecoverRate * Time.deltaTime;
        // Check if the claw has returned to its initial position
        if (grabberArm.anchoredPosition.y > minYPosition)
        {
            grabberArm.anchoredPosition += Vector2.down * moveSpeed * Time.deltaTime;

        }
    }

    private void ScrollUp()
    {

        energyBar.value -= energyDepletionRate * Time.deltaTime;
        if (grabberArm.anchoredPosition.y <= maxYPosition)
        {
            
            grabberArm.anchoredPosition += Vector2.up * moveSpeed * Time.deltaTime;
        }
    }



    private void OnGrabButtonPressed()
    {
        if (isGrabbing) return;
        isGrabbing = true;

        foreach (var cloth in clothes)
        {
            if (IsOverlapping(grabberArm, cloth))
            {
                Debug.Log("grabbed!");
                grabCount++;
                if (grabCount >= maxGrabCount)
                {
                    clothes[clothMaxNum - clothNum - 1].gameObject.SetActive(false);
                    clothNum++;
                    grabCount = 0;
                    CheckAllClothesGrabbed();
                }
                break;
            }
        }

        isGrabbing = false;
    }

    private bool IsOverlapping(RectTransform rect1, RectTransform rect2)
    {
        Rect rect1World = new Rect(rect1.position.x, rect1.position.y, rect1.rect.width, rect1.rect.height);
        Rect rect2World = new Rect(rect2.position.x, rect2.position.y, rect2.rect.width, rect2.rect.height);

        Debug.Log($"rect1World {rect1World}");
        Debug.Log($"rect2World {rect2World}");


        return rect1World.Overlaps(rect2World);
    }

    private void CheckAllClothesGrabbed()
    {
        foreach (var cloth in clothes)
        {
            if (cloth.gameObject.activeSelf)
            {
                return;
            }
        }
        EndMinigame();
    }

    private void EndMinigame()
    {
        Debug.Log("Minigame Completed!");
        CompleteMinigame();
    }
}
