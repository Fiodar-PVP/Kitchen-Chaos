using UnityEngine;

public class ClearCounter : MonoBehaviour, IKitchenObjectParent
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    [SerializeField] private Transform counterTopPoint;

    private KitchenObject kitchenObject;

    /// <summary>
    /// Handles interaction logic between the player and the kitchen object.
    /// If the kitchen object is not present, it instantiates one at the countertop point.
    /// If a kitchen object already exists, it is given to the player.
    /// </summary>
    /// <param name="player">The Player interacting with the kitchen object.</param>
    public void Interact(Player player)
    {
        if (kitchenObject == null)
        {
            Transform kitchenObjectSOTransform = Instantiate(kitchenObjectSO.prefab, counterTopPoint);
            kitchenObjectSOTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(this);
        }
        else
        {
            //Give kitchen object to Player
            kitchenObject.SetKitchenObjectParent(player);
        }
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return counterTopPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}
