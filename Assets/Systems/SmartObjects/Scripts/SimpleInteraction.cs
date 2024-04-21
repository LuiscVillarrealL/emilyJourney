using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimpleInteraction : BaseInteraction
{
    protected class PerformerInfo
    {
        public CommonAIBase PerformingAI;
        public float ElapsedTime;
        public UnityAction<BaseInteraction> OnCompleted;
    }

    [SerializeField] protected int MaxSimultaneousUsers = 1;
    protected int NumCurrentUsers = 0;
    protected List<PerformerInfo> CurrentPerformers = new List<PerformerInfo>();    

    public override bool CanPerform()
    {
        return NumCurrentUsers < MaxSimultaneousUsers;
    }

    public override void LockInteraction()
    {
        ++NumCurrentUsers;

        if (NumCurrentUsers > MaxSimultaneousUsers)
        {
            Debug.LogError($"Too many users {_DisplayName}");
            return;
        }
    }

    public override void Perform(CommonAIBase performer, UnityAction<BaseInteraction> onCompleted = null )
    {
        if (NumCurrentUsers <= 0)
        {
            Debug.LogError($"Tryng to perform interaction without users {_DisplayName}");
            return;
        }

        if (InteractionType == EInteractionType.Instantaneous)
        {

            if(StatChanges.Length > 0)
            {
                ApplyStatChanges(performer, 1f);
            }

                onCompleted.Invoke(this);
        }else if(InteractionType == EInteractionType.OverTime)
        {
            CurrentPerformers.Add(new PerformerInfo() { 
                PerformingAI = performer,
                ElapsedTime = 0, 
                OnCompleted = onCompleted});

        }
    }

    public override void UnlockInteraction()
    {
        if (NumCurrentUsers <= 0)
            Debug.LogError($"Tryng to unlock already unlocked interaction {_DisplayName}");
        --NumCurrentUsers;
    }

    protected virtual void Update()
    {
        //update any current performers
        for(int i = CurrentPerformers.Count - 1; i >= 0; i--)
        {
            PerformerInfo performer = CurrentPerformers[i];

            float previousElapsedTime = performer.ElapsedTime;
            performer.ElapsedTime = Mathf.Min(performer.ElapsedTime + Time.deltaTime, _Duration); 

            if (StatChanges.Length > 0)
            {
                ApplyStatChanges(performer.PerformingAI, (performer.ElapsedTime - previousElapsedTime) / _Duration);
            }

            //interaction compelte
            if (performer.ElapsedTime >= _Duration)
            {
                performer.OnCompleted.Invoke(this);
                CurrentPerformers.RemoveAt(i);
            }
        }
    }
}
