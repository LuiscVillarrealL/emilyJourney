using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private List<Quest> dailyQuests;
    private Dictionary<string, bool> interactionCompletionStatus;

    private void Awake()
    {
        interactionCompletionStatus = new Dictionary<string, bool>();
        InitializeQuests();
    }

    private void InitializeQuests()
    {
        //dailyQuests = new List<Quest>();
        //{
        //    new Quest
        //    {
        //        QuestName = "Cook",
        //        QuestDescription = "Cook your food before going to work",
        //        Steps = new List<QuestStep>
        //        {
        //            new QuestStep { StepName = "Take ingredients", StepDescription = "Go to the fridge and get the ingredients", RequiredInteractions = new List<BaseInteraction>(), IsCompleted = false, IsEndOfDayTrigger = false },
        //            new QuestStep { StepName = "Prepare the ingredients", StepDescription = "Go to the cutting board and prepare the ingredients", RequiredInteractions = new List<BaseInteraction>(), IsCompleted = false, IsEndOfDayTrigger = false },
        //            new QuestStep { StepName = "Cook the ingredients", StepDescription = "Go to the stove and cook the ingredients", RequiredInteractions = new List<BaseInteraction>(), IsCompleted = false, IsEndOfDayTrigger = false },

        //        }
        //    },
        //    new Quest
        //    {
        //        QuestName = "Do the laundry",
        //        QuestDescription = "Go to the washing mashine and put the clothes in it",
        //        Steps = new List<QuestStep>
        //        {
        //            new QuestStep { StepName = "Prepare ingredients", StepDescription = "Gather all necessary ingredients", RequiredInteractions = new List<BaseInteraction>(), IsCompleted = false, IsEndOfDayTrigger = false }
                    
        //        }
        //    },

        //       new Quest
        //    {
        //        QuestName = "Go to work",
        //        QuestDescription = "Leave the house",
        //        Steps = new List<QuestStep>
        //        {
        //            new QuestStep { StepName = "Prepare ingredients", StepDescription = "Gather all necessary ingredients", RequiredInteractions = new List<BaseInteraction>(), IsCompleted = false, IsEndOfDayTrigger = true }

        //        }
        //    },
        //};

       // LoadQuestState();
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
                    return;
                }
            }
        }
    }

    private void CheckAllQuestsCompleted(bool isEndOfDayTrigger)
    {
        foreach (var quest in dailyQuests)
        {
            foreach (var step in quest.Steps)
            {
                if (!step.IsCompleted)
                {
                    return;
                }
            }
        }

        if (isEndOfDayTrigger)
        {
            EndDay();
        }
    }

    private void EndDay()
    {

        GameManager.Instance.ChangeState(GameState.Upgrading);
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
