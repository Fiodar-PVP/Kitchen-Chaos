using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour
{

    [SerializeField] private TMP_InputField lobbyNameInputField;
    [SerializeField] private Button createPublicLobbyButton;
    [SerializeField] private Button createPrivateLobbyButton;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        createPublicLobbyButton.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.CreateLobby(lobbyNameInputField.text, false);
        });
        createPrivateLobbyButton.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.CreateLobby(lobbyNameInputField.text, true);
        });
        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });

        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        createPublicLobbyButton.Select();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        LobbyUI.Instance.SelectCreateLobbyButton();
    }
}
