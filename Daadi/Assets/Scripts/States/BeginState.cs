using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeginState : State
{
    public BeginState(StateManager manager, StateMachine stateMachine) : base(manager, stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.ChangeState(manager.play);
    }   
    
    public override void Exit()
    {
        base.Exit();
    }


}
