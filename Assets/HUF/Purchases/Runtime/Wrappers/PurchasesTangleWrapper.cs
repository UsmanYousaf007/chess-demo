namespace HUF.Purchases.Runtime.Wrappers
{
    public static class PurchasesTangleWrapper
    {
        public static byte[] GooglePlayTangleData
        {
            get
            {
#if !HUF_LIBRARY_BUILD && ( UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS ) && !UNITY_2018
                return UnityEngine.Purchasing.Security.GooglePlayTangle.Data();
#else
                return new byte[0];
#endif
            }
        }

        public static byte[] AppleTangleData
        {
            get
            {
#if !HUF_LIBRARY_BUILD && ( UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS ) && !UNITY_2018
                return UnityEngine.Purchasing.Security.AppleTangle.Data();
#else
                return new byte[0];
#endif
            }
        }
    }
}