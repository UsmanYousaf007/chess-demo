using System;
using UnityEngine.Purchasing;
#if UNITY_PURCHASING

#endif

namespace HUF.Purchases.Runtime.API.Data
{
    public enum IAPProductType
    {
        Consumable = 0,
        NonConsumable = 1,
        Subscription = 2
    }

#if UNITY_PURCHASING
    static class ProductTypeExtensions
    {
        public static ProductType GetPurchasingType( this IAPProductType type )
        {
            switch (type)
            {
                case IAPProductType.Consumable:
                    return ProductType.Consumable;
                case IAPProductType.NonConsumable:
                    return ProductType.NonConsumable;
                case IAPProductType.Subscription:
                    return ProductType.Subscription;
                default:
                    throw new ArgumentOutOfRangeException( nameof(type), type, null );
            }
        }
    }
#endif
}
