using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SmartObject_TV))]
public class TVInteractionChangeChannel : SimpleInteraction
{
    protected SmartObject_TV LinkedTV;

    protected void Awake()
    {
        LinkedTV = GetComponent<SmartObject_TV>();
    }
    public override bool CanPerform(CommonAIBase character)
    {
        return base.CanPerform(character) && LinkedTV.IsOn;
    }
}

