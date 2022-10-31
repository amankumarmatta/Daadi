using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndState : State
{
    public EndState(StateManager manager, StateMachine stateMachine) : base(manager, stateMachine)
    {
    }

    public override void Enter()
    {
        manager.text.text = manager.currentPlayer.name + " Wins";
    }
}
