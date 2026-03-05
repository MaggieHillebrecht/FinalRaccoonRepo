using System;
using UnityEngine;
using StateMachine;

public partial class TemplateRunner
{
    public class TemplateState : BaseState<StateKey, TemplateRunner>
    {
        public TemplateState(TemplateRunner runnerObject) : base(runnerObject) { }
        
        public override void Enter()
        {

        }

        public override void Update()
        {
            
        }

        public override void FixedUpdate()
        {
            
        }

        // Runs immediately after Update(). Use this to define your transition logic.
        public override bool TryGetTransitions(out StateKey targetState)
        {
            // Default if no transition is triggered
            targetState = StateKey.Idle;
            return false;
        }
        
        public override void Exit()
        {

        }
    }
}
