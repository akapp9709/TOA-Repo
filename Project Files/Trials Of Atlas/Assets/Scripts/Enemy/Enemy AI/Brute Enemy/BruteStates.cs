using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EnemyAI;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class BruteStates
{
    public class BruteAttackState : IState
    {
        private EnemyBrain _brain;
        private Animator _anim;
        public BruteAttackState(EnemyBrain brain)
        {
            _brain = brain;
        }

        public string GetName()
        {
            return "Brute Attack";
        }

        public void EnterState(EnemyBehavior controller)
        {
            _anim = controller.anim;
            _anim.SetTrigger("Attack");
        }

        public void UpdateState(EnemyBehavior controller)
        {
            var isAttacking = _anim.GetBool("isAttacking");

            if (!isAttacking)
            {
                _brain.ChangeState("Idle", controller);
            }
        }

        public void ExitState(EnemyBehavior controller)
        {

        }
    }

    public class BruteChargeState : IState
    {
        private EnemyBrain _brain;
        public BruteChargeState(EnemyBrain brain)
        {
            _brain = brain;
        }
        public string GetName()
        {
            return "Brute Charge";
        }

        public void EnterState(EnemyBehavior controller)
        {
            controller.anim.SetTrigger("Charge");
        }

        public void UpdateState(EnemyBehavior controller)
        {
            var charging = controller.anim.GetBool("isAttacking");
            if (!charging)
            {
                _brain.ChangeState("Idle", controller);
            }
        }

        public void ExitState(EnemyBehavior controller)
        {
            controller.OnTick = null;
        }
    }

    public class BruteChaseState : IState
    {
        private EnemyBrain _brain;
        private NavMeshAgent _agent;
        private Animator _anim;
        private Transform _player;
        private Timer _mainTimer;
        public BruteChaseState(EnemyBrain brain)
        {
            _brain = brain;
        }

        public string GetName()
        {
            return "Brute Chase";
        }

        public void EnterState(EnemyBehavior controller)
        {
            _agent = controller.agent;
            _anim = controller.anim;
            var playerObj = (GameObject)_brain.GetValue("target");
            _player = playerObj.transform;


            _anim.SetBool("isWalking", true);

            var chaseTime = (float)_brain.GetValue("Max Chase Time");
            _mainTimer = new Timer(chaseTime, () => { _brain.ChangeState("Stagger", controller); });
        }

        private void IncreaseChaseSpeed()
        {

        }

        public void UpdateState(EnemyBehavior controller)
        {
            _mainTimer.Tick(controller.delta);
            var inRange = Vector3.Distance(_player.position, controller.transform.position) < 1;
            if (inRange)
                _brain.ChangeState("Attack", controller);

            _agent.SetDestination(_player.position);
        }

        public void ExitState(EnemyBehavior controller)
        {
            _agent.ResetPath();
            _anim.SetBool("isWalking", false);
            _mainTimer = null;
        }
    }

    public class BruteStaggerState : IState
    {
        private EnemyBrain _brain;
        private Timer _timer;
        public BruteStaggerState(EnemyBrain brain)
        {
            _brain = brain;
        }

        public string GetName()
        {
            return "Brute Stagger";
        }
        public void EnterState(EnemyBehavior controller)
        {
            Debug.Log("Entered Stagger state");
            controller.anim.SetTrigger("Stagger");
            controller.anim.SetBool("isStaggered", true);
            var time = (float)_brain.GetValue("Stagger Time");
            _timer = new Timer(time, () => { _brain.ChangeState("Idle", controller); });
        }
        public void UpdateState(EnemyBehavior controller)
        {
            _timer.Tick(controller.delta);
        }
        public void ExitState(EnemyBehavior controller)
        {
            _timer = null;
            controller.anim.SetBool("isStaggered", false);
        }
    }

    public class BruteIdleState : IState
    {
        private EnemyBrain _brain;
        private string nextStateName;
        private EnemyBehavior _controller;

        private float _maxDist;
        private Transform _player;
        private Timer _timer;
        public BruteIdleState(EnemyBrain brain)
        {
            _brain = brain;
        }

        public string GetName()
        {
            return "Brute Idle";
        }
        public void EnterState(EnemyBehavior controller)
        {
            _controller = controller;
            _maxDist = (float)_brain.GetValue("Max Chase Distance");
            var playerObj = (GameObject)_brain.GetValue("target");
            _player = playerObj.transform;

            var t = (float)_brain.GetValue("Delay Time");
            _timer = new Timer(t, NextMove);
        }
        public void UpdateState(EnemyBehavior controller)
        {
            if (!_brain.isActive)
                return;

            nextStateName = (Vector3.Distance(_player.position, controller.transform.position) > _maxDist) ? "Charge" : "Chase";
            _timer.Tick(controller.delta);

        }
        public void ExitState(EnemyBehavior controller)
        {
            controller.OnTick = null;
        }

        private void NextMove()
        {
            _brain.ChangeState(nextStateName, _controller);
        }
    }
}
