using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SmartObject_TV))]
public class TVInteraction_TogglePower : SimpleInteraction
{

    protected SmartObject_TV LinkedTV;

    protected void Awake()
    {
        LinkedTV = GetComponent<SmartObject_TV>();
    }

    public override bool Perform(CommonAIBase performer, UnityAction<BaseInteraction> onCompleted = null)
    {
        LinkedTV.ToggleState();
        return base.Perform(performer, onCompleted);
    }



}
