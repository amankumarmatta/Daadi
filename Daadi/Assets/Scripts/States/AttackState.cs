using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AttackState : State
{
    public AttackState(StateManager manager, StateMachine stateMachine) : base(manager, stateMachine)
    {
    }

    public override void Enter()
    {
        Debug.Log("In attack");
    }

    public override void LogicUpdate()
    {    
        if (manager.hitCollider)
        {
            if (manager.hitCollider.tag == manager.currentPlayer.playerPf.tag)
            {
                OnAttackActions();
            }
            else if (manager.hitCollider.tag == "PlayablePos") //The collider of some clones does not detect raycast hence to avoid it
            {
                if(manager.hitCollider.transform.childCount != 0)
                {
                    if(manager.hitCollider.transform.GetChild(0).tag == manager.currentPlayer.playerPf.tag)
                    {
                        OnAttackActions();
                    }
                }
            }
            else
            {
                return;
            }
        }
        
    }

    private void OnAttackActions()
    {
        manager.commonMatch = false;
        manager.interRingMatch = false;

        manager.UpdateHitAddress();

        manager.photonView.RPC("OnAttackEnable", Photon.Pun.RpcTarget.All);
        manager.photonView.RPC("OnTurnEnd", Photon.Pun.RpcTarget.Others);
        manager.photonView.RPC("ChangeTurn", Photon.Pun.RpcTarget.All);

        stateMachine.ChangeState(manager.play);
    }
}
