using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    /// <summary>
    /// Handles the interaction between the player and the kitchen counter. 
    /// This method checks whether there is a kitchen object on the counter 
    /// and whether the player is holding a kitchen object, and acts accordingly.
    /// </summary>
    /// <param name="player">The player interacting with the kitchen counter.</param>
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            //There is no kitchen object on counter
            if (player.HasKitchenObject())
                //Player has a kitchen object. Put it on counter.
                player.GetKitchenObject().SetKitchenObjectParent(this);
            else
            {
                //Player does not have a kitchen object
            }
        }
        else
        {
            //There is a kitchen object on counter
            if (player.HasKitchenObject())
            {
                //Player has a kitchen object
            }
            else
            {
                //Player does not have a kitchen object. Give him a kitchen object.
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
}