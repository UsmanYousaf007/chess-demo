#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("khEfECCSERoSkhEREIPNyjW36y2C0X2SPLXLRbrZt8jTpO1ktnfmrXsQBzESU1z0OwZKaUw17r6ZxR4cmj8QJTWOpw8GXk4bKja3XQ/krsM+ajR4x0a8H6O725z29SBncwQ53Ig8837z94OxEAp5no/WZFBI3ZAt8ul7DlePLWKAsAU+Y28jh9iNeyEgkhEyIB0WGTqWWJbnHRERERUQE4O/jSTEexEV7RTNs5VGyjL5dkW5P5j8oiZLBc0Mrv7jPuQIxvLD5wifPO5MTeO9pjE9sl1pPSN1rVKXgQJIjhagWL90KFLKHksmyU7gmJ7KDyDkvO1pPgwsIf7N8LFP7yAuykjtsICUbuwyH24smSxpacPg+hFJ/lWSyXhejiuWZxITERAR");
        private static int[] order = new int[] { 1,9,5,3,10,12,7,9,8,11,13,11,13,13,14 };
        private static int key = 16;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
