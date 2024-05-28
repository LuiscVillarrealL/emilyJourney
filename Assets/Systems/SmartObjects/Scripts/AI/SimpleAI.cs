using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseNavigation))]

public class SimpleAI : CommonAIBase
{
    [SerializeField] protected float PickInteractionInterval = 2f;

   
    protected float TimeUntilNextInteractionPicked = -1f;


    // Update is called once per frame
    protected override void Update()
    {

        base.Update();  
        if(CurrentInteraction == null )

        {
            TimeUntilNextInteractionPicked -= Time.deltaTime;
            if(TimeUntilNextInteractionPicked <= 0)
            {
                TimeUntilNextInteractionPicked = PickInteractionInterval;
                PickRandomInteraction();
            }
        }
    }

    void PickRandomInteraction()
    {

        int objectIndex = Random.Range(0, SmartObjectManager.Instance.RegisteredObjects.Count);
        var selectedObject = SmartObjectManager.Instance.RegisteredObjects[objectIndex];

        int interactionIndex = Random.Range(0, selectedObject.Interactions.Count);
        var selectedInteraction = selectedObject.Interactions[interactionIndex];

        if (selectedInteraction.CanPerform(this))
        {
            CurrentInteraction = selectedInteraction;
            CurrentInteraction.LockInteraction(this);
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
}
