using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public class StateMachine<StateKey, RunnerObject>
        where StateKey : Enum
        where RunnerObject : MonoBehaviour
    {
        public event Action<StateKey, StateKey> OnStateChanged;
        
        private readonly Dictionary<StateKey, BaseState<StateKey, RunnerObject>> stateDictionary = new();
        private BaseState<StateKey, RunnerObject> currentState;
        private bool debugMode = false;

        public void Begin(StateKey enterState)
        {
            TransitionToState(enterState);
        }

        public void Update()
        {
            currentState.BaseUpdate();

            if (currentState.TryGetTransitions(out StateKey targetState))
            {
                TransitionToState(targetState);
            }
        }

        public void FixedUpdate()
        {
            currentState.FixedUpdate();
        }

        public void TransitionToState(StateKey targetStateKey)
        {
            if (stateDictionary.TryGetValue(targetStateKey, out BaseState<StateKey, RunnerObject> targetState))
            {
                bool isTargetStateValid = targetState != null && targetState != currentState;
                if (isTargetStateValid)
                {
                    StateKey previousStateKey = currentState != null ? currentState.stateKey : targetStateKey;
                    currentState?.Exit();
                    currentState = targetState;
                    currentState.BaseEnter(previousStateKey);
                    OnStateChanged?.Invoke(previousStateKey, currentState.stateKey);
                    if (debugMode) Debug.Log(currentState.stateKey.ToString());
                }
            }
        }

        public void AddState(StateKey stateKey, BaseState<StateKey, RunnerObject> newState)
        {
            if (stateDictionary.ContainsKey(stateKey)) return;
            stateDictionary.Add(stateKey, newState);
            newState.stateKey = stateKey;
        }

        public void SetDebugMode(bool enabled)
        {
            debugMode = enabled;
        }

        public StateKey GetCurrentState() => currentState.stateKey;
        public string GetCurrentStateString() => currentState.stateKey.ToString();
    }
}

