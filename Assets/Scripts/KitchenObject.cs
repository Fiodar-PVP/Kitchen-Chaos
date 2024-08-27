using UnityEngine;

/// <summary>
/// Represents a kitchen object in the game, providing access to its associated scriptable object data.
/// </summary>
public class KitchenObject : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    /// <summary>
    /// Gets the associated KitchenObjectSO, which contains data about this kitchen object.
    /// </summary>
    /// <returns>The KitchenObjectSO associated with this kitchen object.</returns>
    public KitchenObjectSO GetKitchenObjectSO()
    {
        return kitchenObjectSO;
    }
}
