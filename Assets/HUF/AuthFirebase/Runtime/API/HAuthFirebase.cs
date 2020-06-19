using System;
using HUF.Auth.Runtime.API;
using HUF.AuthFirebase.Runtime.Config;
using HUF.AuthFirebase.Runtime.Implementation;
using HUF.Utils.Runtime.Configs.API;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.AuthFirebase.Runtime.API
{
    public static class HAuthFirebase
    {
        static FirebaseAuthService service;
        static readonly string className = typeof(HAuthFirebase).Name;

        /// <summary>
        /// Returns UserName of currently signed-in user or empty string if user is not signed-in
        /// </summary>
        [PublicAPI]
        public static string UserName
        {
            get
            {
                if (service != null)
                {
                    return service.UserName;
                }

                LogServiceNotInitializedWarning("Can't get UserName");
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns PhotoUrl as System.Uri object of currently signed-in user or null if user is not signed-in
        /// </summary>
        [PublicAPI]
        public static Uri PhotoUrl
        {
            get
            {
                if (service != null)
                {
                    return service.PhotoUrl;
                }

                LogServiceNotInitializedWarning("Can't get PhotoUrl");
                return null;
            }
        }

        /// <summary>
        /// Returns Email of currently signed-in user or empty string if user is not signed-in
        /// </summary>
        [PublicAPI]
        public static string Email
        {
            get
            {
                if (service != null)
                {
                    return service.Email;
                }

                LogServiceNotInitializedWarning("Can't get Email");
                return string.Empty;
            }
        }

        /// <summary>
        /// Occurs when signing in to Firebase with Facebook credentials succeed <para />
        /// Bool parameter = true if it was the first login to Firebase through this Facebook account, false otherwise <para/>
        /// </summary>
        [PublicAPI]
        public static event UnityAction<bool> OnSignInWithFacebookSuccess
        {
            add
            {
                if (service != null)
                    service.OnSignInWithFacebookSuccess += value;
                else
                    LogServiceNotInitializedWarning("Adding listener to OnSignInWithFacebookSuccess failed");
            }
            remove
            {
                if (service != null)
                    service.OnSignInWithFacebookSuccess -= value;
                else
                    LogServiceNotInitializedWarning("Removing listener from OnSignInWithFacebookSuccess failed");
            }
        }

        /// <summary>
        /// Occurs when signing in to Firebase with Facebook credentials failed <para />
        /// </summary>
        [PublicAPI]
        public static event UnityAction OnSignInWithFacebookFailure
        {
            add
            {
                if (service != null)
                    service.OnSignInWithFacebookFailure += value;
                else
                    LogServiceNotInitializedWarning("Adding listener to OnSignInWithFacebookFailure failed");
            }
            remove
            {
                if (service != null)
                    service.OnSignInWithFacebookFailure -= value;
                else
                    LogServiceNotInitializedWarning("Removing listener from OnSignInWithFacebookFailure failed");
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void AutoInit()
        {
            var hasConfig = HConfigs.HasConfig<FirebaseAuthConfig>();
            if (hasConfig && HConfigs.GetConfig<FirebaseAuthConfig>().AutoInit)
            {
                Init();
            }
        }

        /// <summary>
        /// Use this method to initialize Firebase auth service
        /// </summary>
        [PublicAPI]
        public static void Init()
        {
            if (!HAuth.IsServiceRegistered(AuthServiceName.FIREBASE))
            {
                service = new FirebaseAuthService();
                HAuth.TryRegisterService(service);
            }
        }

        /// <summary>
        /// Use this method to initialize Firebase auth service
        /// </summary>
        /// <param name="callback">Callback invoked after initialization is finished regardless of the outcome</param>
        [PublicAPI]
        public static void Init( Action callback )
        {
            if ( callback == null )
            {
                Init();
                return;
            }

            if ( !HAuth.IsServiceRegistered( AuthServiceName.FIREBASE ) )
            {
                service = new FirebaseAuthService();
                HAuth.TryRegisterService( service, callback );
                return;
            }

            callback();
        }

        /// <summary>
        /// Use this method to Sign in to Firebase using Facebook access token
        /// </summary>
        /// <param name="accessToken">Facebook user access token</param>
        /// <returns>Returns TRUE if signing-in call succeed. Returns FALSE otherwise.</returns>
        [PublicAPI]
        public static bool SignInWithFacebook(string accessToken)
        {
            if (service != null)
            {
                return service.SignInWithFacebook(accessToken);
            }

            LogServiceNotInitializedWarning("SignInWithFacebookAccessToken failed");
            return false;
        }

        static void LogServiceNotInitializedWarning(string message)
        {
            Debug.LogWarning($"[{className}] {message}, service is not initialized");
        }
    }
}