using System;
using UnityEngine;

namespace HUF.Purchases.Runtime.Implementation.Data
{
    [Serializable]
    public class ProductShopInfo
    {
        [SerializeField] string productId = default;
        [SerializeField] int priceInCents = default;

        public string ProductId => productId;
        public int PriceInCents => priceInCents;

        public ProductShopInfo( string id, int price = 0 )
        {
            productId = id;
            priceInCents = price;
        }
    }
}