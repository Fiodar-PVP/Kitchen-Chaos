using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelectSingleUI : MonoBehaviour
{
    [SerializeField] private int colorId;
    [SerializeField] private Image image;
    [SerializeField] private GameObject selectedGameObject;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            KitchenGameMultiplayer.Instance.ChangePlayerColor(colorId);
        });
    }

    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiplayer_OnPlayerDataListChanged;

        image.color = KitchenGameMultiplayer.Instance.GetPlayerColor(colorId);

        UpdateIsSelected();
    }

    private void KitchenGameMultiplayer_OnPlayerDataListChanged(object sender, System.EventArgs e)
    {
        UpdateIsSelected();
    }

    private void UpdateIsSelected()
    {
        PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerData();

        selectedGameObject.SetActive(playerData.colorId == colorId);
    }

    private void OnDestroy()
    {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= KitchenGameMultiplayer_OnPlayerDataListChanged;
    }
}
