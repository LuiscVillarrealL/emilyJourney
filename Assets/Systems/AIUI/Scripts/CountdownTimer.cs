using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    public float totalTime = 90; //Set the total time for the countdown
    public TextMeshProUGUI timerText;

    void Update()
    {
        if (totalTime > 0)
        {
            // Subtract elapsed time every frame
            totalTime -= Time.deltaTime;

            // Divide the time by 60
            float minutes = Mathf.FloorToInt(totalTime / 60);

            // Returns the remainder
            float seconds = Mathf.FloorToInt(totalTime % 60);

            // Set the text string
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        }
        else
        {
            timerText.text = "Time's up";
            totalTime = 0;
        }
    }
}