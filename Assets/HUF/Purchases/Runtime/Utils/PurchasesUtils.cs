using UnityEngine;

namespace HUF.Purchases.Runtime.Utils
{
    public static class PurchasesUtils
    {
        public static string GetStoreName()
        {
            switch (Application.platform)
            {
#if UNITY_PURCHASING
                case RuntimePlatform.Android:
                    return UnityEngine.Purchasing.GooglePlay.Name;
                case RuntimePlatform.IPhonePlayer:
                    return UnityEngine.Purchasing.AppleAppStore.Name;
#endif
                default:
                    return string.Empty;
            }
        }
    }
}
