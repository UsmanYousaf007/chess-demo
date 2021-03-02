#if UNITY_PURCHASING
using System;
using HUF.Purchases.Runtime.API.Data;
using UnityEngine.Purchasing;

namespace HUF.Purchases.Runtime.API.Services
{
    public interface IPriceConversionService
    {
        event Action<Product> OnGetConversionEnd;

        void TryGetConversion( Product product );
        void LoadPriceData( IPurchasesService purchasesService );
        PriceConversionData GetConversionData();
        void TryGetPriceConversionData( Action<PriceConversionResponse> response, string currency );
    }
}
#endif
