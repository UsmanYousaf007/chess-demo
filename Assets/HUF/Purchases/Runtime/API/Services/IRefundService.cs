#if UNITY_PURCHASING
using System;
using System.Collections.Generic;
using HUF.Purchases.Runtime.API.Data;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

namespace HUF.Purchases.Runtime.API.Services
{
    public interface IRefundService
    {
        event Action<IProductInfo> OnRefundSuccess;

        bool IsNonConsumableBought( string productId );
        bool ProcessTransaction( Product product, IPurchaseReceipt receipt );
        void ProcessProductsLoading( IEnumerable<Product> loadedProducts );
    }
}
#endif
