using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyAI
{
    public abstract class EnemyFSM
    {
        private Dictionary<string, IState> _states = new Dictionary<string, IState>();
        private IState _currentState;

        public void AddState(string name, IState state)
        {
            if (!_states.ContainsKey(name))
            {
                _states[name] = state;
            }
        }

        public void StartFSM(string startState)
        {
            if (_states.ContainsKey(startState))
            {
                _currentState = _states[startState];
                _currentState.EnterState();
            }
        }

        public void UpdateFSM()
        {
            if (_currentState != null)
            {
                _currentState.UpdateState();
            }
        }

        public void ChangeState(string state)
        {
            if (_states.ContainsKey(state) &&
                !EqualityComparer<string>.Default.Equals(_currentState.GetName(), state))
            {
                _currentState.ExitState();
                _currentState = _states[state];
                _currentState.EnterState();
            }
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

