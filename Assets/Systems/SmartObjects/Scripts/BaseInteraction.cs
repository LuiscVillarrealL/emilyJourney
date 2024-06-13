
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

    [SerializeField] private string _InteractionID = System.Guid.NewGuid().ToString(); // Unique identifier



    public string DisplayName => _DisplayName;
    public EInteractionType InteractionType => _InteractionType;
    public float Duration => _Duration;
    public InteractionStatChange[] StatChanges => _StatChanges;

    public abstract bool CanPerform(CommonAIBase character);
    public abstract bool LockInteraction(CommonAIBase performer);
    public abstract bool Perform(CommonAIBase performer, UnityAction<BaseInteraction> onCompleted);
    public abstract bool UnlockInteraction(CommonAIBase performer);


    public string InteractionID => _InteractionID; // Public getter for InteractionID

    private Coroutine statUpdateCoroutine;

    [SerializeField] protected MinigameBase minigame;

    public bool isTutorial = false;

    public void ApplyStatChanges(CommonAIBase performer, float proportion)
    {
        foreach (var statChange in StatChanges)
        {
            performer.UpdateIndividualStat(statChange.LinkedStat, statChange.Value * proportion, Trait.ETargetType.Impact);

            if (statChange.LinkedStat.ConnectedStat != null)
            {
                float connectedChange = statChange.Value * proportion * statChange.LinkedStat.ConnectedStatChangeRate;
                performer.UpdateIndividualStat(statChange.LinkedStat.ConnectedStat, connectedChange, Trait.ETargetType.Impact);
            }
        }
    }

    public void StartContinuousStatUpdates(CommonAIBase performer)
    {
        if (statUpdateCoroutine == null)
        {
            statUpdateCoroutine = StartCoroutine(UpdateStatsContinuously(performer));
        }
    }

    public void StopContinuousStatUpdates()
    {
        if (statUpdateCoroutine != null)
        {
            StopCoroutine(statUpdateCoroutine);
            statUpdateCoroutine = null;
        }
    }

    private IEnumerator UpdateStatsContinuously(CommonAIBase performer)
    {
        while (true)
        {
            ApplyStatChanges(performer, Time.deltaTime);
            yield return null;
        }
    }


}
