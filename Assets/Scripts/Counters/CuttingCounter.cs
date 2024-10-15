using System;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
    public static event EventHandler OnAnyCut;

    new public static void ResetStaticData()
    {
        OnAnyCut = null;
    }

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnCut;

    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    private int cuttingProgress;

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
            {
                //Player has a kitchen object
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    //Player has a kitchen object which can be cut
                    KitchenObject kitchenObject = player.GetKitchenObject();
                    kitchenObject.SetKitchenObjectParent(this);

                    InteractLogicResetCutProgressServerRpc();
                }
            }
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
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    //Player is holding plate
                    if (plateKitchenObject.TryAddIngridient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    }
                }
            }
            else
            {
                //Player does not have a kitchen object. Give him a kitchen object.
                GetKitchenObject().SetKitchenObjectParent(player);

                InteractLogicResetCutProgressServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicResetCutProgressServerRpc()
    {
        InteractLogicPlaceObjectOnCounterClientRpc();
    }

    [ClientRpc]
    private void InteractLogicPlaceObjectOnCounterClientRpc()
    {
        cuttingProgress = 0;

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = 0f
        });
    }

    /// <summary>
    /// Handles the alternate interaction between the player and the kitchen counter. 
    /// This method checks whether there is a kitchen object on the counter 
    /// and whether there is a cut recipe for it, and acts accordingly.
    /// </summary>
    /// <param name="player">The player interacting with the kitchen counter.</param>
    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            //There is a kitchen object on counter AND it can be cut
            CutObjectServerRpc();
            CuttingProgressDoneServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CutObjectServerRpc()
    {
        CutObjectClientRpc();
    }

    [ClientRpc] private void CutObjectClientRpc()
    {
        cuttingProgress++;

        OnCut?.Invoke(this, EventArgs.Empty);
        OnAnyCut?.Invoke(this, EventArgs.Empty);

        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
        });

    }

    [ServerRpc(RequireOwnership = false)]
    private void CuttingProgressDoneServerRpc()
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

        if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
        {
            KitchenObjectSO kitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

            KitchenObject.DestroyKitchenObject(GetKitchenObject());

            KitchenObject.SpawnKitchenObject(kitchenObjectSO, this);
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO kitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(kitchenObjectSO);
        return cuttingRecipeSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO kitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(kitchenObjectSO);
        return cuttingRecipeSO.output;
    }

    /// <summary>
    /// Retrieves the CuttingRecipeSO that corresponds to a given input kitchen object.
    /// </summary>
    /// <param name="inputKitchenObjectSO">The KitchenObjectSO to match against the input in the recipe.</param>
    /// <returns>
    /// The CuttingRecipeSO that has the matching input kitchen object, or null if no match is found.
    /// </returns>
    private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if (cuttingRecipeSO.input == inputKitchenObjectSO)
                return cuttingRecipeSO;
        }

        return null;
    }
}
