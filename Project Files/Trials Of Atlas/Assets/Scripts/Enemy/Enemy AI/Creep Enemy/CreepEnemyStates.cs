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
        private EnemyBehavior _controller;
        private bool _attacking = false;


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
            _controller = controller;

            if (_brain.GetValue("Player Target") is GameObject playerObj)
                _playerTrans = playerObj.transform;

            controller.Action = EndAttack;
        }

        public void UpdateState(EnemyBehavior controller)
        {
            var playerPos = _playerTrans.position;
            var playerDir = playerPos - _trans.position;
            var playerDist = Vector3.Distance(playerPos, _trans.position);
            playerDir.Normalize();
            Debug.DrawLine(_trans.position + Vector3.up, _trans.position + playerDir * 3 + Vector3.up, Color.red, 0.5f);


            _agent.SetDestination(playerDir * (playerDist - 1.5f) + _trans.position);
            _animator.SetBool("isMoving", true);
            _animator.SetFloat("MoveY", 1f);

            if (Vector3.Distance(_trans.position, playerPos) < 2f && !_attacking)
            {
                _animator.SetBool("isMoving", false);

                _animator.SetTrigger("Attack");
                _attacking = true;
            }
            else if (_attacking)
            {
                _agent.ResetPath();
            }
        }

        public void ExitState(EnemyBehavior controller)
        {
            _agent.ResetPath();
        }

        private void EndAttack()
        {
            _attacking = false;
            _controller.Action = null;
            _brain.ChangeState("Move", _controller);
        }
    }
}
