using System;
using HUF.Purchases.Runtime.API.Data;
using UnityEngine;

namespace HUF.Purchases.Runtime.Implementation.Data
{
    [Serializable]
    public partial class ProductInfo : IProductInfo
    {
        public int SubscriptionTrialPeriod => subscriptionSpecificInfo.TrialPeriodInDays;
        public int SubscriptionPeriod => subscriptionSpecificInfo.PeriodInDays;

        [SerializeField] IAPProductType type = default;
        [SerializeField] string productId = default;
        [SerializeField] ProductShopInfo androidConfig = default;
        [SerializeField] ProductShopInfo iOSConfig = default;
        [SerializeField] SubscriptionSpecificInfo subscriptionSpecificInfo = default;

        public ProductInfo( IAPProductType type,
            string id,
            string androidID = "",
            string iOSId = "",
            int priceInCents = 0,
            SubscriptionSpecificInfo subscriptionSpecificInfo = null )
        {
            this.type = type;
            productId = id;
            androidConfig = new ProductShopInfo( androidID == string.Empty ? id : androidID, priceInCents );
            iOSConfig = new ProductShopInfo( iOSId == string.Empty ? id : iOSId, priceInCents );

            if ( subscriptionSpecificInfo == null )
                subscriptionSpecificInfo = new SubscriptionSpecificInfo();
            this.subscriptionSpecificInfo = subscriptionSpecificInfo;
        }

        public IAPProductType Type => type;

        public string ProductId => productId;

        public string ShopId
        {
            get
            {
                var shopId = string.Empty;

                switch ( Application.platform )
                {
                    case RuntimePlatform.WindowsEditor:
                    case RuntimePlatform.IPhonePlayer:
                        shopId = iOSConfig.ProductId;
                        break;
                    case RuntimePlatform.Android:
                        shopId = androidConfig.ProductId;
                        break;
                }

                if ( string.IsNullOrEmpty( shopId ) )
                {
                    shopId = productId;
                }

                return shopId;
            }
        }

        public int PriceInCents
        {
            get
            {
                switch ( Application.platform )
                {
                    case RuntimePlatform.IPhonePlayer:
                        return iOSConfig.PriceInCents;
                    case RuntimePlatform.Android:
                        return androidConfig.PriceInCents;
                    case RuntimePlatform.OSXEditor:
                    case RuntimePlatform.WindowsEditor:
                    case RuntimePlatform.LinuxEditor:
                        return iOSConfig.PriceInCents > 0 ? iOSConfig.PriceInCents : androidConfig.PriceInCents;
                    default:
                        return 0;
                }
            }
        }

        public bool IsRestorable()
        {
            return type == IAPProductType.NonConsumable || type == IAPProductType.Subscription;
        }

        public void SetSubscriptionInfo( int androidTrialPeroid, int androidPeroid, int iOSTrialPeroid, int iOSPeroid )
        {
            subscriptionSpecificInfo.SetAndroidInfo( androidTrialPeroid, androidPeroid );
            subscriptionSpecificInfo.SetIOSInfo( iOSTrialPeroid, iOSPeroid );
        }
    }
}