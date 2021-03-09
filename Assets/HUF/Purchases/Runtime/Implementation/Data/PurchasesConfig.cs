using System.Collections.Generic;
using System.Linq;
using HUF.Purchases.Runtime.API.Data;
using HUF.Utils.Runtime.Attributes;
using HUF.Utils.Runtime.Configs.API;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.Purchases.Runtime.Implementation.Data
{
    [CreateAssetMenu(fileName = "PurchasesConfig.asset", menuName = "HUF/PurchasesUnity/Config")]
    public partial class PurchasesConfig : AbstractConfig
    {
        [SerializeField] List<ProductInfo> products = new List<ProductInfo>();
        [SerializeField] bool enableLocalVerification = true;
        [SerializeField] bool enableEncryption = false;
        [SerializeField] float interruptedPaymentWaitTime = 5f;

        [Header("Refund")]
#pragma warning disable 0414
        [Warning(45.0f), SerializeField, UsedImplicitly] string refundWarningMessage = "Refund service works only when local verification is enabled";
#pragma warning restore 0414
        [SerializeField] bool enableRefundService = true;


        [Header("Subscriptions")]
        [SerializeField] string blowFishSecurityKey = string.Empty;


        [Header("Price Conversion")]
        [SerializeField] bool enableDownloadPriceConversion = false;
        [SerializeField] bool useInternalConversionServer = true;
        [SerializeField] string conversionApiInternalServerURL = "https://3cpy7uewck.execute-api.eu-west-1.amazonaws.com";
        [SerializeField] bool forceDownloadOnBuy = false;
        [Tooltip("How long data will be cached, in minutes")]
        [SerializeField] float conversionDataCacheTime = 1440;
        [Tooltip("How long request will try to get data, in seconds")]
        [SerializeField] int downloadRequestTimeout = 10;
        [SerializeField] string conversionApiExternalServerURL = "http://data.fixer.io";
        [SerializeField] string externalServerApiKey = "API Key";

        [Header("Huuuge IAP Server")]
        [SerializeField] bool enableHuuugeServerVerification = false;
        [SerializeField] bool forceProdHuuugeServerVerification = false;
        [SerializeField] string huuugeIAPServerTokenDebug = string.Empty;
        [SerializeField] string huuugeIAPServerTokenProduction = string.Empty;
        [SerializeField] string huuugeIAPServerURLDebug = "https://sandbox.iap-verification.huuuge.net/";
        [SerializeField] string huuugeIAPServerURLProduction = "https://iap-verification.huuuge.net/";
        [SerializeField] int huuugeIAPServerAttempts = 3;

        [Header("Tests")]
        [SerializeField] bool showDebugMenuInEditor = true;
        [SerializeField] bool editorDefaultPurchaseSuccess = true;
        [SerializeField] bool editorSubscriptionsAlwaysActive = true;
        [SerializeField] bool editorSubscriptionsAlwaysInTrialMode = false;
        [SerializeField] string priceConversionCurrency = "PLN";


        public IEnumerable<ProductInfo> Products => products;
        public bool IsLocalVerificationEnabled => enableLocalVerification || enableHuuugeServerVerification;
        public bool IsEncryptionEnabled => enableEncryption;
        public bool IsRefundServiceEnabled => IsLocalVerificationEnabled && enableRefundService;
        public float InterruptedPaymentWaitTime => interruptedPaymentWaitTime;


        public string BlowFishKey => blowFishSecurityKey;

        public bool IsDownloadPriceConversionEnabled => enableDownloadPriceConversion;
        public bool IsUsingInternalConversionServer => useInternalConversionServer;
        public string GetConversionApiInternalServerURL => conversionApiInternalServerURL;
        public string GetConversionApiExternalServerURL => conversionApiExternalServerURL;
        public string GetExternalServerApiKey => externalServerApiKey;
        public bool IsForceDownloadEnabled => forceDownloadOnBuy;
        public float CacheTime => conversionDataCacheTime;
        public int GetDownloadRequestTimeout => downloadRequestTimeout;

        public string HuuugeIAPServerURL => IsHuuugeIAPServerProd ? huuugeIAPServerURLProduction : huuugeIAPServerURLDebug;
        public int HuuugeIAPServerAttempts => huuugeIAPServerAttempts;
        public bool IsHuuugeServerVerificationEnabled => enableHuuugeServerVerification;
        public bool IsHuuugeIAPServerProd => forceProdHuuugeServerVerification || !Debug.isDebugBuild;
        public string HuuugeIAPServerToken => IsHuuugeIAPServerProd ? huuugeIAPServerTokenProduction : huuugeIAPServerTokenDebug;

        public bool ShowDebugMenuInEditor => showDebugMenuInEditor;
        public bool EditorDefaultPurchaseSuccess => editorDefaultPurchaseSuccess;
        public bool EditorSubscriptionsAlwaysActive => editorSubscriptionsAlwaysActive;
        public bool EditorSubscriptionsAlwaysInTrialMode => editorSubscriptionsAlwaysInTrialMode;
        public string PriceConversionCurrency => priceConversionCurrency;

        public IProductInfo GetProductInfo(string productId)
        {
            return products.FirstOrDefault(p => p.ProductId == productId);
        }

        public void ApplyProducts( IEnumerable<ProductInfo> productList, bool replaceCurrentProducts = true )
        {
            if ( replaceCurrentProducts )
            {
                products.Clear();
            }

            products.AddRange( productList );
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if ( !enableLocalVerification && enableRefundService )
                enableRefundService = false;
        }
    }
}
