using System;
using System.Globalization;
using HUF.Utils.Runtime._3rdParty.Blowfish;
using UnityEngine;

namespace HUF.Utils.Runtime.PlayerPrefs
{
    public class PlayerPrefsSaver
    {
        readonly BlowFish encryption;
        bool EncryptionEnabled => encryption != null;

        public PlayerPrefsSaver(BlowFish encryption = null)
        {
            this.encryption = encryption;
        }

        public bool HasKey(string key)
        {
            return HPlayerPrefs.HasKey(key);
        }

        public bool GetBool(string key, bool defaultValue)
        {
            if (EncryptionEnabled)
            {
                var encryptedBool = HPlayerPrefs.GetString(key);
                if (bool.TryParse(encryption.Decrypt_ECB(encryptedBool), out var result))
                    return result;

                return defaultValue;
            }

            return HPlayerPrefs.GetBool(key, defaultValue);
        }

        public void SetBool(string key, bool value = false)
        {
            if (!TrySetEncryptedValue(key, value.ToString()))
                HPlayerPrefs.SetBool(key, value);
        }

        bool TrySetEncryptedValue(string key, string value)
        {
            if (!EncryptionEnabled)
                return false;
            var encryptedValue = encryption.Encrypt_ECB(value);
            HPlayerPrefs.SetString(key, encryptedValue);
            return true;
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            if (EncryptionEnabled)
            {
                var encryptedInt = HPlayerPrefs.GetString(key);
                if (int.TryParse(encryption.Decrypt_ECB(encryptedInt), out var result))
                    return result;

                return defaultValue;
            }

            return HPlayerPrefs.GetInt(key, defaultValue);
        }

        public void SetInt(string key, int value)
        {
            if (!TrySetEncryptedValue(key, value.ToString()))
                HPlayerPrefs.SetInt(key, value);
        }

        public long GetLong(string key, long defaultValue = 0L)
        {
            if (EncryptionEnabled)
            {
                var encryptedLong = HPlayerPrefs.GetString(key);
                if (long.TryParse(encryption.Decrypt_ECB(encryptedLong), out var result))
                    return result;

                return defaultValue;
            }

            return HPlayerPrefs.GetLong(key, defaultValue);
        }

        public void SetLong(string key, long value)
        {
            if (!TrySetEncryptedValue(key, value.ToString()))
                HPlayerPrefs.SetLong(key, value);
        }

        public float GetFloat(string key, float defaultValue = 0.0f)
        {
            if (EncryptionEnabled)
            {
                var encryptedFloat = HPlayerPrefs.GetString(key, defaultValue.ToString(CultureInfo.InvariantCulture));
                if (float.TryParse(encryption.Decrypt_ECB(encryptedFloat), NumberStyles.Float,
                    CultureInfo.InvariantCulture, out var result))
                {
                    return result;
                }

                return defaultValue;
            }

            return HPlayerPrefs.GetFloat(key, defaultValue);
        }

        public void SetFloat(string key, float value)
        {
            if (!TrySetEncryptedValue(key, value.ToString(CultureInfo.InvariantCulture)))
                HPlayerPrefs.SetFloat(key, value);
        }

        public string GetString(string key, string defaultValue)
        {
            if (EncryptionEnabled)
                return encryption.Decrypt_ECB(HPlayerPrefs.GetString(key));

            return HPlayerPrefs.GetString(key, defaultValue);
        }

        public void SetString(string key, string value)
        {
            if (!TrySetEncryptedValue(key, value))
                HPlayerPrefs.SetString(key, value);
        }

        public T GetCustom<T>(string key, T defaultValue)
        {
            try
            {
                if (EncryptionEnabled)
                {
                    var decryptedValue = encryption.Decrypt_ECB(HPlayerPrefs.GetString(key));
                    return JsonUtility.FromJson<T>(decryptedValue);
                }

                return JsonUtility.FromJson<T>(HPlayerPrefs.GetString(key));
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return defaultValue;
            }
        }

        public void SetCustom<T>(string key, T value)
        {
            var json = JsonUtility.ToJson(value);
            if (EncryptionEnabled)
                json = encryption.Encrypt_ECB(json);
            HPlayerPrefs.SetString(key, json);
        }
    }
}