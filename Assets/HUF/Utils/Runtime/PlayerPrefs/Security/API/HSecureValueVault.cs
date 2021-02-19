using System;
using HUF.Utils.Runtime.Configs.API;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace HUF.Utils.Runtime.PlayerPrefs.Security
{
    using PlayerPrefs = UnityEngine.PlayerPrefs;

    public static partial class HSecureValueVault
    {
        /// <summary>
        /// Raised when it is impossible to deserialize some value likely because of tampering with it.
        /// The event reports key of such value.
        /// </summary>
        [PublicAPI] public static event Action<string> OnSecurityBreach;

        /// <summary>
        /// Creates a secure float.
        /// </summary>
        /// <param name="key">A unique key that identifies the variable.</param>
        /// <param name="initialValue">A value the variable is initialized with if it does not exist yet.</param>
        /// <param name="isHFrameworkTransition">Whether it is a value used in HFramework to be transitioned to HUF Security system.</param>
        /// <returns>Secure float hook that represents stored value (if it was previously saved) or <paramref name="initialValue"/> if the value does not exist in the storage. </returns>
        [PublicAPI]
        public static ISecureLiveFloat CreateVariable( string key, float initialValue, bool isHFrameworkTransition = false )
        {
            if ( HasVariable( key ) )
                return LoadLiveFloat( key );

            if ( isHFrameworkTransition )
                return RestoreHFrameworkValue<ISecureLiveFloat>( key, initialValue );

            return CreateVariable<ISecureLiveFloat>( key, initialValue );
        }

        /// <summary>
        /// Creates a secure int.
        /// </summary>
        /// <param name="key">A unique key that identifies the variable.</param>
        /// <param name="initialValue">A value the variable is initialized with if it does not exist yet.</param>
        /// <param name="isHFrameworkTransition">Whether it is a value used in HFramework to be transitioned to HUF Security system.</param>
        /// <returns>Secure int hook that represents stored value (if it was previously saved) or <paramref name="initialValue"/> if the value does not exist in the storage. </returns>
        [PublicAPI]
        public static ISecureLiveInt CreateVariable( string key, int initialValue, bool isHFrameworkTransition = false )
        {
            if ( HasVariable( key ) )
                return LoadLiveInt( key );

            if ( isHFrameworkTransition )
                return RestoreHFrameworkValue<ISecureLiveInt>( key, initialValue );

            return CreateVariable<ISecureLiveInt>( key, initialValue );
        }

        /// <summary>
        /// Creates a secure string.
        /// </summary>
        /// <param name="key">A unique key that identifies the variable.</param>
        /// <param name="initialValue">A value the variable is initialized with if it does not exist yet.</param>
        /// <param name="old">Value used in HFramework to be transitioned to HUF Security system.
        /// <para>Keep in mind that if a custom class was used to store numerical values it is advised to use <see cref="ISecureLiveFloat"/> and <see cref="ISecureLiveInt"/> for them to protect the values not only in device storage but also in memory.</para></param>
        /// <returns>Secure string hook that represents stored value (if it was previously saved) or <paramref name="initialValue"/> if the value does not exist in the storage. </returns>
        [PublicAPI]
        public static ISecureLiveString CreateVariable( string key, string initialValue, ICustomTransition old = null )
        {
            if ( HasVariable( key ) )
                return LoadLiveString( key );

            if ( old != null )
                return RestoreHFrameworkValue( key, old );

            return CreateVariable<ISecureLiveString>( key, initialValue );
        }

        /// <summary>
        /// Securely stores an <see cref="ISecureLiveValue"/> for future use.
        /// </summary>
        /// <param name="value"> Secure value to save.</param>
        [PublicAPI]
        public static void Save( ISecureLiveValue value )
        {
            PlayerPrefs.SetString( Encrypt( value.Key ), value.HashedValue );
        }

        /// <summary>
        /// Removes value from storage and frees its key.
        /// </summary>
        /// <param name="value">Value to dispose.</param>
        [PublicAPI]
        public static void Dispose( ISecureLiveValue value )
        {
            PlayerPrefs.DeleteKey( Encrypt( value.Key ) );
        }

        /// <summary>
        /// Displays string representation of an <see cref="ISecureLiveValue"/>.
        /// </summary>
        /// <param name="liveValue">A secure value to display.</param>
        /// <returns>String representation of a secure value.</returns>
        [PublicAPI]
        public static string DisplayValue( ISecureLiveValue liveValue )
        {
            return Decrypt( liveValue.Key, liveValue.HashedValue );
        }

#if UNITY_EDITOR
        /// <summary>
        /// Creates a value that can be used in a (remote) config. It also copies it to a clipboard.
        /// </summary>
        /// <param name="config">Config that will be used to store the value.</param>
        /// <param name="key">Key that will be used to identify the value (See <seealso cref="CreateVariable{T}"/>).</param>
        /// <param name="value">Value to be encrypted.</param>
        /// <returns></returns>
        [PublicAPI]
        public static string ConvertForConfig( AbstractConfig config, string key, IConvertible value )
        {
            return EncryptConfigValue( config, key, value );
        }
#endif
        /// <summary>
        /// Shared interface across all secure values
        /// </summary>
        [PublicAPI]
        public partial interface ISecureLiveValue
        {
            /// <summary>
            /// Resets stored value.
            /// </summary>
            /// <param name="toInitial">If true resets to initial value passed in constructor, if false, resets to type's default value.</param>
            [PublicAPI]
            void Reset( bool toInitial = false );

            /// <summary>
            /// Loads a value from (remote) config. Will reset to initial value on security breach.
            /// </summary>
            /// <param name="config">Config holding the value.</param>
            /// <param name="encryptedValue">Encrypted value stored by the config.</param>
            [PublicAPI]
            void SetFromConfig( AbstractConfig config, string encryptedValue );
        }

        /// <summary>
        /// Secure representation of float value
        /// </summary>
        [PublicAPI]
        public interface ISecureLiveFloat : ISecureLiveValue
        {
            /// <summary>
            /// Increments stored value by <paramref name="increment"/>.
            /// </summary>
            /// <param name="increment">A secure value to be added.</param>
            [PublicAPI]
            void Add( ISecureLiveFloat increment );

            /// <summary>
            /// Increments stored value by <paramref name="increment"/>.
            /// </summary>
            /// <param name="increment">A secure value to be added.</param>
            [PublicAPI]
            void Add( ISecureLiveInt increment );

            /// <summary>
            /// Multiplies stored value by <paramref name="multiplier"/>.
            /// </summary>
            /// <param name="multiplier">A secure value to multiply by.</param>
            [PublicAPI]
            void Multiply( ISecureLiveFloat multiplier );

            /// <summary>
            /// Multiplies stored value by <paramref name="multiplier"/>.
            /// </summary>
            /// <param name="multiplier">A secure value to multiply by.</param>
            [PublicAPI]
            void Multiply( ISecureLiveInt multiplier );

            /// <summary>
            /// Sets stored value to other.
            /// </summary>
            [PublicAPI]
            void Set( ISecureLiveFloat other );
        }

        /// <summary>
        /// Secure representation of int value
        /// </summary>
        [PublicAPI]
        public interface ISecureLiveInt : ISecureLiveValue
        {
            /// <summary>
            /// Increments stored value by <paramref name="increment"/>.
            /// </summary>
            /// <param name="increment">A secure value to be added.</param>
            [PublicAPI]
            void Add( ISecureLiveInt increment );

            /// <summary>
            /// Multiplies stored value by <paramref name="multiplier"/>.
            /// </summary>
            /// <param name="multiplier">A secure value to multiply by.</param>
            [PublicAPI]
            void Multiply( ISecureLiveInt multiplier );

            /// <summary>
            /// Increments stored value by 1.
            /// </summary>
            [PublicAPI]
            void Increase();

            /// <summary>
            /// Decrements stored value by 1.
            /// </summary>
            [PublicAPI]
            void Decrease();

            /// <summary>
            /// Sets stored value to other.
            /// </summary>
            [PublicAPI]
            void Set( ISecureLiveInt other );
        }

        /// <summary>
        /// Secure representation of string value. Can be used to keep all kinds of data serialized to json/xml/etc.
        /// </summary>
        [PublicAPI]
        public interface ISecureLiveString : ISecureLiveValue
        {
            /// <summary>
            /// Replaces stored value with <paramref name="newValue"/>.
            /// </summary>
            /// <param name="newValue">A secure value to multiply by.</param>
            [PublicAPI]
            void Replace( string newValue );

            /// <summary>
            /// Checks if values of two strings are the same.
            /// </summary>
            /// <param name="other">String to compare to.</param>
            [PublicAPI]
            bool ValueEquals( ISecureLiveString other );

            /// <summary>
            /// Checks if values of two strings are the same.
            /// </summary>
            /// <param name="other">String to compare to.</param>
            [PublicAPI]
            bool ValueEquals( string other );

            /// <summary>
            /// Sets stored value to other.
            /// </summary>
            [PublicAPI]
            void Set( ISecureLiveString other );
        }
    }
}