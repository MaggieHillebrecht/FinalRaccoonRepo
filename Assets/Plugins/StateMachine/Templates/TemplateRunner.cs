using UnityEngine;
using StateMachine;

public partial class TemplateRunner : MonoBehaviour
{
    [SerializeField] private bool debugMode = false;
    
    private StateMachine<StateKey, TemplateRunner> stateMachine;
    public enum StateKey
    {
        Idle,
        Run,
        Aerial
    }

    private void Awake()
    {
        InitializeStateMachine();
    }

    private void InitializeStateMachine()
    {
        stateMachine = new StateMachine<StateKey, TemplateRunner>();
        stateMachine.SetDebugMode(debugMode);

        var idleState = new TemplateState(this);
        stateMachine.AddState(StateKey.Idle, idleState);
        
        // Repeat the above two lines for each state you want to add
        
        stateMachine.Begin(StateKey.Idle); // DONT FORGET THIS!!!
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }
}

