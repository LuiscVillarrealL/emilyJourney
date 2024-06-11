using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClawGrabberMinigame : MinigameBase
{
    public enum GameMode { ClawMode, ClickMode }
    public GameMode currentMode = GameMode.ClawMode;

    public GameObject minigameScreen;
    public RectTransform clawGrabber;
    public RectTransform clothesParent;
    public RectTransform washingMachineDrum;
    public Button grabButton;
    public float grabSpeed = 200f;
    public float moveSpeed = 100f;
    public GameObject[] clothesPrefabs;
    private List<GameObject> clothes;
    private bool isGrabbing = false;

    private bool isReturning = false;

    private Vector2 initialClawPosition;
    private bool movingRight = true;
    private bool movingDown = false;

    public Rigidbody2D clawRigidbody; // Reference to the claw's Rigidbody2D
    private BoxCollider2D clawCollider;

    private GameObject grabbedCloth;
    private bool grabbed = false;

    private void Start()
    {
        clothes = new List<GameObject>();
        PlaceClothes();

        if (currentMode == GameMode.ClawMode)
        {
            clawCollider = clawRigidbody.GetComponent<BoxCollider2D>();
        }

        
    }



    private void Update()
    {
        if (currentMode == GameMode.ClawMode)
        {
            if (!isGrabbing && !movingDown && !isReturning)
            {
                MoveClaw();
            }
            else if (movingDown)
            {
                GrabClothes();
            }
            else if (isReturning)
            {
                ReturnClaw();
            }
        }
        else if (currentMode == GameMode.ClickMode)
        {
            // Handle click mode interactions
            if (Input.GetMouseButtonDown(0))
            {
                CheckClothClick();
            }
        }
    }

    private void MoveClaw()
    {
        float moveAmount = moveSpeed * Time.deltaTime;
        Vector2 newPosition = clawRigidbody.position;

        if (movingRight)
        {
            newPosition += Vector2.right * moveAmount;

        }
        else
        {

            newPosition += Vector2.left * moveAmount;
        }

        clawRigidbody.MovePosition(newPosition);
    }

    public void ChangeMovingSide()
    {
        movingRight = !movingRight;
    }

    public void GrabbedCloth(GameObject clothGrabbed)
    {
        grabbedCloth = clothGrabbed;
        grabbed = true;
    }

    private void GrabClothes()
    {
        float moveAmount = grabSpeed * Time.deltaTime;
        Vector2 newPosition = clawRigidbody.position + Vector2.down * moveAmount;

        // Check for collision with clothes and grab them
        //foreach (var cloth in clothes)
        //{
        //    if (cloth.activeInHierarchy && clawCollider.IsTouching(cloth.GetComponent<BoxCollider2D>()))
        //    {
        //        // Simulate grabbing the cloth
        //        cloth.SetActive(false);
        //        Debug.Log("Cloth grabbed!");
        //        Return();
        //        return; // Only grab one cloth at a time
        //    }
        //}

        // Stop the claw at the bottom of the washing machine drum
        //if (newPosition.y <= washingMachineDrum.rect.yMin)
        //{
        //    newPosition.y = washingMachineDrum.rect.yMin;
        //    Return();
        //}

        clawRigidbody.MovePosition(newPosition);
    }

    public void Return()
    {
        movingDown = false;
        isGrabbing = false;
        isReturning = true;
    }

    private void ReturnClaw()
    {
        float moveAmount = grabSpeed * Time.deltaTime;
        clawRigidbody.MovePosition(Vector2.MoveTowards(clawRigidbody.position, initialClawPosition, moveAmount));

        if (grabbed)
        {
            grabbedCloth.GetComponent<Rigidbody2D>().GetComponent<Rigidbody2D>().MovePosition(Vector2.MoveTowards(grabbedCloth.GetComponent<Rigidbody2D>().position, initialClawPosition, moveAmount));
        }

        // Check if the claw has returned to its initial position
        if (Vector2.Distance(clawRigidbody.position, initialClawPosition) < 0.1f)
        {
            clawRigidbody.position = initialClawPosition;
            isReturning = false;

            if (grabbed)
            {
                grabbedCloth.SetActive(false);
                grabbed = false;
                CheckIfAllItemsCollected();
            }
        }
    }

    private void OnGrabButtonPressed()
    {
        if (!isGrabbing && !isReturning)
        {
            isGrabbing = true;
            movingDown = true;
            initialClawPosition = clawRigidbody.position; // Capture the claw's position when it starts moving down
        }
    }

    public bool IsGrabbing()
    {
        return isGrabbing;
    }


    private void CheckClothClick()
    {


        Vector2 mousePosition = Input.mousePosition;

        
        foreach (var cloth in clothesPrefabs)
        {
            RectTransform clothRect = cloth.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(clothRect, mousePosition))
            {
                // Simulate grabbing the cloth
                cloth.SetActive(false);
                Debug.Log("Cloth clicked and grabbed!");
                break; // Only grab one cloth at a time
            }
        }
    }

    private void PlaceClothes()
    {
        // Randomly place clothes inside the washing machine drum
        foreach (var cloth in clothesPrefabs)
        {
            Debug.Log($"washingMachineDrum.rect.xMin {washingMachineDrum.rect.xMin}");
            Debug.Log($"washingMachineDrum.rect.xMax {washingMachineDrum.rect.xMax}");
            Debug.Log($"washingMachineDrum.rect.yMin {washingMachineDrum.rect.yMin}");
            Debug.Log($"washingMachineDrum.rect.yMax {washingMachineDrum.rect.yMax}");

            float xPos = Random.Range(washingMachineDrum.rect.xMin, washingMachineDrum.rect.xMax);
            float yPos = Random.Range(washingMachineDrum.rect.yMin, washingMachineDrum.rect.yMax);
            var newCloth = Instantiate(cloth, new Vector3(xPos, yPos, 0), Quaternion.identity, clothesParent);
            newCloth.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);
            clothes.Add(newCloth);

            if (currentMode == GameMode.ClickMode)
            {
                newCloth.AddComponent<Button>().onClick.AddListener(() => OnItemClicked(newCloth));

            }
        }
    }

    public void OnItemClicked(GameObject item)
    {
        item.SetActive(false);
        CheckIfAllItemsCollected();
    }

    private void CheckIfAllItemsCollected()
    {
        //foreach (GameObject item in clothes)
        //{
        //    if (item.gameObject.activeSelf)
        //    {
        //        return;
        //    }
        //}
        int children = (clothesParent.GetComponentsInChildren<Transform>().GetLength(0));
        Debug.Log($"transforms in child = {children}");
        if (children - 1 <= 0)
        {
            EndMinigame();
        }
        
    }

    private void EndMinigame()  
    {
        Debug.Log($"Game ended");
        CompleteMinigame();
        // Trigger any additional end of minigame logic here
    }

}
