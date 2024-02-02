using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedBrain : EnemyBrain
{
    public RangedBrain()
    {
        AddState("Idle", new RangeEnemyStates.RangeIdleState(this));
        AddState("Move", new RangeEnemyStates.RangeMoveState(this));
        AddState("Attack", new RangeEnemyStates.RangeAttackState(this));
    }
}
