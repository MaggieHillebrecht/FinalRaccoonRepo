using System;
using UnityEngine;

namespace StateMachine
{
    public abstract class BaseState<StateKey, RunnerObject>
        where StateKey : Enum
        where RunnerObject : MonoBehaviour
    {
        protected RunnerObject runnerObject;
        public StateKey stateKey;
        public StateKey previousStateKey;
        protected float stateTimer;

        public BaseState(RunnerObject runnerObject)
        {
            this.runnerObject = runnerObject;
        }

        public void BaseEnter(StateKey previousState)
        {
            stateTimer = 0.0f;
            previousStateKey = previousState;
            Enter();
        }
        public abstract void Enter();

        public void BaseUpdate()
        {
            stateTimer += Time.deltaTime;
            Update();
        }
        public abstract void Update();

        public abstract void FixedUpdate();

        public abstract bool TryGetTransitions(out StateKey targetState);
        public abstract void Exit();
    }
}