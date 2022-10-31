using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game Data/Player Data")]
public class PlayerSO : ScriptableObject
{
    public new string name;
    public GameObject playerPf;
    public Sprite playerSprite;
    public Color playerColor;
    public int piecesInPlay;
}
