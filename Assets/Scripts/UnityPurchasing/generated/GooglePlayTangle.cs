// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("sAKBorCNhomqBsgGd42BgYGFgIN9IBAE/nyij/68Cbz5+VNwaoHZbhisY+5jZxMhgJrpDh9G9MDYTQC9Ey8dtFTrgYV9hF0jBdZaomnm1SmvCGwyttuVXZw+bnOudJhWYlN3mAKBj4CwAoGKggKBgYATXVqlJ3u9YnnrnscfvfIQIJWu8/+zF0gd67GS2B6GMMgv5LjCWo7btlnecAgOWgqvgLWlHjefls7ei7qmJ82fdD5T64CXoYLDzGSrltr53KV+LglVjowSQe0CrCVb1SpJJ1hDNH30Jud2PZ+wdCx9+a6cvLFuXWAh33+wvlrYrvqk6FfWLI8zK0sMZmWw9+OUqUwPrH7c3XMtNqGtIs35rbPlPcIHEcUCWejOHrsG94KDgYCB");
        private static int[] order = new int[] { 0,2,12,8,11,12,7,7,8,12,12,11,12,13,14 };
        private static int key = 128;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
