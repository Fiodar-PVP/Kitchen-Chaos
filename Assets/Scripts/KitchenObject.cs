using UnityEngine;

/// <summary>
/// Represents a kitchen object in the game, providing access to its associated scriptable object data.
/// </summary>
public class KitchenObject : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private IKitchenObjectParent kitchenObjectParent;

    /// <summary>
    /// Gets the associated KitchenObjectSO, which contains data about this kitchen object.
    /// </summary>
    /// <returns>The KitchenObjectSO associated with this kitchen object.</returns>
    public KitchenObjectSO GetKitchenObjectSO()
    {
        return kitchenObjectSO;
    }

    public IKitchenObjectParent GetKitchenObjectParent()
    {
        return kitchenObjectParent;
    }

    /// <summary>
    /// Assigns a new KitchenObjectParent to the current kitchen object, managing the transfer of any existing kitchen object.
    /// </summary>
    /// <param name="kitchenObjectParent">The new KitchenObjectParent to assign to this object.</param>
    /// <remarks>
    /// If the current object already has a KitchenObjectParent assigned, it will first clear any kitchen object from the previous KitchenObjectParent.
    /// After assigning the new KitchenObjectParent, the function checks if the new KitchenObjectParent already holds a kitchen object. 
    /// If it does, an error is logged to indicate a conflict. 
    /// The object is then set as the kitchen object for the new KitchenObjectParent and its transform is reset to the local position of the new parent.
    /// </remarks>
    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
    {
        if (this.kitchenObjectParent != null)
        {
            this.kitchenObjectParent.ClearKitchenObject();
        }


        this.kitchenObjectParent = kitchenObjectParent;

        if (kitchenObjectParent.HasKitchenObject())
        {
            Debug.LogError("KitchenObjectParent already has a KitchenObject!");
        }

        kitchenObjectParent.SetKitchenObject(this);

        transform.parent = kitchenObjectParent.GetKitchenObjectFollowTransform();
        transform.localPosition = Vector3.zero;
    }
}
