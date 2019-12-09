using UnityEngine;
using Prefs = UnityEngine.PlayerPrefs;

namespace HUF.Utils.PlayerPrefs 
{
    public static class HPlayerPrefs
    {
        public static bool HasKey(string key)
        {
            return Prefs.HasKey(key);
        }
        
        public static bool GetBool(string key, bool defaultValue = false)
        {
            return Prefs.GetInt(key, defaultValue ? 1 : 0) > 0;
        }

        public static void SetBool(string key, bool value)
        {
            Prefs.SetInt(key, value ? 1 : 0);
            
            if(!Application.isEditor)
                Prefs.Save();
        }

        public static int GetInt(string key, int defaultValue = 0)
        {
            return Prefs.GetInt(key, defaultValue);
        }

        public static void SetInt(string key, int value)
        {
            Prefs.SetInt(key, value);
            
            if(!Application.isEditor)
                Prefs.Save();
        }

        public static uint GetUInt(string key, uint defaultValue = 0)
        {
            if (HasKey(key))
            {
                var stringValue = Prefs.GetString(key);
                if (uint.TryParse(stringValue, out var value))
                    return value;
            }
            return defaultValue;
        }

        public static void SetUInt(string key, uint value)
        {
            Prefs.SetString(key, value.ToString("0"));
            
            if(!Application.isEditor)
                Prefs.Save();
        }

        public static long GetLong(string key, long defaultValue = 0)
        {
            if (HasKey(key))
            {
                var stringValue = Prefs.GetString(key);
                if (long.TryParse(stringValue, out var value))
                    return value;
            }
            return defaultValue;
        }

        public static void SetLong(string key, long value)
        {
            Prefs.SetString(key, value.ToString("0"));
            
            if(!Application.isEditor)
                Prefs.Save();
        }

        public static float GetFloat(string key, float defaultValue = 0.0f)
        {
            return Prefs.GetFloat(key, defaultValue);
        }

        public static void SetFloat(string key, float value)
        {
            Prefs.SetFloat(key, value);
            
            if(!Application.isEditor)
                Prefs.Save();
        }

        public static string GetString(string key, string defaultValue = "")
        {
            return Prefs.GetString(key, defaultValue);
        }

        public static void SetString(string key, string value)
        {
            Prefs.SetString(key, value);
            
            if(!Application.isEditor)
                Prefs.Save();
        }

        public static void DeleteKey(string key)
        {
            Prefs.DeleteKey(key);
            
            if(!Application.isEditor)
                Prefs.Save();
        }

        public static void DeleteAll()
        {
            Prefs.DeleteAll();
            
            if(!Application.isEditor)
                Prefs.Save();
        }
    }
}