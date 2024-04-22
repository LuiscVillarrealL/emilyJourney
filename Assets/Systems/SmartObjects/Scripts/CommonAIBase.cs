using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.CullingGroup;

public enum EStat
{
    Energy,
    Fun
}

[RequireComponent(typeof(BaseNavigation))]
public class CommonAIBase : MonoBehaviour
{
    [Header("General")]
    [SerializeField] int HouseholdID = 1;


    [Header("Energy")]
    [SerializeField] float InitialEnergyLevel = 0.5f;
    [SerializeField] float baseEnergyDecayRate = 0.05f;
    [SerializeField] UnityEngine.UI.Slider EnergyDisplay;

    [Header("Fun")]
    [SerializeField] float InitialFunLevel = 0.5f;
    [SerializeField] float baseFunDecayRate = 0.05f;
    [SerializeField] UnityEngine.UI.Slider FunDisplay;

    protected BaseNavigation Navigation;
    protected BaseInteraction CurrentInteraction
    {
        get
        {

            BaseInteraction interaction = null;

            IndividualBlackboard.TryGetGeneric(EBlackboardKey.Character_FocusObject, out interaction, null);

            return interaction;
        }
        set {

            BaseInteraction previousInteraction = null;
            IndividualBlackboard.TryGetGeneric(EBlackboardKey.Character_FocusObject, out previousInteraction, null);

            IndividualBlackboard.SetGeneric(EBlackboardKey.Character_FocusObject, value);

            List<GameObject> objectsInUse = null;
            HouseholdBlackboard.TryGetGeneric(EBlackboardKey.Household_ObjectsInUse, out objectsInUse, null);

            // are we starting to use something?
            if (value != null)
            {
                // need to create list?
                if (objectsInUse == null)
                    objectsInUse = new List<GameObject>();

                // not already in list? add and update the blackboard
                if (!objectsInUse.Contains(value.gameObject))
                {
                    objectsInUse.Add(value.gameObject);
                    HouseholdBlackboard.SetGeneric(EBlackboardKey.Household_ObjectsInUse, objectsInUse);
                }
            } // we've stopped using something
            else if (objectsInUse != null)
            {
                // attempt to remove and update the blackboard if changed
                if (objectsInUse.Remove(previousInteraction.gameObject))
                    HouseholdBlackboard.SetGeneric(EBlackboardKey.Household_ObjectsInUse, objectsInUse);
            }

        }
    }
    protected bool StartedPerforming = false;

    public float CurrentFun
    {

        get { return IndividualBlackboard.GetFloat(EBlackboardKey.Character_Stat_Fun); }
        set { IndividualBlackboard.Set(EBlackboardKey.Character_Stat_Fun, value); }
    }
    public float CurrentEnergy {

        get{ return IndividualBlackboard.GetFloat(EBlackboardKey.Character_Stat_Energy);}
        set { IndividualBlackboard.Set(EBlackboardKey.Character_Stat_Energy, value); }
    }

    public Blackboard IndividualBlackboard { get; protected set; }

    public Blackboard HouseholdBlackboard { get; protected set; }

    protected virtual void Awake()
    {

        FunDisplay.value = InitialFunLevel;
        EnergyDisplay.value = InitialEnergyLevel;
        Navigation = GetComponent<BaseNavigation>();
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        HouseholdBlackboard = BlackboardManager.Instance.GetSharedBlackboard(HouseholdID);
        IndividualBlackboard = BlackboardManager.Instance.GetIndividualBlackboard(this);

        IndividualBlackboard.Set(EBlackboardKey.Character_Stat_Energy, InitialEnergyLevel);
        IndividualBlackboard.Set(EBlackboardKey.Character_Stat_Fun, InitialFunLevel);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (CurrentInteraction != null)
        {
            if (Navigation.IsAtDestination && !StartedPerforming)
            {
                StartedPerforming = true;
                CurrentInteraction.Perform(this, OnInteractionFinished);
            }

        }


        CurrentFun = Mathf.Clamp01(CurrentFun - baseFunDecayRate * Time.deltaTime);
        FunDisplay.value = CurrentFun;

        CurrentEnergy = Mathf.Clamp01(CurrentEnergy - baseEnergyDecayRate * Time.deltaTime);
        EnergyDisplay.value = CurrentEnergy;
    }

    protected virtual void OnInteractionFinished(BaseInteraction interaction)
    {
        interaction.UnlockInteraction();
        CurrentInteraction = null;
        Debug.Log($"Finished {interaction.DisplayName} ");
    }

    public void UpdateIndividualStat(EStat target, float amount)
    {
        switch (target)
        {
            case EStat.Energy: CurrentEnergy += amount; break;
            case EStat.Fun: CurrentFun += amount; break;
        }
        

    }
}
