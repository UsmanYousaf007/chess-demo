using System;
using System.Collections.Generic;
using System.Linq;
using HUF.Purchases.Runtime.API.Data;
using HUF.Purchases.Runtime.API.Models;
using HUF.Purchases.Runtime.API.Services;
using HUF.Purchases.Runtime.Implementation.Data;
using HUF.Purchases.Runtime.Implementation.Services;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using HUF.Utils.Runtime.UI.CanvasBlocker;
using JetBrains.Annotations;

namespace HUF.Purchases.Runtime.Implementation.Models
{
    [UsedImplicitly]
    public class EmptyPurchasesModel : IPurchasesModel
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(EmptyPurchasesModel) );
        readonly HashSet<string> boughtProducts = new HashSet<string>();

        public bool IsInitialized => isInitialized;

#pragma warning disable 0067
        public event Action OnInitialized;
        public event Action<IProductInfo> OnPurchaseInit;

        public event Action<IProductInfo, TransactionType, PriceConversionData, PurchaseReceiptData>
            OnPurchaseSuccess;

        public event Action<IProductInfo, PurchaseFailureType> OnPurchaseFailure;
        public event Action<IProductInfo> OnRefundSuccess;
        public event Action OnRestoreFailure;
        public event Action<ISubscriptionPurchaseData> OnSubscriptionPurchase;
        public event Action<IProductInfo> OnSubscriptionExpired;
        public event Action<IProductInfo> OnPurchaseHandleInterrupted;
#pragma warning restore 0067

        bool isInitialized = false;
        IPriceConversionService priceConversionService;
        string purchaseProductId;
        PurchasesConfig purchasesConfig = null;

        public void Init()
        {
            purchasesConfig = HConfigs.GetConfig<PurchasesConfig>();

            if ( purchasesConfig == null )
            {
                HLog.LogError( logPrefix, $"Missing purchase config: {nameof(purchasesConfig)}" );
                return;
            }

            priceConversionService = new PriceConversionService( purchasesConfig );
            isInitialized = true;
            OnInitialized.Dispatch();
        }

        public void UpdateSubscription( string oldProductId, string newProductId )
        {}

        bool HaveInitAndConfig()
        {
            return isInitialized && purchasesConfig != null;
        }

        public bool IsProductAvailable( string productId )
        {
            if (HaveInitAndConfig() )
            {
                var product = purchasesConfig.Products.FirstOrDefault( p => p.ProductId == productId );

                if ( product != null )
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsNonConsumableBought( string productId )
        {
            return boughtProducts.Contains( productId );
        }

        public string GetLocalizedPrice( string productId, bool isoFormat )
        {
            return $"{GetPrice( productId )} {( isoFormat ? GetCurrencyCode( productId ) : "$" )}";
        }

        public void BuyProduct( string productId )
        {
            HLog.Log( logPrefix, $"Try to buy {productId}" );

            if ( !IsProductAvailable( productId ) )
            {
                var product = purchasesConfig.Products.FirstOrDefault( p => p.ProductId == productId );
                OnPurchaseFailure.Dispatch( product, PurchaseFailureType.Unknown );
            }

            purchaseProductId = productId;

            if ( purchasesConfig.ShowDebugMenuInEditor )
            {
                foreach(TestPaymentResult result in Enum.GetValues(typeof (TestPaymentResult)))
                {
                    AddDebugScreenButton( result );
                }
                DebugButtonsScreen.Instance.Show("Purchase Debug Screen");
            }
            else
            {
                HandlePurchase( purchasesConfig.EditorDefaultPurchaseSuccess ? TestPaymentResult.Success: TestPaymentResult.Fail );
            }
        }

        void AddDebugScreenButton( TestPaymentResult result )
        {
            DebugButtonsScreen.Instance.AddGUIButton( $"Result:{result}", () => HandlePurchase( result ) );
        }

        public void HandlePurchase( TestPaymentResult result )
        {
            if ( purchasesConfig.ShowDebugMenuInEditor )
            {
                DebugButtonsScreen.Instance.Hide();
            }

            switch ( result )
            {
                case TestPaymentResult.Success:
                {
                    boughtProducts.Add( purchaseProductId );

                    OnPurchaseSuccess.Dispatch(
                        purchasesConfig.Products.FirstOrDefault( p => p.ProductId == purchaseProductId ),
                        TransactionType.Purchase,
                        null,
                        null );
                    break;
                }
                case TestPaymentResult.Fail:
                {
                    OnPurchaseFailure.Dispatch(
                        purchasesConfig.Products.FirstOrDefault( p => p.ProductId == purchaseProductId ),
                        PurchaseFailureType.Unknown );
                    break;
                }
                case TestPaymentResult.Cancel:
                {
                    OnPurchaseFailure.Dispatch(
                        purchasesConfig.Products.FirstOrDefault( p => p.ProductId == purchaseProductId ),
                        PurchaseFailureType.UserCancelled );
                    break;
                }
            }
        }

        public void RestorePurchases()
        {
            HLog.Log( logPrefix, $"Try restore purchases" );
        }

        public SubscriptionStatus GetSubscriptionStatus( string subscriptionId )
        {
            if (HaveInitAndConfig())
                return boughtProducts.Contains( subscriptionId ) ? SubscriptionStatus.Active : SubscriptionStatus.Unknown;

            return SubscriptionStatus.Unknown;
        }

        public bool IsSubscriptionActive( string subscriptionId )
        {
            if (HaveInitAndConfig())
                return purchasesConfig.EditorSubscriptionsAlwaysActive || boughtProducts.Contains( subscriptionId );

            return false;
        }

        public bool IsSubscriptionInTrialMode( string subscriptionId )
        {
            if (HaveInitAndConfig())
                return purchasesConfig.EditorSubscriptionsAlwaysInTrialMode;

            return false;
        }

        public DateTime GetSubscriptionExpirationDate( string subscriptionId )
        {
            if ( IsSubscriptionActive( subscriptionId ) )
            {
                return DateTime.UtcNow.AddDays( 1 );
            }

            return DateTime.MinValue;
        }

        public int GetSubscriptionTrialPeriod( string subscriptionId )
        {
            if (HaveInitAndConfig() )
            {
                var product = purchasesConfig.Products.FirstOrDefault( p => p.ProductId == subscriptionId );

                if ( product != null )
                {
                    return product.SubscriptionTrialPeriod;
                }
            }

            return 0;
        }

        public int GetSubscriptionPeriod( string subscriptionId )
        {
            if (HaveInitAndConfig() )
            {
                var product = purchasesConfig.Products.FirstOrDefault( p => p.ProductId == subscriptionId );

                if ( product != null )
                {
                    return product.SubscriptionPeriod;
                }
            }

            return 0;
        }

        public PriceConversionData GetPriceConversionData()
        {
            return priceConversionService.GetConversionData();
        }

        public void TryGetPriceConversionData( Action<PriceConversionResponse> response, string currency )
        {
            priceConversionService.TryGetPriceConversionData( response, currency );
        }

        public decimal GetPrice( string productId )
        {
            if (HaveInitAndConfig() )
            {
                var product = purchasesConfig.Products.FirstOrDefault( p => p.ProductId == productId );

                if ( product != null )
                {
                    return (decimal)(product.PriceInCents / 100f);
                }
            }

            return 0m;
        }

        public string GetCurrencyCode( string productId )
        {
            if (HaveInitAndConfig() )
                return purchasesConfig.PriceConversionCurrency.IsNotEmpty() ? purchasesConfig.PriceConversionCurrency : "USD" ;

            return string.Empty;
        }

        public enum TestPaymentResult
        {
            Success,
            Fail,
            Cancel
        }
    }
}