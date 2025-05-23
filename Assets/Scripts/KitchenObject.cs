using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Represents a kitchen object in the game, providing access to its associated scriptable object data.
/// </summary>
public class KitchenObject : NetworkBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private FollowTransform followTransform;
    private IKitchenObjectParent kitchenObjectParent;

    protected virtual void Awake()
    {
        followTransform = GetComponent<FollowTransform>();
    }

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
        SetKitchenObjectParentServerRpc(kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetKitchenObjectParentServerRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        SetKitchenObjectParentClientRpc(kitchenObjectParentNetworkObjectReference);
    }

    [ClientRpc]
    private void SetKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

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

        followTransform.SetTargetTransform(kitchenObjectParent.GetKitchenObjectFollowTransform());
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void ClearKitchenObjectOnParent()
    {
        kitchenObjectParent.ClearKitchenObject();
    }

    /// <summary>
    /// Attempts to retrieve the object as a PlateKitchenObject if possible.
    /// </summary>
    /// <param name="plateKitchenObject">
    /// The PlateKitchenObject that will be assigned if the object is a PlateKitchenObject.
    /// </param>
    /// <returns>
    /// Returns true if the object is a PlateKitchenObject, otherwise returns false.
    /// </returns>
    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject)
    {
        if (this is PlateKitchenObject)
        {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }
        else
        {
            plateKitchenObject = null;
            return false;
        }
    }

    /// <summary>
    /// Spawns a KitchenObject using the provided KitchenObjectSO and sets its parent.
    /// </summary>
    /// <param name="kitchenObjectSO">
    /// The KitchenObjectSO that contains the prefab and data for the KitchenObject to be spawned.
    /// </param>
    /// <param name="kitchenObjectParent">
    /// The parent object (implementing IKitchenObjectParent) to assign to the spawned KitchenObject.
    /// </param>
    public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        KitchenGameMultiplayer.Instance.SpawnKitchenObject(kitchenObjectSO, kitchenObjectParent);
    }

    public static void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        KitchenGameMultiplayer.Instance.DestroyKitchenObject(kitchenObject);
    }
}
