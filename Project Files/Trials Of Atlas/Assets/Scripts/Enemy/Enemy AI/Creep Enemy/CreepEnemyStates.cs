using System.Collections;
using System.Collections.Generic;
using EnemyAI;
using UnityEngine;
using UnityEngine.AI;

public class CreepEnemyStates
{
    public class CreepAttackState : IState
    {
        private EnemyBrain _brain;
        private Animator _animator;
        private Transform _trans, _playerTrans;
        private NavMeshAgent _agent;
        
        
        public CreepAttackState(EnemyBrain brain)
        {
            _brain = brain;
        }

        public string GetName()
        {
            return "Attack";
        }

        public void EnterState(EnemyBehavior controller)
        {
            _animator = controller.anim;
            _trans = controller.transform;
            _agent = controller.agent;
            if (_brain.GetValue("Player Target") is GameObject playerObj) 
                _playerTrans = playerObj.transform;

           
        }

        public void UpdateState(EnemyBehavior controller)
        {
            var playerPos = _playerTrans.position;
            _agent.SetDestination(playerPos);

            if (Vector3.Distance(_trans.position, playerPos) < 2f)
            {
                Debug.Log("In Range for attack");
                _agent.ResetPath();
                _brain.ChangeState("Idle", controller);
            }
        }

        public void ExitState(EnemyBehavior controller)
        {
            Debug.Log("Leaving Attack State");
        }
    }
}
