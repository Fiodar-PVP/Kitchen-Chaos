using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
    public event EventHandler<OnIngridientAddedEventArgs> OnIngridientAdded;
    public class OnIngridientAddedEventArgs : EventArgs
    {
        public KitchenObjectSO kitchenObjectSO;
    }

    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;

    private List<KitchenObjectSO> kitchenObjectSOList;

    protected override void Awake()
    {
        base.Awake();

        kitchenObjectSOList = new List<KitchenObjectSO>();
    }

    public bool TryAddIngridient(KitchenObjectSO kitchenObjectSO)
    {
        if (!validKitchenObjectSOList.Contains(kitchenObjectSO))
        {
            //Not a valid ingridient
            return false;
        }

        if (kitchenObjectSOList.Contains(kitchenObjectSO))
        {
            //Already has this type
            return false;
        }
        else
        {
            AddIngridientServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObjectSO));

            return true;
        }
    }

    public List<KitchenObjectSO> GetKitchenObjectSOList()
    {
        return kitchenObjectSOList;
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddIngridientServerRpc(int kitchenObjectSOIndex)
    {
        AddIngridientClientRpc(kitchenObjectSOIndex);
    }

    [ClientRpc]
    private void AddIngridientClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        kitchenObjectSOList.Add(kitchenObjectSO);
        OnIngridientAdded?.Invoke(this, new OnIngridientAddedEventArgs
        {
            kitchenObjectSO = kitchenObjectSO
        });
    }
}
