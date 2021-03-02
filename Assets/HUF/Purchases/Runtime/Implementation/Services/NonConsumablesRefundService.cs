#if UNITY_PURCHASING
using System;
using System.Collections.Generic;
using System.Linq;
using HUF.Purchases.Runtime.API.Data;
using HUF.Purchases.Runtime.API.Services;
using HUF.Utils.Runtime._3rdParty.Blowfish;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.PlayerPrefs.SecureTypes;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

namespace HUF.Purchases.Runtime.Implementation.Services
{
    public class NonConsumablesRefundService : IRefundService
    {
        readonly SecureStringArrayPP boughtProducts;
        readonly IEnumerable<IProductInfo> products;
        readonly bool isRefundEnabled;

        public event Action<IProductInfo> OnRefundSuccess;

        internal NonConsumablesRefundService(IEnumerable<IProductInfo> products, bool isRefundEnabled, BlowFish blowFish)
        {
            this.products = products;
            this.isRefundEnabled = isRefundEnabled;
            boughtProducts = new SecureStringArrayPP("RefundService.boughtProducts", ';', blowFish);
        }

        public void ProcessProductsLoading(IEnumerable<Product> loadedProducts)
        {
            if (!isRefundEnabled)
                return;

            var boughtProductsList = GetBoughtProductsList();
            foreach (var product in loadedProducts)
            {
                var productId = product.definition.id;
                if (!product.hasReceipt && boughtProductsList.Contains(productId))
                {
                    var productInfo = TryGetNonConsumableProductInfo(productId);
                    if (productInfo != null)
                    {
                        RefundItem(productInfo);
                        boughtProductsList.Remove(productId);
                    }
                }
            }
            boughtProducts.Value = boughtProductsList.ToArray();
        }

        List<string> GetBoughtProductsList()
        {
            return boughtProducts.Value == null ? new List<string>() : boughtProducts.Value.ToList();
        }

        public bool IsNonConsumableBought(string productId)
        {
            var boughtProductsList = GetBoughtProductsList();
            return boughtProductsList.Any(q => q.Equals(productId));
        }

        public bool ProcessTransaction(Product product, IPurchaseReceipt receipt)
        {
            var productId = product.definition.id;
            var productInfo = TryGetNonConsumableProductInfo(productId);
            if (productInfo == null)
                return false;

            var boughtProductsList = GetBoughtProductsList();
            if (boughtProductsList.Any(q => q.Equals(productId)))
            {
                if (isRefundEnabled && IsRefunded(receipt))
                {
                    RefundItem(productInfo);
                    boughtProductsList.Remove(productId);
                    boughtProducts.Value = boughtProductsList.ToArray();
                }
            }
            else
            {
                boughtProducts.Add(productId);
                return true;
            }

            return false;
        }

        bool IsRefunded(IPurchaseReceipt receipt)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                if (receipt is GooglePlayReceipt googleReceipt)
                {
                    return googleReceipt.purchaseState == GooglePurchaseState.Refunded;
                }
            }

            return false;
        }

        void RefundItem(IProductInfo productInfo)
        {
            if (productInfo != null)
                OnRefundSuccess.Dispatch(productInfo);
        }

        IProductInfo TryGetNonConsumableProductInfo(string productId)
        {
            var productInfo = products.FirstOrDefault(x => x.ProductId == productId);
            if (productInfo != null && productInfo.Type == IAPProductType.NonConsumable)
            {
                return productInfo;
            }
            return null;
        }
    }
}
#endif