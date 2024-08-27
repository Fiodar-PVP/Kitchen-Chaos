using UnityEngine;

/// <summary>
/// Represents a kitchen object in the game, providing access to its associated scriptable object data.
/// </summary>
public class KitchenObject : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private ClearCounter clearCounter;

    /// <summary>
    /// Gets the associated KitchenObjectSO, which contains data about this kitchen object.
    /// </summary>
    /// <returns>The KitchenObjectSO associated with this kitchen object.</returns>
    public KitchenObjectSO GetKitchenObjectSO()
    {
        return kitchenObjectSO;
    }

    public ClearCounter GetClearCounter()
    {
        return clearCounter;
    }

    /// <summary>
    /// Assigns a new ClearCounter to the current kitchen object, managing the transfer of any existing kitchen object.
    /// </summary>
    /// <param name="clearCounter">The new ClearCounter to assign to this object.</param>
    /// <remarks>
    /// If the current object already has a ClearCounter assigned, it will first clear any kitchen object from the previous ClearCounter.
    /// After assigning the new ClearCounter, the function checks if the new ClearCounter already holds a kitchen object. 
    /// If it does, an error is logged to indicate a conflict. 
    /// The object is then set as the kitchen object for the new ClearCounter and its transform is reset to the local position of the new counter.
    /// </remarks>
    public void SetClearCounter(ClearCounter clearCounter)
    {
        if (this.clearCounter != null)
        {
            this.clearCounter.ClearKitchenObject();
        }


        this.clearCounter = clearCounter;

        if (clearCounter.HasKitchenObject())
        {
            Debug.LogError("Counter already has something on it!");
        }

        clearCounter.SetKitchenObject(this);

        transform.parent = clearCounter.GetKitchenObjectFollowTransform();
        transform.localPosition = Vector3.zero;
    }
}
