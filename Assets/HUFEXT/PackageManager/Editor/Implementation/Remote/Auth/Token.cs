using System.IO;
using HUFEXT.PackageManager.Editor.API.Models;
using HUFEXT.PackageManager.Editor.Implementation.Remote.Data;
using HUFEXT.PackageManager.Editor.Implementation.Remote.Requests;
using HUFEXT.PackageManager.Editor.Utils;
using HUFEXT.PackageManager.Editor.Utils.Helpers;
using HUFEXT.PackageManager.Editor.Views;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Cache = HUFEXT.PackageManager.Editor.Utils.Cache;

namespace HUFEXT.PackageManager.Editor.Implementation.Remote.Auth
{
    public class Token : IAuthModel
    {
        [field: SerializeField]
        public string DeveloperID { private set; get; } = string.Empty;

        [field: SerializeField]
        public string AccessKey { private set; get; } = string.Empty;

        [field: SerializeField]
        public bool IsPolicyAccepted { private set; get; } = false;

        [field: SerializeField]
        public ScopeList Scopes { set; get; } = new ScopeList();

        public static bool Exists => File.Exists( Registry.Cache.TOKEN_FILE_PATH );
        
        public bool IsValid => Exists &&
                               !string.IsNullOrEmpty( DeveloperID ) &&
                               !string.IsNullOrEmpty( AccessKey ) &&
                               IsPolicyAccepted;

        public bool IsValidated => IsValid && Scopes.Items.Count > 0;
        
        public static Token Create( string devId, string accessKey, bool policy )
        {
            var auth = new Token()
            {
                DeveloperID = devId,
                AccessKey = accessKey,
                IsPolicyAccepted = policy
            };
            Cache.Save( Registry.Cache.TOKEN_FILE_PATH, auth, Cache.Policy.EncodedFile );
            return auth;
        }
        
        [CanBeNull]
        public static Token LoadExistingToken()
        {
            if ( !Exists )
            {
                return null;
            }
            
            var token = new Token();
            Cache.Load( token, Registry.Cache.TOKEN_FILE_PATH, Cache.Policy.EncodedFile );
            return token;
        }
        
        public bool Initialize()
        {
            return Cache.Load( this, Registry.Cache.TOKEN_FILE_PATH, Cache.Policy.EncodedFile );
        }

        public void Validate( UnityAction<bool> onComplete )
        {
            if ( !IsValid )
            {
                if ( Exists )
                {
                    Invalidate();
                }
                Debug.LogError( "[PackageManager] Token is not valid. Can't be validated." );
                onComplete?.Invoke( false );
                return;
            }
            
            var route = RoutingScheme.CreateRoute( RoutingScheme.API.AllowedScopes ).Value;
            var request = new BaseRequest( route, ( response ) =>
            {
                if ( response.status == RequestStatus.Failure )
                {
                    Debug.LogError( "[PackageManager] Authorization token validation failed." );
                    onComplete?.Invoke( false );
                    return;
                }

                Scopes = JsonHelper.FromArray<ScopeList>( response.Text );

                if ( Scopes.Items.Count > 0 )
                {
                    Cache.Save( Registry.Cache.TOKEN_FILE_PATH, this, Cache.Policy.EncodedFile );
                    Debug.Log( "[PackageManager] Authorization token validated successfully." );
                }
                
                onComplete?.Invoke( Scopes.Items.Count > 0 );
            } );
            
            request.Send();
        }

        public void Invalidate()
        {
            Cache.Remove( Registry.Cache.TOKEN_FILE_PATH, Cache.Policy.EncodedFile );
            DeveloperID = AccessKey = string.Empty;
            IsPolicyAccepted = false;
            PackageManagerWindow.SetForceCloseFlag();
            AssetDatabase.Refresh();
        }
    }
}
