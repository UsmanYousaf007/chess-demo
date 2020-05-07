using System;
using HUF.Auth.Runtime.Implementation;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEngine.Events;

namespace HUF.Auth.Runtime.API
{
    public static class HAuth
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(HAuth) );
        static IAuthModel authModel;
        static IAuthModel AuthModel => authModel ?? (authModel = new AuthModel());

        /// <summary>
        /// Occurs when service initialization succeed <para />
        /// Auth service name is available in parameter <para/>
        /// Supported service names can be found as constants in <see cref="AuthServiceName"/>
        /// </summary>
        [PublicAPI]
        public static event UnityAction<string> OnInitialized
        {
            add => AuthModel.OnInitialized += value;
            remove => AuthModel.OnInitialized -= value;
        }

        /// <summary>
        /// Occurs when Sign In completed. <para />
        /// Auth service name is available in parameter <para/>
        /// Bool parameter defines if sign in is ended with success <para/>
        /// Supported service names can be found as constants in <see cref="AuthServiceName"/>
        /// </summary>
        [PublicAPI]
        public static event UnityAction<string, bool> OnSignIn
        {
            add => AuthModel.OnSignIn += value;
            remove => AuthModel.OnSignIn -= value;
        }

        /// <summary>
        /// Occurs when Sign Out is completed. Auth service name is passed as parameter. <para/>
        /// Supported service names can be found as constants in <see cref="AuthServiceName"/>
        /// </summary>
        [PublicAPI]
        public static event UnityAction<string> OnSignOutSuccess
        {
            add => AuthModel.OnSignOutComplete += value;
            remove => AuthModel.OnSignOutComplete -= value;
        }

        static HAuth()
        {
            OnInitialized += LogServiceInitialization;
        }

        /// <summary>
        /// Tries to register auth service for future use. The service is automatically initialized <para />
        /// after registration. <para/>
        /// Supported service names can be found as constants in <see cref="AuthServiceName"/>
        /// </summary>
        /// <returns>If service is registered correctly returns TRUE. Returns FALSE otherwise.</returns>
        [PublicAPI]
        public static bool TryRegisterService(IAuthService service)
        {
            bool isPossible = AuthModel.TryRegisterService(service);

            if ( !isPossible )
                HLog.LogError( logPrefix, $"Registration of service {service.Name} not possible" );
            return isPossible;
        }

        /// <summary>
        /// Tries to register auth service for future use. The service is automatically initialized <para />
        /// after registration. <para/>
        /// Supported service names can be found as constants in <see cref="AuthServiceName"/>
        /// </summary>
        /// <param name="service">Service to register</param>
        /// <param name="callback">Callback invoked after initialization is finished regardless of the outcome</param>
        [PublicAPI]
        public static void TryRegisterService( IAuthService service, Action callback )
        {
            if ( callback == null )
            {
                TryRegisterService( service );
                return;
            }

            void CheckInitialization( string unused )
            {
                HandleInitializationEnd();
            }

            void HandleInitializationEnd()
            {
                service.OnInitializationFailure -= HandleInitializationEnd;
                service.OnInitialized -= CheckInitialization;
                callback();
            }

            service.OnInitialized += CheckInitialization;
            service.OnInitializationFailure += HandleInitializationEnd;

            if ( TryRegisterService( service ) )
                return;

            HandleInitializationEnd();
        }


        /// <summary>
        /// Checks if service with given name is already registered <para/>
        /// Supported service names can be found as constants in <see cref="AuthServiceName"/>
        /// </summary>
        [PublicAPI]
        public static bool IsServiceRegistered(string serviceName)
        {
            return AuthModel.IsServiceRegistered(serviceName);
        }

        /// <summary>
        /// Checks if service with given name is initialized <para/>
        /// Supported service names can be found as constants in <see cref="AuthServiceName"/>
        /// </summary>
        [PublicAPI]
        public static bool IsServiceInitialized(string serviceName)
        {
            return AuthModel.IsInitialized(serviceName);
        }

        /// <summary>
        /// Tries to sign in into service with given name. <para/>
        /// Supported service names can be found as constants in <see cref="AuthServiceName"/>
        /// </summary>
        /// <returns>Returns TRUE if user could be signed in. Returns FALSE otherwise.</returns>
        [PublicAPI]
        public static bool SignIn(string serviceName)
        {
            return AuthModel.SignIn(serviceName);
        }

        /// <summary>
        /// Tries to sign out from service with given name. <para/>
        /// Supported service names can be found as constants in <see cref="AuthServiceName"/>
        /// </summary>
        /// <returns>Returns TRUE if user could be signed out. Returns FALSE otherwise.</returns>
        [PublicAPI]
        public static bool SignOut(string serviceName)
        {
            return AuthModel.SignOut(serviceName);
        }

        /// <summary>
        /// Return info if user is signed in in service with given name. <para/>
        /// Supported service names can be found as constants in <see cref="AuthServiceName"/>
        /// </summary>
        [PublicAPI]
        public static bool IsSignedIn(string serviceName)
        {
            return AuthModel.IsSignedIn(serviceName);
        }

        /// <summary>
        /// Return user id in service with given name. <para />
        /// If user is not signed in yet in chosen service it will return empty string. <para/>
        /// Supported service names can be found as constants in <see cref="AuthServiceName"/>
        /// </summary>
        [PublicAPI]
        public static string GetUserId(string serviceName)
        {
            return AuthModel.GetUserId(serviceName);
        }

        static void LogServiceInitialization( string serviceName )
        {
            HLog.Log( logPrefix, $"Service {serviceName} initialized" );
        }
    }
}