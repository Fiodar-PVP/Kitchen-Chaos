using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeDelivered;
    public event EventHandler OnRecipeFailed;
    public event EventHandler OnRecipeSuccess;

    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListSO recipeListSO;

    private List<RecipeSO> waitingRecipeSOList;
    private float spawnRecipeTimer = 4f;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipesMax = 4;
    private int successfullyDeliveredRecipies;
    //Used to ensure that recipe is delivered only once on Server Side
    private int waitingRecipeSOListCount;

    private void Awake()
    {
        Instance = this;
        waitingRecipeSOList = new List<RecipeSO>();
    }

    private void Update()
    {
        if (!IsServer) return;

        spawnRecipeTimer -= Time.deltaTime;

        if (spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (KitchenGameManager.Instance.IsPlaying() && waitingRecipeSOList.Count < waitingRecipesMax)
            {
                int waitingRecipeSOIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);

                SpawnNewWaitingRecipeClientRpc(waitingRecipeSOIndex);
            }
        }
    }

    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRpc(int waitingRecipeSOIndex)
    {
        RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeSOIndex];
        waitingRecipeSOList.Add(waitingRecipeSO);
        waitingRecipeSOListCount = waitingRecipeSOList.Count;

        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        //Check each Recipe in the waiting Recipe List
        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                //Has the same number of the ingridients
                bool plateContentsMatchesRecipe = true;
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                {
                    bool ingridientFound = false;
                    //Cycle throw all ingridients in the Recipe
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        //Cycle throw all ingridients in the Plate
                        if (recipeKitchenObjectSO == plateKitchenObjectSO)
                        {
                            //Ingridient matches!
                            ingridientFound = true;
                            break;
                        }
                    }
                    if (!ingridientFound)
                    {
                        //This Recipe ingridient was not found on the Plate
                        plateContentsMatchesRecipe = false;
                    }
                }

                if (plateContentsMatchesRecipe)
                {
                    //Player delivered the correct recipe!
                    DeliverCorrectRecipeServerRpc(i, waitingRecipeSOListCount);
                    waitingRecipeSOListCount = -1;
                    return;
                }
            }
        }

        // No matches found!
        // Player did not deliver correct recipe
        DeliverIncorrectRecipeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(int waitingRecipeSOListIndex, int waitingRecipeSOListCount)
    {
        if (waitingRecipeSOList.Count != waitingRecipeSOListCount)
        {
            //Run check in case client sends a lot of Rpc due to high ping
            //and the recipe already has been removed on Server side
            return;
        }
        
        DeliverCorrectRecipeClientRpc(waitingRecipeSOListIndex);
    }

    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int waitingRecipeSOListIndex)
    {
        waitingRecipeSOList.RemoveAt(waitingRecipeSOListIndex);
        successfullyDeliveredRecipies++;

        OnRecipeDelivered?.Invoke(this, EventArgs.Empty);
        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverIncorrectRecipeServerRpc()
    {
        DeliverIncorrectRecipeClientRpc();
    }

    [ClientRpc]
    private void DeliverIncorrectRecipeClientRpc()
    {
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }


    public List<RecipeSO> GetRecipeSOList()
    {
        return waitingRecipeSOList;
    }

    public int GetSuccessfullyDeliveredRecipies()
    {
        return successfullyDeliveredRecipies;
    }
}
