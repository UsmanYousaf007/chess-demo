#if UNITY_PURCHASING
using System;
using HUF.Purchases.Runtime.API.Data;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

namespace HUF.Purchases.Runtime.API.Services
{
    public interface IPurchasesService
    {
        bool IsInitialized { get; }
        event Action<Product[]> OnInitComplete;
        event Action<Product, IPurchaseReceipt> OnPurchaseSuccess;
        event Action<string, PurchaseFailureType> OnPurchaseFailure;
        event Action<string> OnPurchaseHandleInterrupted;

        void InitiatePurchase( Product product );
        void RestoreTransactions( Action<bool> restoreCallback );
        void UpdateSubscription( string oldProductId, string newProductId );
        string AccountCurrency { get; }
    }
}
#endif
