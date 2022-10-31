using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayState : State
{
    public PlayState(StateManager manager, StateMachine stateMachine) : base(manager, stateMachine)
    {
    }

    private int clickCount = 0;
    bool canMove = false;

    public override void LogicUpdate()
    {
        if (manager.activePieces < manager.numberOfPieces)
        {
            if (manager.hitCollider.tag != "Player1" && manager.hitCollider.tag != "Player2")
            {
                if (manager.isMyTurn && manager.hitCollider.transform.childCount == 0)
                {
                    manager.isMyTurn = false;
                    manager.PlacePieces(new Vector3(manager.hitCollider.transform.position.x/(float)manager.width, manager.hitCollider.transform.position.y / (float)manager.width, manager.hitCollider.transform.position.z / (float)manager.width));
                    manager.SwitchPlayer(); //Event is to others so need player switch in local client 
                    manager.photonView.RPC("OnTurnEnd", Photon.Pun.RpcTarget.Others);
                    manager.photonView.RPC("ChangeTurn", Photon.Pun.RpcTarget.All);
                }
            }
        }

        else
        {
            switch (clickCount)
            {
                case 0:
                    if (manager.hitCollider.gameObject.tag == manager.currentPlayer.playerPf.tag && manager.isMyTurn)
                    {
                        manager.selectedPiece = manager.hitCollider.transform.gameObject;
                        manager.UpdateHitAddress();
                        clickCount++;
                    }
                    else if (manager.hitCollider.gameObject.tag == "PlayablePos" && manager.isMyTurn) //The collider of some clones does not detect raycast hence to avoid it
                    {
                        if (manager.hitCollider.transform.childCount != 0)
                        {
                            if (manager.hitCollider.transform.GetChild(0).tag == manager.currentPlayer.playerPf.tag)
                            {
                                manager.selectedPiece = manager.hitCollider.transform.GetChild(0).gameObject;
                                manager.UpdateHitAddress();
                                clickCount++;
                            }
                        }
                    }
                    break;

                case 1:
                    if (manager.hitCollider.tag == "PlayablePos")
                    {
                        CheckIfEdgePiece(manager.hitCollider.transform);

                        int parentIndex = manager.selectedPiece.transform.parent.transform.parent.GetSiblingIndex();
                        float actualDistance = Mathf.Round((manager.selectedPiece.transform.parent.transform.localPosition - manager.hitCollider.transform.localPosition).magnitude * 1000f) * 0.001f;
                        float requiredDistance = Mathf.Round((manager.unitDistance * (parentIndex + 1)) * 1000f) * 0.001f;

                        if (manager.hitCollider.transform.childCount == 0)
                        {
                            CheckIfCanMove(actualDistance, requiredDistance);
                        }
                        else canMove = false;
                        

                        if (canMove)
                        {
                            manager.photonView.RPC("OnDestroyPiece", Photon.Pun.RpcTarget.All, manager.hitAddress);
                            manager.UpdateHitAddress();
                            manager.MovePiece(new Vector3(manager.hitCollider.transform.position.x / (float)manager.width, manager.hitCollider.transform.position.y / (float)manager.width, manager.hitCollider.transform.position.z / (float)manager.width), manager.hitCollider.transform);
                            manager.SwitchPlayer();
                        }
                    }
                    clickCount = 0;
                    canMove = false;
                    break;

                default:
                    Debug.LogError("Clickcount is not changing properly");
                    break;
            }
        }
    }

    private void CheckIfEdgePiece(Transform transform)
    {
        manager.interRing = (transform.GetSiblingIndex() % 2 == 0) ? true : false;
    }

    private void CheckIfCanMove(float actualDistance, float requiredDistance)
    {
        if (manager.interRing)
        {
            if (actualDistance == requiredDistance || actualDistance == manager.unitDistance)
            {
                canMove = true;
            }
        }
        else
        {
            if (actualDistance == requiredDistance)
            {
                canMove = true;
            }
        }
    }
}
