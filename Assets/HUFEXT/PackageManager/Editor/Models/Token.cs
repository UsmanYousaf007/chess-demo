using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace HUFEXT.PackageManager.Editor.Models
{
    [Serializable]
    internal class Scope
    {
        public string name = string.Empty;
    }
    
    [Serializable]
    public class Token
    {
        [SerializeField] string id;
        [SerializeField] string key;
        [SerializeField] bool isSigned;
        
        public static bool Exists => EditorPrefs.HasKey( Keys.CACHE_AUTH_TOKEN_KEY );
        internal bool CanBeSigned => Exists && !string.IsNullOrEmpty( id ) && !string.IsNullOrEmpty( key );
        public static bool IsValid
        {
            get
            {
                var token = new Token();
                return token.CanBeSigned && token.isSigned;
            }
        }
        public static string ID => new Token().id;

        private Token()
        {
            Core.Registry.Load( Keys.CACHE_AUTH_TOKEN_KEY, this, Core.CachePolicy.EncodedEditorPrefs );
        }

        public static bool CreateUnsignedToken( string id, string key )
        {
            return Core.Registry.Save( Keys.CACHE_AUTH_TOKEN_KEY, new Token { id = id, key = key, isSigned = false }, Core.CachePolicy.EncodedEditorPrefs );
        }

        internal static bool CreateSignedToken()
        {
            var token = new Token();
            if( token.CanBeSigned )
            {
                token.isSigned = true;
                Core.Registry.Save( Keys.CACHE_AUTH_TOKEN_KEY, token, Core.CachePolicy.EncodedEditorPrefs );
            }
            return Exists;
        }

        public static UnityWebRequest CreateSignedRequest( string route )
        {
            var token = new Token();
            
            if ( !token.CanBeSigned )
            {
                Invalidate();
                return null;
            }

            var request = UnityWebRequest.Get( route );
            request.SetRequestHeader( "x-developer-id", token.id );
            request.SetRequestHeader( "x-api-key", token.key );
            return request;
        }

        public static void Invalidate()
        {
            Core.Registry.Remove( Keys.CACHE_AUTH_TOKEN_KEY );
        }
    }
}