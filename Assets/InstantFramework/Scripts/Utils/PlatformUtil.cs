using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
namespace TurboLabz.TLUtils {
    public static class PlatformUtil
    {
        public static Platform GetPlatform()
        {
#if UNITY_IOS
            return Platform.ios;


#elif UNITY_ANDROID
            return Platform.Android;
#endif
        }

        public static string AppendPlatform(this string str)
        {
            string suffix = "_" + GetPlatform().ToString().ToLower();
            if (!str.EndsWith(suffix))
                return str + suffix;
            else
                return str;
        }

        public static string RemovePlatfrom(this string str)
        {
            string substring = "_" + GetPlatform().ToString().ToLower();
            if (str.EndsWith(substring))
                return str.Substring(0, str.Length - substring.Length);
            else
                return str;
        }

        public static bool IsCurrentPlatformSuffixAppended(string str)
        {
            string substring = "_" + GetPlatform().ToString().ToLower();
            return str.EndsWith(substring);
                
        }
    }
}