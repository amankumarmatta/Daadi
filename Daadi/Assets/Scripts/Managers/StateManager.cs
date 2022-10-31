using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

public class StateManager : MonoBehaviour
{
    #region Variables
    [Header("Input")]
    public PlayerInputAction playerInputAction;

    public float unitDistance;
    private bool isClicked;
    internal Collider2D hitCollider;

    [Header("Gameplay")]
    public int numberOfPieces;
    public int activePieces { get; private set; }
    internal GameObject selectedPiece;
    internal PlayerSO currentPlayer;

    [Header("Player Data")]
    public PlayerSO player1Data;
    public PlayerSO player2Data;

    [Header("States")]
    public StateMachine gameSM;
    public BeginState begin;
    public PlayState play;
    public AttackState attack;
    public EndState end;

    [Header("Attack Check")]
    internal bool interRing;
    internal bool commonMatch;
    internal bool interRingMatch;
    private bool inCheck; //for corner pieces to check attack status

    [Header("Photon Require Variables")]
    public TextMeshProUGUI text;
    public NetworkManager networkManager;
    public PhotonView photonView;
    internal int[] hitAddress = new int[2];
    internal bool isMyTurn;
    internal double width;


    #endregion

    #region Methods
    public void UpdateHitAddress()
    {
        int[] hitAddress = new int[2];

        if (hitCollider.transform.parent.childCount != 1)
        {
            hitAddress[0] = hitCollider.transform.GetSiblingIndex();
            hitAddress[1] = hitCollider.transform.parent.transform.GetSiblingIndex();
        }
        else
        {
            hitAddress[0] = hitCollider.transform.parent.transform.GetSiblingIndex();
            hitAddress[1] = hitCollider.transform.parent.transform.parent.transform.GetSiblingIndex();
        }

        photonView.RPC("UpdateBoardData", RpcTarget.All, hitAddress, numberOfPieces, activePieces, player1Data.piecesInPlay, player2Data.piecesInPlay);
    }

    public void PlacePieces(Vector3 position)
    {
        activePieces++;
        UpdateHitAddress();
        photonView.RPC("SpawnPiece", RpcTarget.All, position);
    }

    public void MovePiece(Vector3 position, Transform hit)
    {
        photonView.RPC("SpawnPiece", RpcTarget.All, position);
        isMyTurn = false;
        StartCoroutine(AttackCheck(hit));
    }

    public IEnumerator AttackCheck(Transform hit)
    {
        yield return new WaitUntil(() => selectedPiece == null); //wait till piece at previous place is destroyed

        Transform parent = hit.parent;
        int parentIndex = parent.GetSiblingIndex();
        int hitIndex = hit.GetSiblingIndex();
        List<int> adjacent = AttackPositions(hitIndex);

        CheckCommonMatch(adjacent, parent, hit);

        if (interRing) CheckInterRingMatch(parent, hit, parentIndex, hitIndex);

        if (commonMatch || interRingMatch)
        {
            gameSM.ChangeState(attack);
            interRingMatch = false;
            commonMatch = false;
            photonView.RPC("CanAttack", Photon.Pun.RpcTarget.All);
        }
        else
        {
            photonView.RPC("OnTurnEnd", Photon.Pun.RpcTarget.Others);
            photonView.RPC("ChangeTurn", Photon.Pun.RpcTarget.All);
        }
        interRing = false;
    }

    private List<int> AttackPositions(int index)
    {
        List<int> adjacent = new List<int>();

        adjacent.Add((index + 1 > 7) ? index - 7 : index + 1);
        adjacent.Add((index - 1 < 0) ? index + 7 : index - 1);

        return adjacent;
    }

    private bool ComparePositions(Transform parent, Transform hit, int index)
    {
        return (parent.GetChild(index).GetChild(0).tag == hit.GetChild(0).tag) ? true : false;
    }

    private void CheckCommonMatch(List<int> adjacent, Transform parent, Transform hit)
    {
        foreach (int var in adjacent)
        {
            if (parent.GetChild(var).childCount > 0)
            {
                if (!ComparePositions(parent, hit, var))
                {
                    if (interRing || inCheck)
                    {
                        commonMatch = false;
                        break;
                    }
                    else continue;
                }
                else
                {
                    if (!interRing && !inCheck)
                    {
                        inCheck = true;
                        CheckCommonMatch(AttackPositions(parent.GetChild(var).GetSiblingIndex()), parent, parent.GetChild(var).transform);
                    }
                    else commonMatch = true;
                }
            }
            else
            {
                if (interRing || inCheck)
                {
                    commonMatch = false;
                    break;
                }
                else continue;
            }
        }
        inCheck = false;
    }

    private void CheckInterRingMatch(Transform parent, Transform hit, int parentIndex, int hitIndex)
    {
        foreach (Transform var in parent.parent)
        {
            if (var.GetSiblingIndex() == parentIndex) continue;
            else
            {
                if (var.GetChild(hitIndex).childCount > 0)
                {
                    if (!ComparePositions(var, hit, hitIndex))
                    {
                        interRingMatch = false;
                        break;
                    }
                    else interRingMatch = true;
                }
                else
                {
                    interRingMatch = false;
                    break;
                }
            }
        }
    }
    public void SwitchPlayer()
    {
        currentPlayer = (currentPlayer == player1Data) ? player2Data : player1Data;
    }

    #endregion

    #region RPC Methods
    [PunRPC]
    private void SpawnPiece(Vector3 position)
    {
        Vector3 localPosition = position * (float)width;
        GameObject piece = Instantiate(currentPlayer.playerPf, localPosition, Quaternion.identity);
        piece.transform.localScale *= ((float)width / 20);
    }

    [PunRPC]
    private void UpdateBoardData(int[] hitAddress, int numberOfPieces, int activePieces, int player1Pieces, int player2Pieces)
    {
        this.hitAddress = hitAddress;
        this.numberOfPieces = numberOfPieces;
        this.activePieces = activePieces;
        this.player1Data.piecesInPlay = player1Pieces;
        this.player2Data.piecesInPlay = player2Pieces;
    }

    [PunRPC]
    private void OnDestroyPiece(int[] hitAddress)
    {
        Destroy(transform.GetChild(hitAddress[1]).GetChild(hitAddress[0]).GetChild(0).gameObject);
    }

    [PunRPC]
    private void OnTurnEnd()
    {
        SwitchPlayer();
        isMyTurn = true;
    }

    [PunRPC]
    private void ChangeTurn()
    {
        text.text = currentPlayer.name + "'s Turn";
    }

    [PunRPC]
    private void CanAttack()
    {
        if(currentPlayer == player1Data)
        {
            text.text = player2Data.name + " can Attack";
        }
        else text.text = player1Data.name + " can Attack";
    }

    [PunRPC]
    private void OnAttackEnable()
    {
        Destroy(transform.GetChild(hitAddress[1]).GetChild(hitAddress[0]).GetChild(0).gameObject);
        numberOfPieces--;
        activePieces--;

        currentPlayer.piecesInPlay--;
        photonView.RPC("UpdateBoardData", RpcTarget.Others, hitAddress, numberOfPieces, activePieces, player1Data.piecesInPlay, player2Data.piecesInPlay);
    }
    #endregion

    #region MonoBehaviour Callbacks
    private void Awake()
    {
        width = Camera.main.orthographicSize * 2.0 * Screen.width / Screen.height;
        transform.localScale = new Vector3((float)width, (float)width, 1);
    }

    private void Start()
    {
        player1Data.piecesInPlay = numberOfPieces / 2;
        player2Data.piecesInPlay = numberOfPieces / 2;
        photonView = GetComponent<PhotonView>();

        currentPlayer = player1Data;
        gameSM = new StateMachine();

        begin = new BeginState(this, gameSM);
        play = new PlayState(this, gameSM);
        attack = new AttackState(this, gameSM);
        end = new EndState(this, gameSM);

        gameSM.Initialize(begin);

        networkManager = GameObject.FindGameObjectWithTag("Network").GetComponent<NetworkManager>();
        StartCoroutine(UpdateLogic());
    }

    private IEnumerator UpdateLogic()
    {
        while (true)
        {
            if (player1Data.piecesInPlay == 0 || player2Data.piecesInPlay == 0)
            {
                SwitchPlayer();
                gameSM.Initialize(end);
                break;
            }

            if (isClicked)
            {
                isClicked = false;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hitRay = Physics2D.Raycast(ray.origin, ray.direction);

                if (hitRay.collider != null)
                {
                    hitCollider = hitRay.collider;
                    gameSM.CurrentState.LogicUpdate();
                }
            }

            yield return new WaitForSeconds(Random.Range(0.016f, 0.033f));
        }
    }

    private void OnEnable()
    {
        playerInputAction = new PlayerInputAction();
        playerInputAction.Player.Enable();

        playerInputAction.Player.Place.performed += Place_performed;
    }

    public void Place_performed(InputAction.CallbackContext obj)
    {
        isClicked = true;
    }

    private void OnDisable()
    {
        playerInputAction.Player.Place.performed -= Place_performed;
    }
}
    #endregion
