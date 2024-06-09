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

    [SerializeField] protected OutlineSelectionScript outlineSelectionScript;

    [SerializeField] protected CameraController cameraController;

    private bool ButtonsGenerated = false;
    private GameObject selectedObject = null;

    private void Start()
    {
        BuyButton.onClick.AddListener(OnBuyButtonClicked);
        CancelButton.onClick.AddListener(OnClosePanelClick);
        ContinueButton.onClick.AddListener(OnClosePanelClick);
    }

    private void OnClosePanelClick()
    {
        ClearInteractionButtons();
    }

    void Update()
    {
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

    private void GenerateInteractionButtons()
    {
        if (outlineSelectionScript.selection == null)
        {
            return;
        }

        panel.SetActive(true);
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
