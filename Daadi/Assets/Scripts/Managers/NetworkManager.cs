using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private StateManager manager;

    public bool onJoined;

    private static NetworkManager s_Instance = null;

    void Awake()
    {
        if (s_Instance == null)
        {
            s_Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateGame(InputField input)
    {
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 2;
        ro.PlayerTtl = 60000;
        ro.EmptyRoomTtl = 1000;
        ro.CleanupCacheOnLeave = true;
        PhotonNetwork.CreateRoom(input.text, ro, null);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log($"Connected to server");
    }

    public void JoinGame(InputField input)
    {
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 2;
        ro.PlayerTtl = 60000;
        ro.EmptyRoomTtl = 1000;
        ro.CleanupCacheOnLeave = true;
        PhotonNetwork.JoinOrCreateRoom(input.text, ro, TypedLobby.Default);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"Join room failed because of {message}. Creating new room");
        PhotonNetwork.CreateRoom(null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Player {PhotonNetwork.LocalPlayer.ActorNumber} joined the room");
        SceneManager.LoadScene(1);
        StartCoroutine(FindManager());
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player {newPlayer.ActorNumber} entered the room");
        onJoined = true;
    }

    private IEnumerator FindManager()
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "GameScene");
        manager = GameObject.FindGameObjectWithTag("Board").GetComponent<StateManager>();
        if (PhotonNetwork.IsMasterClient)
        {
            yield return new WaitUntil(() => onJoined == true);
            manager.isMyTurn = true;
            //manager.photonView.RPC("ChangeTurn", Photon.Pun.RpcTarget.All);
            manager.text.text = manager.currentPlayer.name + "'s Turn";
        }
        else manager.text.text = manager.currentPlayer.name + "'s Turn";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
    }
}
