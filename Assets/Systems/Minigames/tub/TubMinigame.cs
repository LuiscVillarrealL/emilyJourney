using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TubMinigame : MonoBehaviour
{
    public GameObject minigamePanel;
    public RectTransform balanceIndicator; // Reference to the balance indicator RectTransform
    public RectTransform horizontalBar; // Reference to the horizontal bar RectTransform
    public RectTransform targetZone; // Reference to the target zone RectTransform
    public Button leftButton; // Reference to the left button
    public Button rightButton; // Reference to the right button
    public Slider progressBar; // Reference to the progress bar

    public float forceAmount = 200f; // Amount of force applied when pressing buttons
    public float damping = 1f; // Damping factor to reduce the force over time
    public float progressSpeed = 0.01f; // Speed at which the progress bar fills
    public float maxIndicatorDistance = 200f; // Maximum distance the indicator can move from the center
    public float randomForceInterval = 1f; // Interval at which random forces are applied
    public float randomForceAmount = 50f; // Magnitude of the random force

    private float currentForce = 0f; // Current force applied to the balance indicator
    private float randomForceTimer = 0f; // Timer to track random force application

    public Image firstImage;
    public Image secondImage;

    public bool gotInTub = false;

    private float alphaValue = 0;

    private void Start()
    {
        // Add EventTrigger to handle button presses
        AddEventTrigger(leftButton.gameObject, EventTriggerType.PointerDown, (data) => { ApplyForce(-forceAmount); });
        AddEventTrigger(rightButton.gameObject, EventTriggerType.PointerDown, (data) => { ApplyForce(forceAmount); });

        progressBar.value = 0f;

        SetImageAlpha(firstImage, 100f);
        SetImageAlpha(secondImage, 0f);
    }

    private void Update()
    {
        // Apply random force at intervals
        randomForceTimer += Time.deltaTime;
        if (randomForceTimer >= randomForceInterval)
        {
            ApplyRandomForce();
            randomForceTimer = 0f;
        }

        // Apply damping to the current force
        currentForce = Mathf.Lerp(currentForce, 0, damping * Time.deltaTime);

        // Move the balance indicator based on the current force
        balanceIndicator.anchoredPosition += new Vector2(currentForce * Time.deltaTime, 0);

        // Clamp the balance indicator position within the horizontal bar
        float clampedX = Mathf.Clamp(balanceIndicator.anchoredPosition.x, -maxIndicatorDistance, maxIndicatorDistance);
        balanceIndicator.anchoredPosition = new Vector2(clampedX, balanceIndicator.anchoredPosition.y);

        // Check if balance indicator is within the target zone
        if (IsIndicatorInTargetZone())
        {
            if (!gotInTub)
            {
                progressBar.value += progressSpeed * Time.deltaTime;
            }
            else
            {
                progressBar.value -= progressSpeed * Time.deltaTime;
            }
            
           
        }
        else
        {
            if (!gotInTub)
            {
                progressBar.value -= progressSpeed * Time.deltaTime * 0.5f;
            }
            else
            {
                progressBar.value += progressSpeed * Time.deltaTime * 0.5f;
            }

             // Penalize for imbalance
        }

        alphaValue = Mathf.Clamp01(progressBar.value);
        SetImageAlpha(firstImage, 1f - alphaValue); // First image fades out
        SetImageAlpha(secondImage, alphaValue); // Second image fades in

        // Check if the minigame is complete
        if (progressBar.value >= progressBar.maxValue && !gotInTub)
        {
            gotInTub = true;
        }

        if (progressBar.value <= 0 && gotInTub)
        {
            CompleteMinigame();
        }


    }

    private void ApplyForce(float force)
    {
        currentForce += force;
    }

    private void ApplyRandomForce()
    {
        float randomForce = Random.Range(-randomForceAmount, randomForceAmount);
        currentForce += randomForce;
    }

    private void CompleteMinigame()
    {
        Debug.Log("Minigame Complete!");
        // Add actions to handle the minigame completion, such as progressing the game state or giving rewards
        minigamePanel.SetActive(false);
    }

    private void AddEventTrigger(GameObject target, EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = target.GetComponent<EventTrigger>() ?? target.AddComponent<EventTrigger>();
        var entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    private bool IsIndicatorInTargetZone()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(targetZone, balanceIndicator.position, null);
    }

    private void SetImageAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
