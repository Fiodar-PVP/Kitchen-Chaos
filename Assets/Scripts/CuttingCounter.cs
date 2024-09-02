using System;
using UnityEngine;

public class CuttingCounter : BaseCounter
{
    public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;
    public class OnProgressChangedEventArgs : EventArgs
    {
        public float progressNormalized;
    }

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
                    player.GetKitchenObject().SetKitchenObjectParent(this);

                    ResetCuttingProgress(this);
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
            }
            else
            {
                //Player does not have a kitchen object. Give him a kitchen object.
                GetKitchenObject().SetKitchenObjectParent(player);

                ResetCuttingProgress(player);
            }
        }
    }

    /// <summary>
    /// Handles the alternate interaction between the player and the kitchen counter. 
    /// This method checks whether there is a kitchen object on the counter 
    /// and whether there is a cut recipe for it, and acts accordingly.
    /// </summary>
    /// <param name="player">The player interacting with the kitchen counter.</param>
    public override void InteractAlternate(Player player)
    {
        if(HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            //There is a kitchen object on counter AND it can be cut
            cuttingProgress++;

            OnCut?.Invoke(this, EventArgs.Empty);

            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

            OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs
            {
                progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
            });

            if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
            {
                KitchenObjectSO kitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

                GetKitchenObject().DestroySelf();

                KitchenObject.SpawnKitchenObject(kitchenObjectSO, this);
            }
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

    /// <summary>
    /// Resets the cutting progress and updates the progress display for a given kitchen object parent.
    /// </summary>
    /// <param name="kitchenObjectParent">The object that holds the kitchen object whose cutting progress will be reset.</param>
    private void ResetCuttingProgress(IKitchenObjectParent kitchenObjectParent)
    {
        cuttingProgress = 0;

        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(kitchenObjectParent.GetKitchenObject().GetKitchenObjectSO());

        OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs
        {
            progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
        });
    }
}
