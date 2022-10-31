using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetParent : MonoBehaviour
{
    private StateManager manager;
    private GameObject board;

    // Start is called before the first frame update
    void Awake()
    {
        board = GameObject.FindGameObjectWithTag("Board");
        manager = board.GetComponent<StateManager>();

        gameObject.transform.parent = FindParent();
    }

    private Transform FindParent()
    {
        Transform parent;
        int[] parentAddress = new int[2];
        parentAddress = manager.hitAddress;

        parent = board.transform.GetChild(parentAddress[1]).GetChild(parentAddress[0]);
        return parent;
    }
}
