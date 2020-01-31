using BlowFishCS;
using UnityEngine;

namespace HUF.Utils.PlayerPrefs.SecureTypes  
{
    public sealed class SecureCustomPP<T> : CustomPP<T>
    {
        readonly BlowFish encryption;

        public SecureCustomPP(string key, BlowFish encryption = null, T defaultValue = default) : base(key, defaultValue)
        {
            this.encryption = encryption;
        }

        protected override void SetValue(T value)
        {
            HPlayerPrefs.SetString(key, SecurePPHelper.EncryptString(JsonUtility.ToJson(value), encryption));
        }

        protected override T GetValue()
        {
            return JsonUtility.FromJson<T>(SecurePPHelper.DecryptString(HPlayerPrefs.GetString(key), encryption));
        }
    }
}