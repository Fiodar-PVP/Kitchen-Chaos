using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionResponseMessageUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI responseMessageText;

    private void Awake()
    {
        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnFailedToJoin += KitchenGameMultiplayer_OnFailedToJoin;

        Hide();
    }

    private void KitchenGameMultiplayer_OnFailedToJoin(object sender, System.EventArgs e)
    {
        Show();

        responseMessageText.text = NetworkManager.Singleton.DisconnectReason;

        if (responseMessageText.text == "")
        {
            //if connection Times out
            responseMessageText.text = "Failed to connect!";
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        KitchenGameMultiplayer.Instance.OnFailedToJoin -= KitchenGameMultiplayer_OnFailedToJoin;
    }
}
