using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashStartEvent : CombatEvent
{
    public virtual void EventAction(){
        Debug.Log("Dash Start Event firing");
    }

    protected virtual void OnEnable() {
        
    }
}