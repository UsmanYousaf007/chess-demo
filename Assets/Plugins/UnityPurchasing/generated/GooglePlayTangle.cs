#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("r/LC1iyucF0sbttuKyuBorhTC7zdfqwOD6H/5HN/8B8rf2E37xDVw01ipv6vK3xObmO8j7LzDa1ibIgKwf3PZoY5U1evVo/x1wSIcLs0B/vYfVJnd8zlTUQcDFlodPUfTabsgXwodjqFBP5d4fmZ3rS3YiUxRnueQArMVOIa/TZqEIhcCWSLDKLa3IjQU11SYtBTWFDQU1NSwY+Id/Wpb8p+sTyxtcHzUkg73M2UJhIKn9JvYtBTcGJfVFt41BrUpV9TU1NXUlF92r7gZAlHj07svKF8pkqEsIGlSjlSRXNQER62eUQIKw53rPzbh1xewJM/0H73iQf4m/WKkeavJvQ1pO+wqzlMFc1vIMLyR3whLWHFms85YxfQizoczGnUJVBRU1JT");
        private static int[] order = new int[] { 2,13,4,8,8,10,6,13,12,12,11,11,12,13,14 };
        private static int key = 82;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
