using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashEndEvent : CombatEvent
{
    public virtual void EventAction(){
        Debug.Log("Dash End Event firing");
    }

    protected virtual void OnEnable() {
        
    }
}