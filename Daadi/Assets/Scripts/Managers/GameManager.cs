using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public Button pauseButton, resumeButton, optionsButton, leaveButton, yesButton, noButton;
    public GameObject pauseMenuPanel, dialogPanel;

    public void Awake()
    {
        DisablePanels();
    }

    public void Start()
    {
        pauseButton.onClick.AddListener(PauseMenu);
        resumeButton.onClick.AddListener(Resume);
        leaveButton.onClick.AddListener(ExitMatch);
        yesButton.onClick.AddListener(Yes);
        noButton.onClick.AddListener(No);
    }
    public void PauseMenu()
    {
        DisablePanels();
        pauseMenuPanel.SetActive(true);
    }
    public void Resume()
    {
        DisablePanels();
    }
    public void ExitMatch()
    {
        DisablePanels();
        dialogPanel.SetActive(true);
    }
    public void Yes()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }
    public void No()
    {
        DisablePanels();
        pauseMenuPanel.SetActive(true);
    }
    public void Options()
    {
        SceneManager.LoadScene(0);
    }
    public void DisablePanels()
    {
        pauseMenuPanel.SetActive(false);
        dialogPanel.SetActive(false);
    }
}
