using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyAI
{
    public abstract class EnemyFSM
    {
        private Dictionary<string, IState> _states = new Dictionary<string, IState>();
        private IState _currentState;

        public void SetupFSM()
        {
            
        }

        public void AddState(string name, IState state)
        {
            
        }

        public void StartFSM(IState startState)
        {
            
        }

        public void UpdateFSM()
        {
            
        }

        public void ChangeState(IState state)
        {
            
        }
    }

    public interface IState
    {
        string GetName();
        void EnterState();
        void UpdateState();
        void ExitState();
    }
}

