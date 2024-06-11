using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Item
{
    public string ItemID;
    public GameObject ItemGameObject;
    public bool InitialState; // Indicates whether the item should be enabled initially
}

[System.Serializable]
public class Upgrade
{
    public string UpgradeID;
    public List<string> ItemIDsToEnable;
    public List<string> ItemIDsToDisable;
    public int cost;
}

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [SerializeField] private List<Item> items; // List of all items in the scene
    [SerializeField] private List<Upgrade> upgrades; // List of all possible upgrades

    private Dictionary<string, Item> itemDictionary; // Dictionary to quickly access items by ID
    private Dictionary<string, bool> itemStates; // Dictionary to keep track of item states across scenes

    private bool isInitialized = false;

    public bool upgradesStartedOnce = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist between scenes
            SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the scene loaded event
            itemStates = new Dictionary<string, bool>(); // Initialize the item states dictionary
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeItems();
    }

    private void InitializeItems()
    {
        if (itemDictionary == null)
        {
            itemDictionary = new Dictionary<string, Item>();
        }

        foreach (var item in items)
        {
            var foundObject = GameObject.Find(item.ItemID);
            if (foundObject != null)
            {
                item.ItemGameObject = foundObject;
                itemDictionary[item.ItemID] = item;

                // Initialize item state based on saved states or default state
                if (itemStates.ContainsKey(item.ItemID))
                {
                    item.ItemGameObject.SetActive(itemStates[item.ItemID]);
                }
                else
                {
                    item.ItemGameObject.SetActive(item.InitialState);
                    itemStates[item.ItemID] = item.InitialState;
                }
            }
            else
            {
               // Debug.LogError($"GameObject with name {item.ItemID} not found in the scene.");
            }
        }
    }

    public void ApplyUpgrade(string upgradeID)
    {
        // Find the upgrade
        var upgrade = upgrades.Find(u => u.UpgradeID == upgradeID);
        if (upgrade == null)
        {
            Debug.LogError($"Upgrade with ID {upgradeID} not found.");
            return;
        }

        

        // Enable items
        foreach (var itemID in upgrade.ItemIDsToEnable)
        {
            if (itemDictionary.TryGetValue(itemID, out var item))
            {
                item.ItemGameObject.SetActive(true);
                itemStates[itemID] = true;
            }
            else
            {
                Debug.LogError($"Item with ID {itemID} not found.");
            }
        }
        SoundManager.Instance.PlaySFX("money");

        // Disable items
        foreach (var itemID in upgrade.ItemIDsToDisable)
        {
            if (itemDictionary.TryGetValue(itemID, out var item))
            {
                item.ItemGameObject.SetActive(false);
                itemStates[itemID] = false;
            }
            else
            {
                Debug.LogError($"Item with ID {itemID} not found.");
            }
        }
    }

    // Expose the items and upgrades list for easier setup in the Inspector
    public List<Item> Items => items;
    public List<Upgrade> Upgrades => upgrades;
}
