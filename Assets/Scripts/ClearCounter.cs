using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    /// <summary>
    /// Handles interaction logic between the player and the kitchen object.
    /// If the kitchen object is not present, it instantiates one at the countertop point.
    /// If a kitchen object already exists, it is given to the player.
    /// </summary>
    /// <param name="player">The Player interacting with the kitchen object.</param>
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            Transform kitchenObjectSOTransform = Instantiate(kitchenObjectSO.prefab);
            kitchenObjectSOTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(this);
        }
        else
        {
            //Give kitchen object to Player
            GetKitchenObject().SetKitchenObjectParent(player);
        }
    }

}
