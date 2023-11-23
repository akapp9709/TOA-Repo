using System.Collections;
using System.Collections.Generic;
using EnemyAI;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

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

    public class CreepIdleState : IState
    {
        private EnemyBrain _machine;
        private Transform _trans;
        private EnemyStats _stats;
        private EnemyBehavior _controller;

        public CreepIdleState(EnemyBrain machine)
        {
            _machine = machine;
        }

        public string GetName()
        {
            return "Ranged Idle";
        }

        public void EnterState(EnemyBehavior controller)
        {
            _trans = controller.transform;
            _stats = controller.enemySO;
            _controller = controller;
            controller.OnTick = CheckTime;
        }

        public void UpdateState(EnemyBehavior controller)
        {
            if (_machine.isActive)
            {
                var player = _machine.GetValue("Player Target");
                if (player == null || player.GetType() != typeof(GameObject)) return;

                var playerGO = player as GameObject;
                var playerTrans = playerGO.transform;

                var targetFwd = playerTrans.position - _trans.position;
                var targetRot = Quaternion.LookRotation(targetFwd);

                _trans.rotation = Quaternion.Slerp(_trans.rotation, targetRot, 0.1f);
            }
        }

        public void ExitState(EnemyBehavior controller)
        {
            controller.OnTick = null;
        }

        private void CheckTime()
        {
            if (!_machine.isActive)
            {
                return;
            }

            var rnd = new Random();
            var num = (float)rnd.NextDouble();

            if (num < _stats.aggression)
            {
                _machine.ChangeState("Attack", _controller);
            }
            else
            {
                _machine.ChangeState("Move", _controller);
            }
        }
    }
}
