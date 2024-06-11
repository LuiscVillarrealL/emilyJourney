using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesSelection : MonoBehaviour
{
    [SerializeField] protected GameObject panel;
    [SerializeField] protected TextMeshProUGUI Description;
    [SerializeField] protected Button BuyButton;
    [SerializeField] protected Button CancelButton;
    [SerializeField] protected Button ContinueButton;

    [SerializeField] protected TextMeshProUGUI upgradesLeft;

    [SerializeField] protected OutlineSelectionScript outlineSelectionScript;

    [SerializeField] protected CameraController cameraController;

    [SerializeField] protected GameObject NoticePanel;
    [SerializeField] protected TextMeshProUGUI NoticePanelDes;
    [SerializeField] protected Button NoticePanelButton;
    [SerializeField] protected GameObject NumUpgradesPanel;

    private bool ButtonsGenerated = false;
    private GameObject selectedObject = null;

    public int upgradesNeeded = 3;

    private void Start()
    {
        BuyButton.onClick.AddListener(OnBuyButtonClicked);
        CancelButton.onClick.AddListener(OnClosePanelClick);
        ContinueButton.onClick.AddListener(OnClosePanelClick);
        NoticePanelButton.onClick.AddListener(ClearInfoInteractionButtons);

        if (!UpgradeManager.Instance.upgradesStartedOnce)
        {
            NoticePanel.SetActive(true);
            NoticePanelButton.gameObject.SetActive(true);
            NoticePanelDes.text = "Emily has enough money to make 3 upgrades in her house. Find objects in the house that act as barriers and upgrade them." +
                "Start with the objects that are more difficult to use.";


            UpgradeManager.Instance.upgradesStartedOnce = true;
        }
        else
        {
            NumUpgradesPanel.gameObject.SetActive(true);
            NoticePanel.SetActive(false);
            NoticePanelButton.gameObject.SetActive(false);
        }


    }

    private void OnClosePanelClick()
    {
        ClearInteractionButtons();
    }

    void Update()
    {



        upgradesLeft.text = $"Upgrades left for the day: {upgradesNeeded}";
        if (outlineSelectionScript == null)
        {
            return;
        }

        if (outlineSelectionScript.selection != null)
        {
            if (!ButtonsGenerated)
            {
                selectedObject = outlineSelectionScript.selection.gameObject;
                GenerateInteractionButtons();
                ButtonsGenerated = true;
            }

            if (ButtonsGenerated && selectedObject != outlineSelectionScript.selection.gameObject)
            {
                ClearInteractionButtons();
                selectedObject = outlineSelectionScript.selection.gameObject;
                GenerateInteractionButtons();
            }
        }
        else
        {
            ClearInteractionButtons();
            ButtonsGenerated = false;
            selectedObject = null;
        }
    }

    private void ClearInteractionButtons()
    {
        panel.SetActive(false);
        BuyButton.gameObject.SetActive(false);
        CancelButton.gameObject.SetActive(false);
        ContinueButton.gameObject.SetActive(false);
        cameraController.CanMoveCamera();
    }

    private void ClearInfoInteractionButtons()
    {
        NoticePanel.gameObject.SetActive(false);
        NoticePanelDes.gameObject.SetActive(false);
        NoticePanelButton.gameObject.SetActive(false);
        NumUpgradesPanel.gameObject.SetActive(true);

        cameraController.CanMoveCamera();
    }

    private void GenerateInteractionButtons()
    {
        if (outlineSelectionScript.selection == null)
        {
            return;
        }

        panel.SetActive(true);

        if (upgradesNeeded > 0)
        {
            var upgradeHandler = outlineSelectionScript.selection.GetComponent<ItemUpgradeHandler>();
            if (upgradeHandler != null && upgradeHandler.upgradeSO != null)
            {
                Description.text = upgradeHandler.upgradeSO.Description;
                BuyButton.gameObject.SetActive(true);
                CancelButton.gameObject.SetActive(true);
                ContinueButton.gameObject.SetActive(false);
                cameraController.DontMoveCamera();
            }
            else
            {
                Debug.LogWarning("Selected object does not have an ItemUpgradeHandler or UpgradeSO is null.");
                ClearInteractionButtons();
            }
        }
        else
        {
            Description.text = "Emily can't afford any more upgrades today!";
            ContinueButton.gameObject.SetActive(true);
            cameraController.DontMoveCamera();
        }


    }

    private void OnBuyButtonClicked()
    {
        if (outlineSelectionScript.selection == null)
        {
            return;
        }

        var upgradeHandler = outlineSelectionScript.selection.GetComponent<ItemUpgradeHandler>();
        if (upgradeHandler != null && upgradeHandler.upgradeSO != null)
        {
            ApplyUpgrade(upgradeHandler.upgradeSO.UpgradeId);
            UpgradeBought();
        }
        else
        {
            Debug.LogWarning("Selected object does not have an ItemUpgradeHandler or UpgradeSO is null.");
        }
    }

    private void ApplyUpgrade(string upgradeID)
    {
        // Implement the logic to apply the upgrade using the upgradeID
        Debug.Log($"Applying upgrade with ID: {upgradeID}");
        // Your upgrade application logic here
        BuyButton.interactable = false;
        UpgradeManager.Instance.ApplyUpgrade(upgradeID);
        upgradesNeeded--;
    }

    public void UpgradeBought()
    {
        if (outlineSelectionScript.selection == null)
        {
            return;
        }



        panel.SetActive(true);
        var upgradeHandler = outlineSelectionScript.selection.GetComponent<ItemUpgradeHandler>();
        if (upgradeHandler != null && upgradeHandler.upgradeSO != null)
        {
            Description.text = upgradeHandler.upgradeSO.DescriptionAfter;
            BuyButton.interactable = true;
            BuyButton.gameObject.SetActive(false);
            CancelButton.gameObject.SetActive(false);
            ContinueButton.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Selected object does not have an ItemUpgradeHandler or UpgradeSO is null.");
        }
    }
}
