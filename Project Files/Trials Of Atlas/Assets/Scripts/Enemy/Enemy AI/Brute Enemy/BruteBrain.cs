using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruteBrain : EnemyBrain
{
    public BruteBrain()
    {
        AddState("Chase", new BruteStates.BruteChaseState(this));
        AddState("Attack", new BruteStates.BruteAttackState(this));
        AddState("Charge", new BruteStates.BruteChargeState(this));
        AddState("Stagger", new BruteStates.BruteStaggerState(this));
        AddState("Idle", new BruteStates.BruteIdleState(this));
    }
}
