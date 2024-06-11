using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;

public class SimpleInteraction : BaseInteraction
{
    protected class PerformerInfo
    {
        public float ElapsedTime;
        public UnityAction<BaseInteraction> OnCompleted;
    }

    [SerializeField] protected int MaxSimultaneousUsers = 1;

    protected Dictionary<CommonAIBase, PerformerInfo> CurrentPerformers = new Dictionary<CommonAIBase, PerformerInfo>();
    public int NumCurrentUsers => CurrentPerformers.Count;

    protected List<CommonAIBase> PerformersToCleanup = new List<CommonAIBase>();

    [SerializeField] protected List<BaseInteraction> Prerequisites; // List of interactions that must be completed before this one

    public bool finishMinigame = false;

    private CommonAIBase currentPerformer;
    private UnityAction<BaseInteraction> currentOnCompleted;

    private void Start()
    {
        finishMinigame = false;
    }
    public override bool CanPerform(CommonAIBase character)
    {
        QuestManager questManager = FindObjectOfType<QuestManager>();

        if (questManager.IsEndOfDay(this) && !questManager.AreAllQuestsCompletedExceptStairs() || questManager.IsStepCompleted(this))
        {
            return false;
        }

        if (Prerequisites.Count > 0)
        {
            foreach (var prerequisite in Prerequisites)
            {
                if (character.HasCompletedInteraction(prerequisite))
                {
                    return true;
                }
            }
            return false;
        }

        // Check if this interaction is the end-of-day trigger and if all other quests are completed
        if (_InteractionType == EInteractionType.OverTime)
        {
            return NumCurrentUsers < MaxSimultaneousUsers;
        }

        return NumCurrentUsers < MaxSimultaneousUsers;
    }



    public override bool LockInteraction(CommonAIBase performer)
    {
        if (NumCurrentUsers >= MaxSimultaneousUsers)
        {
            Debug.LogError($"{performer.name} trying to lock {_DisplayName} which is already at max users");
            return false;
        }

        if (CurrentPerformers.ContainsKey(performer))
        {
            Debug.LogError($"{performer.name} tried to lock {_DisplayName} multiple times.");
            return false;
        }

        CurrentPerformers[performer] = null;

        return true;
    }

    public override bool Perform(CommonAIBase performer, UnityAction<BaseInteraction> onCompleted)
    {

        currentPerformer = performer;
        currentOnCompleted = onCompleted;

        if (!CurrentPerformers.ContainsKey(performer))
        {
            Debug.LogError($"{performer.name} is trying to perform an interaction {_DisplayName} that they have not locked");
            return false;
        }

        // check the interaction type
        if (InteractionType == EInteractionType.Instantaneous)
        {
            if (StatChanges.Length > 0)
                ApplyStatChanges(performer, 1f);

            
                OnInteractionCompleted(performer, onCompleted);
        }
        else if (InteractionType == EInteractionType.OverTime)
        {
            CurrentPerformers[performer] = new PerformerInfo() { ElapsedTime = 0, OnCompleted = onCompleted };
            StartContinuousStatUpdates(performer); // Start continuous stat updates
            StartMinigame();
        }

        return true;
    }

    protected void OnInteractionCompleted(CommonAIBase performer, UnityAction<BaseInteraction> onCompleted)
    {
        onCompleted.Invoke(this);

        if (!PerformersToCleanup.Contains(performer))
        {
            PerformersToCleanup.Add(performer);
            Debug.LogWarning($"{performer.name} did not unlock interaction in their OnCompleted handler for {_DisplayName}");
        }

        performer.MarkInteractionCompleted(this);


        foreach (var prerequisite in Prerequisites)
        {
            if (performer.HasCompletedInteraction(prerequisite))
            {
                performer.ClearInteraction(prerequisite);

                performer.GetComponent<PickInteractionAI>().ClearInteractionButtons();
            }
        }

        // Notify the QuestManager that this interaction is completed
        QuestManager questManager = FindObjectOfType<QuestManager>();
        if (questManager != null)
        {
            questManager.CompleteQuestStep(this);
        }


    }

    public override bool UnlockInteraction(CommonAIBase performer)
    {
        if (CurrentPerformers.ContainsKey(performer))
        {
            PerformersToCleanup.Add(performer);
            return true;
        }

        Debug.LogError($"{performer.name} is trying to unlock an interaction {_DisplayName} they have not locked");

        return false;
    }

    protected virtual void Update()
    {
        foreach (var kvp in CurrentPerformers)
        {
            CommonAIBase performer = kvp.Key;
            PerformerInfo performerInfo = kvp.Value;

            if (performerInfo == null)
                continue;

            float previousElapsedTime = performerInfo.ElapsedTime;
            performerInfo.ElapsedTime = Mathf.Min(performerInfo.ElapsedTime + Time.deltaTime, _Duration);

            if (StatChanges.Length > 0)
                ApplyStatChanges(performer, (performerInfo.ElapsedTime - previousElapsedTime) / _Duration);

            if (finishMinigame)
            {
                StopContinuousStatUpdates();
                OnInteractionCompleted(performer, performerInfo.OnCompleted);
            }
        }

        foreach (var performer in PerformersToCleanup)
            CurrentPerformers.Remove(performer);
        PerformersToCleanup.Clear();
    }

    private void StartMinigame()
    {
        if (minigame != null)
        {
            minigame.gameObject.SetActive(true);
            minigame.OnMinigameCompleted += CompleteMinigame;
            minigame.StartMinigame();
        }
    }

    public void CompleteMinigame()
    {
        if (minigame != null)
        {
            minigame.OnMinigameCompleted -= CompleteMinigame;
            minigame.gameObject.SetActive(false);
        }

        StopContinuousStatUpdates();
        OnInteractionCompleted(currentPerformer, currentOnCompleted);
    }

}

