using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FeedbackUIPanel : MonoBehaviour
{
    public GameObject interactionButtonPrefab;
    public Transform interactionButtonParent;

    public PickInteractionAI interactionAI;

    public GameObject StatPanelPrefab;
    public Transform StatRoot;

    public AIStatPanel AddStat(AIStat linkedStat, float initialValue)
    {
        var newGO = Instantiate(StatPanelPrefab, StatRoot);
        newGO.name = $"Stat_{linkedStat.DisplayName}";
        var statPanelLogic = newGO.GetComponent<AIStatPanel>();
        statPanelLogic.Bind(linkedStat, initialValue);

        return statPanelLogic;
    }

    public void RegenerateButtons()
    {
        interactionAI.GenerateInteractionButtons();
    }


}
