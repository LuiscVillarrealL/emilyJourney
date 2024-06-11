using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private List<Quest> dailyQuests;
    private Dictionary<string, bool> interactionCompletionStatus;

    public AIStat StressStat;

    [Header("UI Elements")]
    public GameObject questPanel;
    public GameObject questItemPrefab;

    private void Awake()
    {
        interactionCompletionStatus = new Dictionary<string, bool>();
        InitializeQuests();
        PopulateQuestPanel();
    }

    private void InitializeQuests()
    {
        foreach (var quest in dailyQuests)
        {
            foreach (var step in quest.Steps)
            {
                interactionCompletionStatus[step.StepName] = false;
            }
        }
    }
    public void PopulateQuestPanel()
    {
        foreach (Transform child in questPanel.transform)
        {
            Debug.Log($"child {child}");
            Destroy(child.gameObject); // Clear existing items
        }
        GameObject QuestList = Instantiate(questItemPrefab, questPanel.transform);

        QuestList.GetComponentInChildren<TextMeshProUGUI>().text = "Objectives of the day:";
        QuestList.GetComponentInChildren<TextMeshProUGUI>().color = Color.yellow;

        foreach (var quest in dailyQuests)
        {

            if (!IsQuestCompleted(quest))
            {
                GameObject questItem = Instantiate(questItemPrefab, questPanel.transform);

                questItem.GetComponentInChildren<TextMeshProUGUI>().text = quest.QuestName;
                questItem.GetComponentInChildren<TextMeshProUGUI>().color = Color.blue;

                GameObject stepTitelItem = Instantiate(questItemPrefab, questPanel.transform);
                stepTitelItem.GetComponentInChildren<TextMeshProUGUI>().text = "Steps: ";
                stepTitelItem.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                foreach (var step in quest.Steps)
                {

                    if (!step.IsCompleted)
                    {
                        GameObject stepItem = Instantiate(questItemPrefab, questPanel.transform);

                        stepItem.GetComponentInChildren<TextMeshProUGUI>().text = step.StepName;
                    }



                }
                GameObject Space = Instantiate(questItemPrefab, questPanel.transform);
                Space.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }


        }
    }


    public List<Quest> GetQuests()
    {
        return dailyQuests;
    }

    public void CompleteQuestStep(BaseInteraction interaction)
    {
        interactionCompletionStatus[interaction.InteractionID] = true;

        foreach (var quest in dailyQuests)
        {
            foreach (var step in quest.Steps)
            {
                if (!step.IsCompleted && step.RequiredInteractions.Contains(interaction))
                {
                    step.IsCompleted = true;
                    CheckAllQuestsCompleted(step.IsEndOfDayTrigger);
                    SaveQuestState();
                    PopulateQuestPanel(); // Update the quest panel
                    return;
                }
            }
        }
    }

    public bool IsEndOfDay(BaseInteraction interaction)
    {
        foreach (var quest in dailyQuests)
        {
            foreach (var step in quest.Steps)
            {
                if (!step.IsCompleted && step.RequiredInteractions.Contains(interaction))
                {
                    return step.IsEndOfDayTrigger;
                }
            }
        }

        return false;
    }

    public bool AreAllQuestsCompleted()
    {
        foreach (var quest in dailyQuests)
        {
            foreach (var step in quest.Steps)
            {
                if (!step.IsCompleted)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public bool IsQuestCompleted(Quest quest)
    {

            foreach (var step in quest.Steps)
            {
                if (!step.IsCompleted)
                {
                    return false;
                }
            
        }
        return true;
    }

    public bool IsStepCompleted(BaseInteraction interaction)
    {
        foreach (var quest in dailyQuests)
        {
            foreach (var step in quest.Steps)
            {
                if (step.RequiredInteractions.Contains(interaction) && step.IsCompleted )
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool AreAllQuestsCompletedExceptStairs()
    {
        foreach (var quest in dailyQuests)
        {
            foreach (var step in quest.Steps)
            {
                if (!step.IsEndOfDayTrigger && !step.IsCompleted )
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void CheckAllQuestsCompleted(bool isEndOfDayTrigger)
    {
        if (AreAllQuestsCompleted() && isEndOfDayTrigger)
        {
            EndDay();
        }
    }

    private void EndDay()
    {

        CommonAIBase AIPlayer = FindObjectOfType<CommonAIBase>();

        GameManager.Instance.ActualStress = AIPlayer.IndividualBlackboard.GetStat(StressStat);

        GameManager.Instance.ChangeState(GameState.Result);
        Debug.Log("All quests completed. The day is over.");
    }

    private void SaveQuestState()
    {
        foreach (var quest in dailyQuests)
        {
            foreach (var step in quest.Steps)
            {
                foreach (var interaction in step.RequiredInteractions)
                {
                    if (interactionCompletionStatus.ContainsKey(interaction.InteractionID))
                    {
                        PlayerPrefs.SetInt(interaction.InteractionID, interactionCompletionStatus[interaction.InteractionID] ? 1 : 0);
                    }
                }
            }
        }
        PlayerPrefs.Save();
    }

    private void LoadQuestState()
    {
        foreach (var quest in dailyQuests)
        {
            foreach (var step in quest.Steps)
            {
                foreach (var interaction in step.RequiredInteractions)
                {
                    if (PlayerPrefs.HasKey(interaction.InteractionID))
                    {
                        interactionCompletionStatus[interaction.InteractionID] = PlayerPrefs.GetInt(interaction.InteractionID) == 1;
                        step.IsCompleted = interactionCompletionStatus[interaction.InteractionID];
                    }
                }
            }
        }
    }
}

[System.Serializable]
public class Quest
{
    public string QuestName;
    public string QuestDescription;
    public List<QuestStep> Steps;
    public bool IsCompleted;
}

[System.Serializable]
public class QuestStep
{
    public string StepName;
    public string StepDescription;
    public List<BaseInteraction> RequiredInteractions;
    public bool IsCompleted;
    public bool IsEndOfDayTrigger;
}
