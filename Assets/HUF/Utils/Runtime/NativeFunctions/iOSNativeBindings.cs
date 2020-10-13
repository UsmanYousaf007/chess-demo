using System.Runtime.InteropServices;

namespace HUF.Utils.Runtime.NativeFunctions
{
    public class iOSNativeBindings
    {
#if UNITY_IOS
        [DllImport( "__Internal" )]
        public static extern string GetSettingsURL();
#endif
    }
}