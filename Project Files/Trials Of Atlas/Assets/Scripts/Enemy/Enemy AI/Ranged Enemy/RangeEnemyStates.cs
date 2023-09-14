using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using EnemyAI;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public class RangeEnemyStates
{
    public class RangeAttackState : IState
    {
        private EnemyBrain _machine;
        private EnemyBehavior _controller;
        
        public RangeAttackState(EnemyBrain machine)
        {
            _machine = machine;
        }
        
        public string GetName()
        {
            return "Range Attack";
        }

        public void EnterState(EnemyBehavior controller)
        {
            Debug.Log("I am Attacking");
            _controller = controller;
            controller.OnTick = EndAttack;
        }

        public void UpdateState(EnemyBehavior controller)
        {
            
        }

        public void ExitState(EnemyBehavior controller)
        {
            Debug.Log("Leaving Attack State");
            controller.OnTick = null;
        }

        private void EndAttack()
        {
            _machine.ChangeState("Idle", _controller);
        }
    }

    public class RangeMoveState : IState
    {
        private EnemyBrain _machine;
        private NavMeshAgent agent;
        private EnemyBehavior _controller;
        private Transform _trans;
        private Vector3 _destPos;
        
        public RangeMoveState(EnemyBrain machine)
        {
            _machine = machine;
        }
        
        public string GetName()
        {
            return "Range Move";
        }

        public void EnterState(EnemyBehavior controller)
        {
            Debug.Log("I am Moving");
            _controller = controller;
            agent = _controller.agent;
            _trans = _controller.transform;

            var player = _machine.GetValue("Player Target") as GameObject;
            if (player == null)
            {
                Debug.Log("No Player Object in Dictionary", _controller.gameObject);
                return;
            }
            
            var pos = _trans.position;
            var playerPos = player.transform.position;

            var playerAttackRange = 3f;
            var attackRange = 15f;

            var maxDistance = 9f;
            int numCasts = 10;
            var angleIncrement = 360f / numCasts;
            List<Vector3> possiblePositions = new List<Vector3>();            
            NavMeshHit nmHit = new NavMeshHit();
            for (var i = 0; i < numCasts; i++)
            {
                var rayDir = Quaternion.Euler(0f, i * angleIncrement, 0f) * _trans.forward;
                var ray = new Ray(_trans.position, rayDir);
                Debug.DrawLine(pos, pos+ rayDir*maxDistance, Color.blue, 5f);

                var targetPos = pos + rayDir * maxDistance;
                var dist = Vector3.Distance(playerPos, targetPos);
                var dot = Vector3.Dot(playerPos - pos, rayDir);
                
                if (dist > playerAttackRange && dist < attackRange && dot < 0.4f 
                && NavMesh.SamplePosition(targetPos, out nmHit, maxDistance, NavMesh.AllAreas))
                {
                    possiblePositions.Add(targetPos);
                }
            }
            
            Debug.Log($"There are {possiblePositions.Count} available positions");
            foreach (var hit in possiblePositions)
            {
                Debug.DrawLine(pos + Vector3.up*2, hit + Vector3.up*2, Color.red, 5f);
            }

            if (possiblePositions.Count == 0) return;
            
            var rnd = new Random();
            var num = rnd.Next(0, possiblePositions.Count-1);
                
            agent.SetDestination(nmHit.position);
            _destPos = agent.destination;
        }

        public void UpdateState(EnemyBehavior controller)
        {
            var pos = _trans.position;
            if (Vector3.Distance(pos, _destPos) < 0.5f)
            {
                _machine.ChangeState("Idle", controller);
            }
        }

        public void ExitState(EnemyBehavior controller)
        {
            
        }
    }

    public class RangeIdleState : IState
    {
        private EnemyBrain _machine;
        private Transform _trans;
        private EnemyStats _stats;
        private EnemyBehavior _controller;
        
        public RangeIdleState(EnemyBrain machine)
        {
            _machine = machine;
        }
        
        public string GetName()
        {
            return "Ranged Idle";
        }

        public void EnterState(EnemyBehavior controller)
        {
            Debug.Log(GetName() + "This is my current state");
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
            var num = (float) rnd.NextDouble();

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
