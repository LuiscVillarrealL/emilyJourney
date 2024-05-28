
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public enum EInteractionType
{
    Instantaneous = 0,
    OverTime = 1
}

[System.Serializable]
public class InteractionStatChange
{
    public AIStat LinkedStat;
    public float Value;
}

public abstract class BaseInteraction : MonoBehaviour
{
    [SerializeField] protected string _DisplayName;
    [SerializeField] protected EInteractionType _InteractionType = EInteractionType.Instantaneous;
    [SerializeField] protected float _Duration = 0f;
    [SerializeField, FormerlySerializedAs("StatChanges")] protected InteractionStatChange[] _StatChanges;

    

    public string DisplayName => _DisplayName;
    public EInteractionType InteractionType => _InteractionType;
    public float Duration => _Duration;
    public InteractionStatChange[] StatChanges => _StatChanges;

    public abstract bool CanPerform(CommonAIBase character);
    public abstract bool LockInteraction(CommonAIBase performer);
    public abstract bool Perform(CommonAIBase performer, UnityAction<BaseInteraction> onCompleted);
    public abstract bool UnlockInteraction(CommonAIBase performer);

    public void ApplyStatChanges(CommonAIBase performer, float proportion)
    {
        foreach (var statChange in StatChanges)
        {
            float changedValue = statChange.Value * proportion;

            if(statChange.LinkedStat.ConnectedStat != null && statChange.LinkedStat.ConnectedStat.ConnectedStat == statChange.LinkedStat)
            {
                Debug.Log($"{statChange.LinkedStat.ConnectedStat} = {performer.GetStatValue(statChange.LinkedStat.ConnectedStat)}");
                changedValue = changedValue + performer.GetStatValue(statChange.LinkedStat.ConnectedStat) * .005f;
            }

            performer.UpdateIndividualStat(statChange.LinkedStat, changedValue, Trait.ETargetType.Impact);

            UpdateConnectedStats(performer, proportion, statChange);
        }
    }

    private static void UpdateConnectedStats(CommonAIBase performer, float proportion, InteractionStatChange statChange)
    {
        // Check if the AIStat has a connected stat
        if (statChange.LinkedStat.ConnectedStat != null)
        {
            // Calculate the change for the connected stat based on the connected change rate
            float connectedChange = statChange.Value * proportion * statChange.LinkedStat.ConnectedStatChangeRate;
            performer.UpdateIndividualStat(statChange.LinkedStat.ConnectedStat, connectedChange, Trait.ETargetType.Impact);
        }


    }
}
