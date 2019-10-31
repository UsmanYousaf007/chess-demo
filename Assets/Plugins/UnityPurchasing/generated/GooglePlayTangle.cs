#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("Ko+glYU+F7+27v6rmoYH7b9UHnOy+D6mEOgPxJjieq77lnn+UCgueo8oTBKW+7V9vB5OU45UuHZCc1e4XQAwJN5cgq/enCmc2dlzUEqh+U5CWcu+5z+d0jAAtY7T35M3aD3Lkb+QVAxd2Y68nJFOfUAB/1+Qnnr4L4xe/P1TDRaBjQLt2Y2TxR3iJzHLoLeBouPsRIu2+tn8hV4OKXWurCKhr6CQIqGqoiKhoaAzfXqFB1udMmHNIowFe/UKaQd4YxRd1AbHVh04jEPOQ0czAaC6yS4/ZtTg+G0gnZAioYKQraapiiboJletoaGhpaCjMw89lHTLoaVdpH0DJfZ6gknG9QmO2oTId/YMrxMLayxGRZDXw7SJbOUiecjuPpsm16KjoaCh");
        private static int[] order = new int[] { 3,6,11,11,7,7,13,7,13,9,12,11,13,13,14 };
        private static int key = 160;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
