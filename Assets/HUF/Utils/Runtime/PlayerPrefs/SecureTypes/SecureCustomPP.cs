using System;
using HUF.Utils.Runtime._3rdParty.Blowfish;
using HUF.Utils.Runtime.PlayerPrefs.Security;
using UnityEngine;

namespace HUF.Utils.Runtime.PlayerPrefs.SecureTypes
{
#pragma warning disable 0809
    [Obsolete("Use HSecureValueVault instead")]
    public sealed class SecureCustomPP<T> : CustomPP<T>, HSecureValueVault.ICustomTransition
    {
        readonly BlowFish encryption;

        [Obsolete( "Use HSecureValueVault instead" )]
        public SecureCustomPP(string key, BlowFish encryption = null, T defaultValue = default) : base(key, defaultValue)
        {
            this.encryption = encryption;
        }

        [Obsolete( "Use HSecureValueVault instead" )]
        protected override void SetValue(T value)
        {
            HPlayerPrefs.SetString(key, SecurePPHelper.EncryptString(JsonUtility.ToJson(value), encryption));
        }

        [Obsolete( "Use HSecureValueVault instead" )]
        protected override T GetValue()
        {
            return JsonUtility.FromJson<T>(SecurePPHelper.DecryptString(HPlayerPrefs.GetString(key), encryption));
        }

        public string GetDeprecatedValue()
        {
            return SecurePPHelper.DecryptString( HPlayerPrefs.GetString( key ), encryption );
        }
    }
#pragma warning restore 0809
}