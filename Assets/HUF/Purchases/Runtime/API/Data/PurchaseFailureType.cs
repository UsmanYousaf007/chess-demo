#if UNITY_PURCHASING
using UnityEngine.Purchasing;

#endif

namespace HUF.Purchases.Runtime.API.Data
{
    public enum PurchaseFailureType
    {
        PurchasingUnavailable = 0,
        ExistingPurchasePending = 1,
        ProductUnavailable = 2,
        SignatureInvalid = 3,
        UserCancelled = 4,
        PaymentDeclined = 5,
        DuplicateTransaction = 6,
        Unknown = 7,
        NonConsumableAlreadyPurchased = 8,
        HuuugeIAPServerError = 9,
        OldSubscriptionNotBought = 10,
        NewSubscriptionNotAvailable = 11
    }

#if UNITY_PURCHASING
    static class PurchaseFailureReasonExtensions
    {
        public static PurchaseFailureType GetFailureType( this PurchaseFailureReason reason )
        {
            var type = PurchaseFailureType.Unknown;

            switch( reason )
            {
                case PurchaseFailureReason.PurchasingUnavailable:
                    type = PurchaseFailureType.PurchasingUnavailable;
                    break;
                case PurchaseFailureReason.ExistingPurchasePending:
                    type = PurchaseFailureType.ExistingPurchasePending;
                    break;
                case PurchaseFailureReason.ProductUnavailable:
                    type = PurchaseFailureType.ProductUnavailable;
                    break;
                case PurchaseFailureReason.SignatureInvalid:
                    type = PurchaseFailureType.SignatureInvalid;
                    break;
                case PurchaseFailureReason.UserCancelled:
                    type = PurchaseFailureType.UserCancelled;
                    break;
                case PurchaseFailureReason.PaymentDeclined:
                    type = PurchaseFailureType.PaymentDeclined;
                    break;
                case PurchaseFailureReason.DuplicateTransaction:
                    type = PurchaseFailureType.DuplicateTransaction;
                    break;
            }
            return type;
        }
    }
#endif
}
