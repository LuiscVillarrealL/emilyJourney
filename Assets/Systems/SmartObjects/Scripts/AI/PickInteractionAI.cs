using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;
using TMPro;

[RequireComponent(typeof(BaseNavigation))]
public class PickInteractionAI : CommonAIBase
{
    [SerializeField] protected float DefaultInteractionScore = 0f;
    [SerializeField] protected float PickInteractionInterval = 2f;
    [SerializeField] protected int InteractionPickSize = 30;
    [SerializeField] bool AvoidInUseObjects = true;

    protected float TimeUntilNextInteractionPicked = -1f;

    [SerializeField] protected OutlineSelectionScript OutlineSelectionScript;

    protected GameObject SelectedObject;

    [SerializeField]  protected bool ButtonsGenerated = false;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        StartCoroutine(GenerateInteractionButtonsAfterDelay());
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (OutlineSelectionScript.selection != null)
        {
            if (!ButtonsGenerated)
            {
                SelectedObject = OutlineSelectionScript.selection.gameObject;
                GenerateInteractionButtons();
                ButtonsGenerated = true;
            }


            if (ButtonsGenerated && SelectedObject != OutlineSelectionScript.selection.gameObject)
            {
                ClearInteractionButtons();
                SelectedObject = OutlineSelectionScript.selection.gameObject;
                GenerateInteractionButtons();
                ButtonsGenerated = true;
            }


        }   
        else
        {
            ClearInteractionButtons();
            ButtonsGenerated = false;
            SelectedObject = null;
        }

        

    }

    float ScoreInteraction(BaseInteraction interaction)
    {
        if (interaction.StatChanges.Length == 0)
        {
            return DefaultInteractionScore;
        }

        float score = 0f;
        foreach (var change in interaction.StatChanges)
            score += ScoreChange(change.LinkedStat, change.Value);

        return score;
    }

    float ScoreChange(AIStat linkedStat, float amount)
    {
        float currentValue = GetStatValue(linkedStat);

        return (1f - currentValue) * ApplyTraitsTo(linkedStat, Trait.ETargetType.Score, amount);
    }

    class ScoredInteraction
    {
        public SmartObject TargetObject;
        public BaseInteraction Interaction;
        public float Score;
    }

    IEnumerator GenerateInteractionButtonsAfterDelay()
    {
        yield return null; // Wait for one frame to make sure all the interactions are available

        GenerateInteractionButtons();
    }



    void PickBestInteraction(int selectedIndex, List<ScoredInteraction> sortedInteractions)
    {
        // Check if there's a current interaction in progress
        if (CurrentInteraction != null)
        {
            // Stop the current interaction
            CurrentInteraction.UnlockInteraction(this);
            CurrentInteraction = null;
        }

        var selectedObject = sortedInteractions[selectedIndex].TargetObject;
        var selectedInteraction = sortedInteractions[selectedIndex].Interaction;

        // Lock and set the new interaction
        CurrentInteraction = selectedInteraction;
        CurrentInteraction.LockInteraction(this);
        StartedPerforming = false;

        // Move to the target
        if (!Navigation.SetDestination(selectedObject.InteractionPoint))
        {
            Debug.LogError($"Could not move to {selectedObject.name}");
            CurrentInteraction = null;
        }
        else
        {
            Debug.Log($"Going to {CurrentInteraction.DisplayName} at {selectedObject.DisplayName}");
            StartCoroutine(StartInteractionAfterArrival(selectedObject.InteractionPoint));
        }

        ClearInteractionButtons();
    }

    IEnumerator StartInteractionAfterArrival(Vector3 targetPosition)
    {
        Vector3 actualposition = gameObject.transform.position;
        Vector3 destination = Navigation.Destination;
        // Wait until the AI reaches the destination
        while (!Navigation.IsAtDestination)
        {
            yield return null;
        }

        // Start the selected interaction after reaching the destination
        CurrentInteraction.Perform(this, OnInteractionFinished);
    }

    public void ClearInteractionButtons()
    {
        foreach (Transform child in LinkedUI.interactionButtonParent)
        {
            Destroy(child.gameObject);
        }
    }

  //  public void GenerateInteractionButtons()
  //  {
  //      ClearInteractionButtons();

  //      List<GameObject> objectsInUse = null;
  //      HouseholdBlackboard.TryGetGeneric(EBlackboardKey.Household_ObjectsInUse, out objectsInUse, null);

  //      // loop through all the objects
  //      List<ScoredInteraction> unsortedInteractions = new List<ScoredInteraction>();
  //      foreach (var smartObject in SmartObjectManager.Instance.RegisteredObjects)
  //      {
  //          // loop through all the interactions
  //          foreach (var interaction in smartObject.Interactions)
  //          {
  //              if (!interaction.CanPerform())
  //                  continue;

  //              // skip if someone else is using
  //              if (AvoidInUseObjects && objectsInUse != null && objectsInUse.Contains(interaction.gameObject))
  //                  continue;

  //              float score = ScoreInteraction(interaction);

  //              unsortedInteractions.Add(new ScoredInteraction()
  //              {
  //                  TargetObject = smartObject,
  //                  Interaction = interaction,
  //                  Score = score
  //              });
  //          }
  //      }

  //      if (unsortedInteractions.Count == 0)
  //          return;

  //      // sort and pick from one of the best interactions
  ////      var sortedInteractions = unsortedInteractions.OrderByDescending(scoredInteraction => scoredInteraction.Score).ToList();
  //      int maxIndex = Mathf.Min(InteractionPickSize, unsortedInteractions.Count);

  //      for (int i = 0; i < maxIndex; i++)
  //      {
  //          var interactionButtonObject = Instantiate(LinkedUI.interactionButtonPrefab, LinkedUI.interactionButtonParent);
  //          var interactionButton = interactionButtonObject.GetComponent<Button>();

  //          int index = i; // to avoid the modified closure issue

  //          interactionButton.onClick.AddListener(() => OnInteractionButtonClick(index, unsortedInteractions));
  //          interactionButton.GetComponentInChildren<TextMeshProUGUI>().text = unsortedInteractions[i].Interaction.DisplayName;
  //      }
  //  }

    public void GenerateInteractionButtons()
    {
        //ClearInteractionButtons();

        List<GameObject> objectsInUse = null;
        HouseholdBlackboard.TryGetGeneric(EBlackboardKey.Household_ObjectsInUse, out objectsInUse, null);

        // loop through all the objects
        List<ScoredInteraction> unsortedInteractions = new List<ScoredInteraction>();
        foreach (var smartObject in SmartObjectManager.Instance.RegisteredObjects)
        {

            if (smartObject.gameObject == SelectedObject)
            {
                // loop through all the interactions
                foreach (var interaction in smartObject.Interactions)
                {
                    if (!interaction.CanPerform(this))
                        continue;

                    // skip if someone else is using
                    if (AvoidInUseObjects && objectsInUse != null && objectsInUse.Contains(interaction.gameObject))
                        continue;

                    float score = ScoreInteraction(interaction);

                    unsortedInteractions.Add(new ScoredInteraction()
                    {
                        TargetObject = smartObject,
                        Interaction = interaction,
                        Score = score
                    });

                    if (!GameManager.Instance.tutorialSecondLoopFinished && interaction.isTutorial)
                    {
                        GameManager.Instance.tutorialSecondLoopFinished = true;
                        FindAnyObjectByType<FirstTime>().ContinueTutorial();

                    }
                }
            }

        }

        if (unsortedInteractions.Count == 0)
            return;

        // sort and pick from one of the best interactions
        //      var sortedInteractions = unsortedInteractions.OrderByDescending(scoredInteraction => scoredInteraction.Score).ToList();
        int maxIndex = Mathf.Min(InteractionPickSize, unsortedInteractions.Count);

        for (int i = 0; i < maxIndex; i++)
        {
            var interactionButtonObject = Instantiate(LinkedUI.interactionButtonPrefab, LinkedUI.interactionButtonParent);
            var interactionButton = interactionButtonObject.GetComponent<Button>();

            int index = i; // to avoid the modified closure issue

            interactionButton.onClick.AddListener(() => OnInteractionButtonClick(index, unsortedInteractions));
            interactionButton.GetComponentInChildren<TextMeshProUGUI>().text = unsortedInteractions[i].Interaction.DisplayName;
        }
    }

    void OnInteractionButtonClick(int selectedIndex, List<ScoredInteraction> sortedInteractions)
    {


        PickBestInteraction(selectedIndex, sortedInteractions);
        GenerateInteractionButtons(); // Regenerate buttons after interaction
    }
}
