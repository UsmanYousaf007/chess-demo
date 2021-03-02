using System;
using HUF.Purchases.Runtime.API.Data;

#if !UNITY_2018

#endif
namespace HUF.Purchases.Runtime.API.Models
{
    public interface IPurchasesModel
    {
        bool IsInitialized { get; }

        event Action OnInitialized;
        event Action<IProductInfo> OnPurchaseInit;
        event Action<IProductInfo, TransactionType, PriceConversionData, PurchaseReceiptData> OnPurchaseSuccess;
        event Action<IProductInfo, PurchaseFailureType> OnPurchaseFailure;
        event Action<IProductInfo> OnRefundSuccess;
        event Action OnRestoreFailure;
        event Action<ISubscriptionPurchaseData> OnSubscriptionPurchase;
        event Action<IProductInfo> OnSubscriptionExpired;
        event Action<IProductInfo> OnPurchaseHandleInterrupted;

        void Init();
        void UpdateSubscription( string oldProductId, string newProductId );
        bool IsProductAvailable( string productId );
        bool IsNonConsumableBought( string productId );
        string GetLocalizedPrice( string productId, bool isoFormat );
        decimal GetPrice( string productId );
        string GetCurrencyCode( string productId );
        void BuyProduct( string productId );

        void RestorePurchases();

        SubscriptionStatus GetSubscriptionStatus( string subscriptionId );
        bool IsSubscriptionActive( string subscriptionId );
        bool IsSubscriptionInTrialMode( string subscriptionId );
        DateTime GetSubscriptionExpirationDate( string subscriptionId );
        int GetSubscriptionTrialPeriod( string subscriptionId );
        int GetSubscriptionPeriod( string subscriptionId );
        PriceConversionData GetPriceConversionData();
        void TryGetPriceConversionData( Action<PriceConversionResponse> response, string currency );
    }
}