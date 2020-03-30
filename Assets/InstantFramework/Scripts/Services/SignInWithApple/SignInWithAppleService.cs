/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using HUF.AuthSIWA.Runtime.API;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using UnityEngine.SignInWithApple;

namespace TurboLabz.InstantFramework
{
    public class SignInWithAppleService : ISignInWithAppleService
    {
        IPromise<SignInWithApple.CallbackArgs> promise;

        public IPromise<SignInWithApple.CallbackArgs> Login()
        {
            promise = new Promise<SignInWithApple.CallbackArgs>();
            HAuthSIWA.Service.ServiceComponent.Login(OnLogin);
            return promise;
        }

        private void OnLogin(SignInWithApple.CallbackArgs args)
        {
            promise.Dispatch(args);
            promise = null;
        }

        public IPromise<SignInWithApple.CallbackArgs> GetCredentialState()
        {
            promise = new Promise<SignInWithApple.CallbackArgs>();
            HAuthSIWA.Service.ServiceComponent.GetCredentialState(HAuthSIWA.Service.UserId, OnCredentialState);
            return promise;
        }

        private void OnCredentialState(SignInWithApple.CallbackArgs args)
        {
            promise.Dispatch(args);
            promise = null;
        }

        public bool IsSignedIn()
        {
            return IsSupported() && HAuthSIWA.Service.IsSignedIn;
        }

        public bool IsSupported()
        {
            return HAuthSIWA.IsSupported;
        }

        public string GetDisplayName()
        {
            return HAuthSIWA.DisplayName;
        }
    }
}