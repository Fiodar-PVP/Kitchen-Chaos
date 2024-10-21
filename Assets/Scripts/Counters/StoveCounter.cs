using System;
using Unity.Netcode;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;

    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned,
    }

    private NetworkVariable<State> state = new NetworkVariable<State>(State.Idle);

    private NetworkVariable<float> fryingTimer = new NetworkVariable<float>(0f);
    private FryingRecipeSO fryingRecipeSO;

    private NetworkVariable<float> burningTimer = new NetworkVariable<float>(0f);
    private BurningRecipeSO burningRecipeSO;

    public override void OnNetworkSpawn()
    {
        fryingTimer.OnValueChanged += FryingTimer_OnValueChanged;
        burningTimer.OnValueChanged += BurningTimer_OnValueChanged;
        state.OnValueChanged += State_OnValueChanged;
    }

    private void FryingTimer_OnValueChanged(float previousValue, float newValue)
    {
        float fryingTimeMax = fryingRecipeSO != null ? fryingRecipeSO.fryingTimerMax : 1f;

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = fryingTimer.Value / fryingTimeMax
        });
    }

    private void BurningTimer_OnValueChanged(float previousValue, float newValue)
    {
        float burningTimeMax = burningRecipeSO != null ? burningRecipeSO.fryingTimerMax : 1f;

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = burningTimer.Value / burningTimeMax
        });
    }

    private void State_OnValueChanged(State previousValue, State newValue)
    {
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state.Value });

        if (state.Value == State.Burned || state.Value == State.Idle)
        {
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = 0
            });
        }
    }


    private void Update()
    {
        if (!IsServer) return;
        if (HasKitchenObject())
        {
            switch (state.Value)
            {
                case State.Idle:
                    break;

                case State.Frying:

                    fryingTimer.Value += Time.deltaTime;

                    if (fryingTimer.Value >= fryingRecipeSO.fryingTimerMax)
                    {
                        //Fried
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());

                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);

                        state.Value = State.Fried;

                        burningTimer.Value = 0f;

                        SetBurningRecipeClientRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(GetKitchenObject().GetKitchenObjectSO()));
                    }
                    break;

                case State.Fried:

                    burningTimer.Value += Time.deltaTime;

                    if (burningTimer.Value >= burningRecipeSO.fryingTimerMax)
                    {
                        //Burned
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());

                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

                        state.Value = State.Burned;
                    }
                    break;

                case State.Burned:
                    break;
            }
        }

    }


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

                    InteractLogicPlaceObjectOnCounterServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectSO()));
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

                        SetStateIdleServerRpc();
                    }
                }
            }
            else
            {
                //Player does not have a kitchen object. Give him a kitchen object.
                GetKitchenObject().SetKitchenObjectParent(player);

                SetStateIdleServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStateIdleServerRpc()
    {
        state.Value = State.Idle;
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc(int kitchenObjectSOIndex)
    {
        SetFryingRecipeClientRpc(kitchenObjectSOIndex);

        state.Value = State.Frying;

        fryingTimer.Value = 0f;
    }

    [ClientRpc]
    private void SetFryingRecipeClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);

        fryingRecipeSO = GetFryingRecipeSOWithInput(kitchenObjectSO);
    }

    [ClientRpc]
    private void SetBurningRecipeClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);

        burningRecipeSO = GetBurningRecipeSOWithInput(kitchenObjectSO);
    }

    private bool HasRecipeWithInput(KitchenObjectSO kitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(kitchenObjectSO);
        return fryingRecipeSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO kitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(kitchenObjectSO);
        return fryingRecipeSO.output;
    }

    /// <summary>
    /// Retrieves the FryingRecipeSO that corresponds to a given input kitchen object.
    /// </summary>
    /// <param name="inputKitchenObjectSO">The KitchenObjectSO to match against the input in the recipe.</param>
    /// <returns>
    /// The FryingRecipeSO that has the matching input kitchen object, or null if no match is found.
    /// </returns>
    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
        {
            if (fryingRecipeSO.input == inputKitchenObjectSO)
                return fryingRecipeSO;
        }

        return null;
    }

    private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray)
        {
            if (burningRecipeSO.input == inputKitchenObjectSO)
                return burningRecipeSO;
        }

        return null;
    }

    public bool IsFried()
    {
        return state.Value == State.Fried;
    }
}
