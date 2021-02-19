using System;
using System.Collections.Generic;
using HUF.Auth.Runtime.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.Auth.Runtime.Implementation
{
    public class AuthModel : IAuthModel
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(AuthModel) );

        readonly Dictionary<string, IAuthService> services = new Dictionary<string, IAuthService>();

        public event Action<string> OnInitialized;
        public event Action<string, bool> OnSignIn;
        public event Action<string, AuthSignInResult> OnSignInResult;
        public event Action<string> OnSignOutComplete;

        public bool TryRegisterService( IAuthService service )
        {
            if ( IsServiceRegistered( service ) )
            {
                HLog.LogWarning( logPrefix, $"{service.Name} is already added to the model." );
                return false;
            }

            services.Add( service.Name, service );
            service.OnInitialized += Initialized;
            service.OnSignInResult += HandleSignIn;
            service.OnSignOutComplete += SignOutCompete;
            service.Init();
            return true;
        }

        bool IsServiceRegistered( IAuthService service )
        {
            return IsServiceRegistered( service.Name );
        }

        public bool IsServiceRegistered( string serviceName )
        {
            return services.ContainsKey( serviceName );
        }

        void Initialized( string serviceName )
        {
            OnInitialized.Dispatch( serviceName );
        }

        void HandleSignIn( string serviceName, AuthSignInResult result )
        {
            OnSignInResult.Dispatch( serviceName, result );
            OnSignIn.Dispatch( serviceName, result == AuthSignInResult.Success );
        }

        void SignOutCompete( string serviceName )
        {
            OnSignOutComplete.Dispatch( serviceName );
        }

        public bool SignIn( string serviceName )
        {
            var service = GetService( serviceName );
            return service != null && service.SignIn();
        }

        public bool SignOut( string serviceName )
        {
            var service = GetService( serviceName );
            return service != null && service.SignOut();
        }

        public bool IsSignedIn( string serviceName )
        {
            var service = GetService( serviceName );
            return service != null && service.IsSignedIn;
        }

        public string GetUserId( string serviceName )
        {
            var service = GetService( serviceName );
            return service != null ? service.UserId : string.Empty;
        }

        public bool IsInitialized( string serviceName )
        {
            var service = GetService( serviceName );
            return service != null && service.IsInitialized;
        }

        IAuthService GetService( string serviceName )
        {
            if ( !IsServiceRegistered( serviceName ) )
            {
                HLog.LogWarning( logPrefix, $"Can't find service {serviceName}" );
                return null;
            }

            return services[serviceName];
        }
    }
}