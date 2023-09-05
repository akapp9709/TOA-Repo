using System.Collections;
using System.Collections.Generic;
using EnemyAI;
using UnityEngine;

public class RangeEnemyStates
{
    public class RangeAttackState : IState
    {
        private EnemyFSM _machine;
        
        public RangeAttackState(EnemyFSM machine)
        {
            _machine = machine;
        }
        
        public string GetName()
        {
            return "Range Attack";
        }

        public void EnterState()
        {
            
        }

        public void UpdateState()
        {
            
        }

        public void ExitState()
        {
            
        }
    }

    public class RangeMoveState : IState
    {
        private EnemyFSM _machine;
        public RangeMoveState(EnemyFSM machine)
        {
            _machine = machine;
        }
        
        public string GetName()
        {
            return "Range Move";
        }

        public void EnterState()
        {
            
        }

        public void UpdateState()
        {
            
        }

        public void ExitState()
        {
            
        }
    }

    public class RangeIdleState : IState
    {
        private EnemyFSM _machine;
        public RangeIdleState(EnemyFSM machine)
        {
            _machine = machine;
        }
        
        public string GetName()
        {
            return "Ranged Idle";
        }

        public void EnterState()
        {
            
        }

        public void UpdateState()
        {
            
        }

        public void ExitState()
        {
            
        }
    }
}
