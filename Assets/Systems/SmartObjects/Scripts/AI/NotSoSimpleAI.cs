using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(BaseNavigation))]
public class NotSoSimpleAI : CommonAIBase
{

    [SerializeField] protected float DefaultInteractionInterval = 0f;
    [SerializeField] protected float PickInteractionInterval = 2f;
    [SerializeField] protected int InteractionPickSize = 5;
    [SerializeField] protected bool AvoidInUseObjects = true;

    protected float TimeUntilNextInteractionPicked = -1f;



    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (CurrentInteraction == null)

        {
            TimeUntilNextInteractionPicked -= Time.deltaTime;

            if (TimeUntilNextInteractionPicked <= 0)
            {
                TimeUntilNextInteractionPicked = PickInteractionInterval;
                PickBestInteraction();
            }
        }

    }

    [SerializeField] protected float DefaultInteractionScore = 0f;

    float ScoreInteraction(BaseInteraction interacion)
    {
        if (interacion.StatChanges.Length == 0)
        {
            return DefaultInteractionScore;
        }

        float score = 0;

        foreach (var change in interacion.StatChanges)
        {
            score += ScoreChange(change.target, change.value);
        }

        return score;
    }

    class ScoredInteraction
    {
        public SmartObject TargetObject;
        public BaseInteraction Interaction;
        public float Score;
    }

    float ScoreChange(EStat target, float amount)
    {
        float currentValue = 0;

        switch (target)
        {
            case EStat.Energy: currentValue = CurrentEnergy; break;
            case EStat.Fun: currentValue = CurrentFun; break;
        }

        return (1f - currentValue) * amount;
    }

    void PickBestInteraction()
    {

        List<BaseInteraction> objectsInUse = null;

        HouseholdBlackboard.TryGetGeneric(EBlackboardKey.Household_ObjectsInUse, out objectsInUse, null);

        //loop through objects
        List<ScoredInteraction> unsortedInteractions = new List<ScoredInteraction>();

        foreach (var smartObject in SmartObjectManager.Instance.RegisteredObjects)
        {
            //loop through interactions
            foreach(var interaction in smartObject.Interactions)
            {
                if (!interaction.CanPerform())
                {
                    continue;
                }

                if(AvoidInUseObjects && objectsInUse != null && objectsInUse.Contains(interaction)
                {
                    continue;
                }

                float score = ScoreInteraction(interaction);

                unsortedInteractions.Add(new ScoredInteraction() { TargetObject = smartObject, Interaction = interaction, Score = score });
            }
        }

        if(unsortedInteractions.Count == 0)
        {
            return;
        }

        var sortedInteractions = unsortedInteractions.OrderByDescending(scoredInteractions => scoredInteractions.Score).ToList();

        int maxIndex = Mathf.Min(InteractionPickSize, sortedInteractions.Count);

        var selectedIndex = Random.Range(0, maxIndex);

        var selectedObject = sortedInteractions[selectedIndex].TargetObject;
        var selectedInteraction = sortedInteractions[selectedIndex].Interaction;

        CurrentInteraction = selectedInteraction;
        CurrentInteraction.LockInteraction();
        StartedPerforming = false;

        if (!Navigation.SetDestination(selectedObject.InteractionPoint))
        {
            Debug.LogError($"Could not move to {selectedObject.DisplayName}");
            CurrentInteraction = null;

        }
        else
        {
            Debug.Log($"Going to {CurrentInteraction.DisplayName} at " +
                $" {selectedObject.DisplayName}");
        }




    }

}
