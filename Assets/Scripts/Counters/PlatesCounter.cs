using System;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    public EventHandler OnPlatesSpawned;
    public EventHandler OnPlatesRemoved;

    [SerializeField] private KitchenObjectSO platesKitchenSO;
    [SerializeField] private float spawnPlateTimerMax = 4f;
    [SerializeField] private int platesSpawnedAmountMax = 4;

    private float spawnPlateTimer;
    private int platesSpawnedAmount;

    private void Update()
    {
        if(!IsServer) return;

        spawnPlateTimer += Time.deltaTime;

        if(spawnPlateTimer >= spawnPlateTimerMax)
        {
            spawnPlateTimer = 0;

            if(KitchenGameManager.Instance.IsPlaying() && platesSpawnedAmount < platesSpawnedAmountMax)
            {
                SpawnPlateClientRpc();
            }
        }
    }

    [ClientRpc]
    private void SpawnPlateClientRpc()
    {
        platesSpawnedAmount++;

        OnPlatesSpawned?.Invoke(this, EventArgs.Empty);
    }

    public override void Interact(Player player)
    {
        if(!player.HasKitchenObject())
        {
            //Player does not have any kitchen object
            if(platesSpawnedAmount > 0)
            {
                //There is at least one plate on the counter
                KitchenObject.SpawnKitchenObject(platesKitchenSO, player);

                InteractLogicServerRpc();
            }
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        platesSpawnedAmount--;

        OnPlatesRemoved?.Invoke(this, EventArgs.Empty);
    }
}
