#define HUF_AUTH

using HUF.Auth.Implementation;
using JetBrains.Annotations;
using UnityEngine.Events;

namespace HUF.Auth.API
{
    public static class HAuth
    {
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
        /// Occurs when Sign In completed with success. <para />
        /// Auth service name is available in parameter <para/>
        /// Supported service names can be found as constants in <see cref="AuthServiceName"/>
        /// </summary>
        [PublicAPI]
        public static event UnityAction<string> OnSignInSuccess
        {
            add => AuthModel.OnSignInSuccess += value;
            remove => AuthModel.OnSignInSuccess -= value;
        }

        /// <summary>
        /// Occurs when Sign In fails. <para />
        /// Auth service name is available in parameter <para/>
        /// Supported service names can be found as constants in <see cref="AuthServiceName"/>
        /// </summary>
        [PublicAPI]
        public static event UnityAction<string> OnSignInFailure
        {
            add => AuthModel.OnSignInFailure += value;
            remove => AuthModel.OnSignInFailure -= value;
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

        /// <summary>
        /// Tries to register auth service for future use. The service is automatically initialized <para />
        /// after registration. <para/>
        /// Supported service names can be found as constants in <see cref="AuthServiceName"/>
        /// </summary>
        /// <returns>If service is registered correctly returns TRUE. Returns FALSE otherwise.</returns>
        [PublicAPI]
        public static bool TryRegisterService(IAuthService service)
        {
            return AuthModel.TryRegisterService(service);
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
    }
}