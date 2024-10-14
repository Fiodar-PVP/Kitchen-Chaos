using System;
using Unity.Netcode;

public class TrashCounter : BaseCounter
{
    public static EventHandler OnAnyObjectTrashed;
    new public static void ResetStaticData()
    {
        OnAnyObjectTrashed = null;

    }

    /// <summary>
    /// Handles the interaction between the player and the trash counter. 
    /// This method checks whether player has a kitchen object and if so
    /// destroys it.
    /// </summary>
    /// <param name="player">The player interacting with the kitchen counter.</param>
    public override void Interact(Player player)
    {
        if(player.HasKitchenObject())
        {
            //Player has a kitchen object;
            KitchenObject.DestroyKitchenObject(player.GetKitchenObject());

            InteractLogicServerRpc();
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
        OnAnyObjectTrashed?.Invoke(this, EventArgs.Empty);
    }
}
