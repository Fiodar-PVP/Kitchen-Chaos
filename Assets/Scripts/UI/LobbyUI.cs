using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    public static LobbyUI Instance { get; private set; }

    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private TMP_InputField codeInputField;
    [SerializeField] private Button joinCodeButton;
    [SerializeField] private LobbyCreateUI lobbyCreateUI;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private Transform container;
    [SerializeField] private Transform lobbyTemplate;

    private void Awake()
    {
        Instance = this;

        mainMenuButton.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });

        createLobbyButton.onClick.AddListener(() =>
        {
            lobbyCreateUI.Show();
        });

        quickJoinButton.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.QuickJoin();
        });

        joinCodeButton.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.JoinWithCode(codeInputField.text);
        });

        lobbyTemplate.gameObject.SetActive(false);
        SelectCreateLobbyButton();
    }

    private void Start()
    {
        KitchenGameLobby.Instance.OnLobbyListChanged += KitchenGameLobby_OnLobbyListChanged;

        playerNameInputField.text = KitchenGameMultiplayer.Instance.GetPlayerName();

        playerNameInputField.onValueChanged.AddListener((string playerName) =>
        {
            KitchenGameMultiplayer.Instance.SetPlayerName(playerName);
        });

        UpdateLobbyList(new List<Lobby>());
    }

    private void KitchenGameLobby_OnLobbyListChanged(object sender, KitchenGameLobby.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (Transform child in container.transform)
        {
            if (child == lobbyTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (Lobby lobby in lobbyList)
        {
            Transform templateTransform = Instantiate(lobbyTemplate, container);
            templateTransform.gameObject.SetActive(true);
            templateTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
        }
    }

    public void SelectCreateLobbyButton()
    {
        createLobbyButton.Select();
    }

    private void OnDestroy()
    {
        KitchenGameLobby.Instance.OnLobbyListChanged -= KitchenGameLobby_OnLobbyListChanged;
    }
}
