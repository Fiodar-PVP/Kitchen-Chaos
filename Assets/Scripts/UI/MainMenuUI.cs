using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button multiplayerButton;
    [SerializeField] private Button singleplayerButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        multiplayerButton.onClick.AddListener(()=>
        {
            KitchenGameMultiplayer.playMultiplayer = true;
            Loader.Load(Loader.Scene.LobbyScene);
        });
        singleplayerButton.onClick.AddListener(() =>
        {
            KitchenGameMultiplayer.playMultiplayer = false;
            Loader.Load(Loader.Scene.LobbyScene);
        });
        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        //Make sure to unpause the game after leaving GameScene through Pause menu
        Time.timeScale = 1.0f;

        multiplayerButton.Select();
    }
}
