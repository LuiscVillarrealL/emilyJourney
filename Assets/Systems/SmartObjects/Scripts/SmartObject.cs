using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartObject : MonoBehaviour
{

    [SerializeField] protected string _DisplayName;
    [SerializeField] protected Transform _InteractionMarkup;
    protected List<BaseInteraction> CachedInteractions = null;

    public Vector3 InteractionPoint => _InteractionMarkup != null ? _InteractionMarkup.position : transform.position;   


    public string DisplayName => _DisplayName;
    public List<BaseInteraction> Interactions
    {
        get
        {
            if (CachedInteractions == null)
                CachedInteractions = new List<BaseInteraction>(GetComponents<BaseInteraction>());

            return CachedInteractions;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.Instance.CurrentState == GameState.Playing)
        {
            SmartObjectManager.Instance.RegisterSmartObject(this);
        }
        
    }

    private void OnDestroy()
    {

        SmartObjectManager.Instance.DeRegisterSmartObject(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
