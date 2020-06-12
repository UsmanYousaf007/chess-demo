using System;
using System.Globalization;
using HUF.Utils.Runtime._3rdParty.Blowfish;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;

// ReSharper disable once CheckNamespace
namespace HUF.Utils.Runtime.PlayerPrefs.Security
{
    using PlayerPrefs = UnityEngine.PlayerPrefs;

    public static partial class HSecureValueVault
    {
        const char SIGN_SEPARATOR = '×';
        const string KEY_PART_S = "f34";
        const int KEY_PART_C = 28;
        const byte KEY_PART_T = 3;

        static readonly BlowFish encryptor = new BlowFish($"{KEY_PART_C}{KEY_PART_S}{nameof(KEY_PART_T)}");

        static ISecureLiveFloat LoadLiveFloat( string key )
        {
            return new HSecureLiveValue( key, GetHashedFloat( PlayerPrefs.GetString( Encrypt( key ) ), key ) );
        }

        static ISecureLiveInt LoadLiveInt( string key )
        {
            return new HSecureLiveValue( key, GetHashedInt( PlayerPrefs.GetString( Encrypt( key ) ), key ) );
        }

        static ISecureLiveString LoadLiveString( string key )
        {
            return new HSecureLiveValue( key, GetHashedString( PlayerPrefs.GetString( Encrypt( key ) ), key ) );
        }

        static string Encrypt( string key, IConvertible value )
        {
            return encryptor.Encrypt_ECB( SignValue( key, value.ToString( CultureInfo.InvariantCulture ) ) );
        }

        static string Encrypt( string key )
        {
            return encryptor.Encrypt_ECB( key );
        }

        static string Decrypt( string key, string serialized )
        {
            return UnsignValue( key, encryptor.Decrypt_ECB( serialized ) );
        }

        static string SignValue( string key, string value )
        {
            return $"{key}{SIGN_SEPARATOR}{value}";
        }

        static string UnsignValue( string key, string signed )
        {
            string[] split = signed.Split( SIGN_SEPARATOR );

            if ( split.Length != 2 || key != split[0] )
            {
                OnSecurityBreach?.Invoke( key );
                return string.Empty;
            }

            return split[1];
        }

        static bool UnsignConfig( AbstractConfig config, string hashedValue, out string value )
        {
            value = Decrypt( config.ConfigId, hashedValue );
            return !string.IsNullOrEmpty( value );
        }

#if UNITY_EDITOR

        static string EncryptConfigValue( AbstractConfig config, string key, IConvertible value )
        {
            string signedValue = Encrypt( key, value );
            string result = Encrypt( config.ConfigId, signedValue );
            UnityEditor.EditorGUIUtility.systemCopyBuffer = result;
            return result;
        }
#endif

        static T CreateVariable<T>( string key, IConvertible initialValue ) where T : class, ISecureLiveValue
        {
            if ( key.Contains( SIGN_SEPARATOR.ToString() ) )
            {
                throw new ArgumentException($"You've unlocked \"Throwing the least expected exception possible\" achievement! Keys cannot contain {SIGN_SEPARATOR}");
            }
            T value = new HSecureLiveValue( key, initialValue ) as T;
            Save( value );
            return value;
        }

        static T RestoreHFrameworkValue<T>( string key, IConvertible old ) where T : class, ISecureLiveValue
        {
            PlayerPrefs.DeleteKey( key );
            return CreateVariable<T>( key, old );
        }

        static ISecureLiveString RestoreHFrameworkValue( string key, ICustomTransition old )
        {
            PlayerPrefs.DeleteKey( key );
            return CreateVariable<ISecureLiveString>( key, old.GetDeprecatedValue() );
        }

        static bool HasVariable( string key )
        {
            string hash = Encrypt( key );
            return PlayerPrefs.HasKey( hash );
        }

        static float GetHashedFloat( string serializedHash, string key )
        {
            if ( string.IsNullOrEmpty( serializedHash ) )
                return default;

            serializedHash = Decrypt( key, serializedHash );

            if ( float.TryParse( serializedHash, out float @base ) )
                return @base;

            OnSecurityBreach.Dispatch( key );

            return default;
        }

        static int GetHashedInt( string serializedHash, string key )
        {
            if ( string.IsNullOrEmpty( serializedHash ) )
                return default;

            serializedHash = Decrypt( key, serializedHash );

            if ( int.TryParse( serializedHash, out int @base ) )
                return @base;

            OnSecurityBreach.Dispatch( key );

            return default;
        }

        static string GetHashedString( string serializedHash, string key )
        {
            if ( string.IsNullOrEmpty( serializedHash ) )
                return default;

            return Decrypt( key, serializedHash );
        }

        public partial interface ISecureLiveValue
        {
            string HashedValue { get; }
            string Key { get; }
        }

        public interface ICustomTransition
        {
            string GetDeprecatedValue();
        }

        class HSecureLiveValue : ISecureLiveFloat, ISecureLiveInt, ISecureLiveString
        {
            string hashedValue;
            string hashedInitialValue;

            public string Key { get; }

            public string HashedValue
            {
                get => hashedValue;
                private set
                {
                    string newValue = value;

                    if ( string.Equals( newValue, hashedValue ) )
                        return;

                    hashedValue = value;

                    if ( !string.Equals( hashedValue, newValue ) )
                        OnSecurityBreach.Dispatch( Key );
                }
            }

            public HSecureLiveValue( string key, IConvertible initialValue )
            {
                Key = key;
                hashedValue = HSecureValueVault.Encrypt( key, initialValue );
                hashedInitialValue = HashedValue;
            }

            public void Reset( bool toInitial = false )
            {
                HashedValue = toInitial ? hashedInitialValue : string.Empty;
            }

            public void SetFromConfig( AbstractConfig config, string encryptedValue )
            {
                if ( UnsignConfig( config, encryptedValue, out string hashed ) )
                    HashedValue = hashed;
                else
                    HashedValue = hashedInitialValue;
            }

            void ISecureLiveFloat.Add( ISecureLiveFloat increment )
            {
                float newValue = GetFloat() + GetFloat( increment );
                HashedValue = Encrypt( newValue );
            }

            void ISecureLiveFloat.Add( ISecureLiveInt increment )
            {
                float newValue = GetFloat() + GetInt( increment );
                HashedValue = Encrypt( newValue );
            }

            void ISecureLiveFloat.Multiply( ISecureLiveFloat multiplier )
            {
                float newValue = GetFloat() * GetFloat( multiplier );
                HashedValue = Encrypt( newValue );
            }

            void ISecureLiveFloat.Multiply( ISecureLiveInt multiplier )
            {
                float newValue = GetFloat() * GetInt( multiplier );
                HashedValue = Encrypt( newValue );
            }

            void ISecureLiveFloat.Set( ISecureLiveFloat other )
            {
                Set( other );
            }

            void ISecureLiveInt.Increase()
            {
                HashedValue = Encrypt( GetInt() + 1 );
            }

            void ISecureLiveInt.Decrease()
            {
                HashedValue = Encrypt( GetInt() - 1 );
            }

            void ISecureLiveInt.Set( ISecureLiveInt other )
            {
                Set( other );
            }

            void ISecureLiveInt.Add( ISecureLiveInt increment )
            {
                int newValue = GetInt() + GetInt( increment );
                HashedValue = Encrypt( newValue );
            }

            void ISecureLiveInt.Multiply( ISecureLiveInt multiplier )
            {
                int newValue = GetInt() * GetInt( multiplier );
                HashedValue = Encrypt( newValue );
            }

            void ISecureLiveString.Replace( string newValue )
            {
                HashedValue = Encrypt( newValue );
            }

            bool ISecureLiveString.ValueEquals( ISecureLiveString other )
            {
                return string.Equals( DisplayValue( this ), DisplayValue( other ) );
            }

            bool ISecureLiveString.ValueEquals( string other )
            {
                return string.Equals( DisplayValue( this ), other );
            }

            void ISecureLiveString.Set( ISecureLiveString other )
            {
                Set( other );
            }

            void Set( ISecureLiveValue other )
            {
                HashedValue = other.HashedValue;
            }

            string Encrypt( IConvertible value )
            {
                return HSecureValueVault.Encrypt( Key, value );
            }

            int GetInt( ISecureLiveInt value ) => GetHashedInt( value.HashedValue, value.Key );
            float GetFloat( ISecureLiveFloat value ) => GetHashedFloat( value.HashedValue, value.Key );
            int GetInt() => GetInt( this );
            float GetFloat() => GetFloat( this );
        }
    }
}