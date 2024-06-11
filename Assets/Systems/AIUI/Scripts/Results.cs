using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Results : MonoBehaviour
{

    public TextMeshProUGUI timerText;
    public float endTime;
    public float stress;

    public MenuScript menuScript;

    public ResultsSOScript LateVeryStressed;
    public ResultsSOScript LateKindaStressed;
    public ResultsSOScript LateNotStressed;

    public ResultsSOScript OnTimeVeryStressed;
    public ResultsSOScript OnTimeKindaStressed;
    public ResultsSOScript OnTimeNotStressed;


    // Start is called before the first frame update
    void Start()
    {
        endTime = GameManager.Instance.LastTime;
        stress = GameManager.Instance.ActualStress;


        if (endTime <= 0)
        {
            if (stress >= .6)
            {
                timerText.text = LateVeryStressed.ResultText;
            }
            else if ((stress > 0) && (stress < .6)){
                timerText.text = LateKindaStressed.ResultText;
            }
            else
            {
                timerText.text = LateNotStressed.ResultText;
            }
        }
        else
        {
            if (stress >= .6)
            {
                timerText.text = OnTimeVeryStressed.ResultText;
            }
            else if ((stress > 0) && (stress < .6))
            {
                timerText.text = OnTimeKindaStressed.ResultText;
            }
            else
            {
                timerText.text = OnTimeNotStressed.ResultText;
            }
        }
    }

    public void ContinueGame() {

        if (endTime > 0 && stress <= 0)
        {
            menuScript.GameOver();
        }
        else
        {
            menuScript.UgradeScene();
        }
    
    
    }


}
