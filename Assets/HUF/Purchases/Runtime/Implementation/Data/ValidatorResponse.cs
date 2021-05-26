using HUF.Utils.Runtime.Extensions;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

namespace HUF.Purchases.Runtime.Implementation.Data
{
    public struct ValidatorResponse
    {
        public bool IsValidPurchase =>
            responseCode == 200 && ( subscriptionResponse != null || requestId.IsNotEmpty() );

        public ProductType Type => product.definition.type;

        public Product product;
        public IPurchaseReceipt receipt;
        public long responseCode;
        public string requestId;
        public SubscriptionResponse subscriptionResponse;
        public string responseError;
    }
}