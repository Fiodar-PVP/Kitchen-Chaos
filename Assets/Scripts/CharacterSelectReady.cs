using System.Collections.Generic;
using Unity.Netcode;

public class CharacterSelectReady : NetworkBehaviour
{
    public static CharacterSelectReady Instance { get; private set; }

    private Dictionary<ulong, bool> playerReadyDictionary;

    private void Awake()
    {
        Instance = this;

        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    public void SetLocalPlayerReady()
    {
        LocalPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void LocalPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        bool allClientsReady = true;

        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                //This player is NOT ready
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            Loader.LoadNetwork(Loader.Scene.GameScene);
        }
    }
}
