using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Scene Dependencies")]
    [SerializeField] private NetworkManager networkManager;

    [Header("MainMenu Pannel Interactables")]
    public GameObject mainMenuPanel;
    public Button playButton;
    public Button optionsButton;
    public Button quitButton;

    [Header("Options Pannel Interactables")]
    public GameObject optionsPanel;
    public Slider volSlider;
    public Button optionsBack;

    [Header("Lobby Pannel Interactables")]
    public GameObject lobbyPanel;
    public Button toCreatePanelButton;
    public Button toJoinPanelButton;
    public Button lobbyBack;

    [Header("Create Pannel Interactables")]
    public GameObject createCodePanel;
    public InputField createCode;
    public Button createButton;
    public Button createBack;

    [Header("Join Pannel Interactables")]
    public GameObject joinCodePanel;
    public InputField joinCode;
    public Button joinButton;
    public Button joinBack;

    [Header("Quit Pannel Interactables")]
    public GameObject quitPanel;
    public Button yesButton;
    public Button noButton;

    public void Awake()
    {
        DisableAllPanels();
        mainMenuPanel.SetActive(true);
    }
    public void Start()
    {
        playButton.onClick.AddListener(OnPlayClicked);
        optionsButton.onClick.AddListener(OnOptionsClicked);
        quitButton.onClick.AddListener(OnQuitClicked);

        optionsBack.onClick.AddListener(BackToMainMenu);

        lobbyBack.onClick.AddListener(BackToMainMenu);
        toCreatePanelButton.onClick.AddListener(Create);
        toJoinPanelButton.onClick.AddListener(Join);

        createButton.onClick.AddListener(OnConnect);
        createBack.onClick.AddListener(BackToLobby);

        joinBack.onClick.AddListener(BackToLobby);
        joinButton.onClick.AddListener(OnJoin);

        yesButton.onClick.AddListener(QuitGame);
        noButton.onClick.AddListener(BackToMainMenu);
    }

    public void OnConnect()
    {
        networkManager.CreateGame(createCode);
    }

    public void OnJoin()
    {
        networkManager.JoinGame(joinCode);
    }

    public void BackToLobby()
    {
        DisableAllPanels();
        lobbyPanel.SetActive(true);
    }
    public void OnPlayClicked()
    {
        DisableAllPanels();
        lobbyPanel.SetActive(true);
    }
    public void OnOptionsClicked()
    {
        DisableAllPanels();
        optionsPanel.SetActive(true);
    }

    public void OnQuitClicked()
    {
        DisableAllPanels();
        quitPanel.SetActive(true);
    }
    public void Create()
    {
        DisableAllPanels();
        createCodePanel.SetActive(true);
    }
    public void Join()
    {
        DisableAllPanels();
        joinCodePanel.SetActive(true);
    }
    
    public void BackToMainMenu()
    {
        DisableAllPanels();
        mainMenuPanel.SetActive(true);
    }
    public void MainMenuButton2()
    {
        DisableAllPanels();
        mainMenuPanel.SetActive(true);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void DisableAllPanels()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(false);
        quitPanel.SetActive(false);
        createCodePanel.SetActive(false);
        joinCodePanel.SetActive(false);
        lobbyPanel.SetActive(false);
    }
}
