using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CreepBrain : EnemyBrain
{
    public CreepBrain()
    {
        AddState("Idle", new CreepEnemyStates.CreepIdleState(this));
        AddState("Move", new RangeEnemyStates.RangeMoveState(this));
        AddState("Attack", new CreepEnemyStates.CreepAttackState(this));
    }
}
