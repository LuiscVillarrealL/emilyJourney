using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartObjectManager : MonoBehaviour
{
    public static SmartObjectManager Instance { get; private set; } = null;

    public List<SmartObject> RegisteredObjects { get; private set; } = new List<SmartObject>();  

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError($"Trying to create seconf smartobject on {gameObject.name}");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RegisterSmartObject(SmartObject toRegister)
    {
        RegisteredObjects.Add(toRegister);
        Debug.Log("Registering " + toRegister.DisplayName);
    }

    public void DeRegisterSmartObject(SmartObject toDeRegister)
    {
        RegisteredObjects.Remove(toDeRegister);
    }
}
