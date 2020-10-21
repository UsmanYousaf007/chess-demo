#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("kb56InP3oJKyv2BTbi/Rcb6wVNYMj4GOvgyPhIwMj4+OHVNUqyl1s5zWEIg+xiHqtsxUgNW4V9B+BgBU5Y6Zr4zNwmqlmNT30qtwIAdbgIIdIRO6WuWPi3OKUy0L2FSsZ+jbJ74Mj6y+g4iHpAjGCHmDj4+Pi46NAaJw0tN9IzivoyzD96O96zPMCR8Wom3gbWkdL46U5wARSPrO1kMOs6D0quZZ2CKBPSVFAmhrvvntmqdCbHflkMkRs/weLpug/fG9GUYT5b9zLh4K8HKsgfCyB7L3911+ZI/XYAShjrurEDmRmMDQhbSoKcORejBdHE/jDKIrVdskRylWTTpz+ijpeDOhBmI8uNWbU5IwYH2gepZYbF15lssMV+bAELUI+YyNj46P");
        private static int[] order = new int[] { 4,1,6,5,8,8,13,12,10,12,13,13,12,13,14 };
        private static int key = 142;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
