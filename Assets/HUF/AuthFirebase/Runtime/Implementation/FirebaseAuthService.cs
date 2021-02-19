using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using HUF.Auth.Runtime.API;
using HUF.InitFirebase.Runtime;
using HUF.InitFirebase.Runtime.API;
using HUF.InitFirebase.Runtime.Config;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using UnityEngine.Events;

namespace HUF.AuthFirebase.Runtime.Implementation
{
    public class FirebaseAuthService : IAuthService
    {
        const int MS_FOR_FIRST_FACEBOOK_LOGIN = 1000;

        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(FirebaseAuthService) );

        FirebaseAuth firebaseAuth;

        public string Name => AuthServiceName.FIREBASE;
        public bool IsInitialized => firebaseAuth != null;
        public bool IsSignedIn => IsInitialized && firebaseAuth.CurrentUser != null && !isSigningWithFacebookInProgress;
        public string UserId => IsSignedIn ? firebaseAuth.CurrentUser.UserId : string.Empty;
        public string UserName => IsSignedIn ? firebaseAuth.CurrentUser.DisplayName : string.Empty;
        public string Email => IsSignedIn ? firebaseAuth.CurrentUser.Email : string.Empty;
        public Uri PhotoUrl => IsSignedIn ? firebaseAuth.CurrentUser.PhotoUrl : null;

        public event Action<string> OnInitialized;
        public event Action OnInitializationFailure;
        public event Action<string, AuthSignInResult> OnSignInResult;
        public event Action<string> OnSignOutComplete;
        public event Action<bool> OnSignInWithFacebookSuccess;
        public event Action OnSignInWithFacebookFailure;

        bool isSigningWithFacebookInProgress;

        public void Init()
        {
            if ( IsInitialized )
                return;

            HLog.Log( logPrefix, "Firebase auth init start." );

            if ( HInitFirebase.IsInitialized )
            {
                CreateFirebaseRef();
            }
            else
            {
                HInitFirebase.OnInitializationSuccess += InitializationSuccessCallback;
                HInitFirebase.OnInitializationFailure += InitializationFailureCallback;

                if ( !HConfigs.HasConfig<HFirebaseConfig>() || HConfigs.GetConfig<HFirebaseConfig>().AutoInit )
                    HInitFirebase.Init();
            }
        }

        void InitializationSuccessCallback()
        {
            HInitFirebase.OnInitializationSuccess -= InitializationSuccessCallback;
            HInitFirebase.OnInitializationFailure -= InitializationFailureCallback;
            CreateFirebaseRef();
        }

        void InitializationFailureCallback()
        {
            OnInitializationFailure.Dispatch();
            HLog.Log( logPrefix, "Initialization fail." );
        }

        void CreateFirebaseRef()
        {
            var firebaseApp = FirebaseApp.DefaultInstance;

            if ( firebaseApp == null )
            {
                HLog.LogWarning( logPrefix, "Firebase App is not initialized." );
                OnInitializationFailure.Dispatch();
                return;
            }

            firebaseAuth = FirebaseAuth.GetAuth( firebaseApp );
            OnInitialized.Dispatch( Name );

            if ( IsSignedIn )
            {
                HLog.Log( logPrefix, "Initialization succeed. User was already authenticated." );
                OnSignInResult.Dispatch( Name, AuthSignInResult.Success );
            }
            else
            {
                HLog.Log( logPrefix, "Initialization succeed." );
            }
        }

        public bool SignIn()
        {
            if ( !IsInitialized )
            {
                HLog.LogWarning( logPrefix, "SignIn failed, firebase auth is not initialized yet." );
                return false;
            }

            if ( IsSignedIn )
            {
                HLog.Log( logPrefix, "SignIn failed, user is already authenticated." );
                return false;
            }

            firebaseAuth.SignInAnonymouslyAsync().ContinueWithOnMainThread( SignInCallback );
            return true;
        }

        void SignInCallback( Task<FirebaseUser> task )
        {
            if ( task.IsFaulted || task.IsCanceled )
            {
                var failReason = GetTaskFailReason( task );
                HLog.LogWarning( logPrefix, $"Authentication fail - reason {failReason}" );

                OnSignInResult.Dispatch( Name,
                    task.IsCanceled ? AuthSignInResult.Cancelled : AuthSignInResult.UnspecifiedFailure );
            }
            else
            {
                HLog.Log( logPrefix, "Authentication succeed." );
                OnSignInResult.Dispatch( Name, AuthSignInResult.Success );
            }
        }

        string GetTaskFailReason( Task task )
        {
            if ( task.Exception != null && task.Exception.Message.IsNotEmpty() )
            {
                return task.Exception.GetFullErrorMessage();
            }

            return task.IsCanceled ? "Authentication task was cancelled" : "Authentication task failed";
        }

        public bool SignOut()
        {
            if ( !IsInitialized )
            {
                HLog.LogWarning( logPrefix, "SignOut failed, firebase auth is not initialized yet." );
                return false;
            }

            if ( !IsSignedIn )
            {
                HLog.LogWarning( logPrefix, "SignOut failed, user is not signed in." );
                return false;
            }

            firebaseAuth.SignOut();
            OnSignOutComplete.Dispatch( Name );
            return true;
        }

        public bool SignInWithFacebook( string accessToken )
        {
            if ( !IsInitialized )
            {
                HLog.LogWarning( logPrefix,
                    "SignInWithFacebookAccessToken failed, firebase auth is not initialized yet." );
                return false;
            }

            if ( accessToken.IsNullOrEmpty() )
            {
                HLog.LogError( logPrefix, "SignInWithFacebookAccessToken failed, accessToken is empty" );
                return false;
            }

            isSigningWithFacebookInProgress = true;

            if ( firebaseAuth.CurrentUser != null )
            {
                OnSignOutComplete.Dispatch( Name );
            }

            var credential = FacebookAuthProvider.GetCredential( accessToken );

            firebaseAuth.SignInAndRetrieveDataWithCredentialAsync( credential )
                .ContinueWithOnMainThread( SignInWithFacebookCallback );
            return true;
        }

        void SignInWithFacebookCallback( Task<SignInResult> task )
        {
            if ( task.IsCanceled || task.IsFaulted )
            {
                var failReason = GetTaskFailReason( task );
                HLog.LogWarning( logPrefix, $"SignInWithFacebook fail - reason {failReason}" );
                isSigningWithFacebookInProgress = false;
                OnSignInWithFacebookFailure.Dispatch();

                OnSignInResult.Dispatch( Name,
                    task.IsCanceled ? AuthSignInResult.Cancelled : AuthSignInResult.UnspecifiedFailure );
            }
            else
            {
                var isFirstLogin = IsFirstTimeFacebookLogin( task.Result );
                HLog.Log( logPrefix, $"SignInWithFacebook succeed, isFirstLogin: {isFirstLogin}" );
                isSigningWithFacebookInProgress = false;
                OnSignInWithFacebookSuccess.Dispatch( isFirstLogin );
                OnSignInResult.Dispatch( Name, AuthSignInResult.Success );
            }
        }

        bool IsFirstTimeFacebookLogin( SignInResult result )
        {
            var meta = result.User.Metadata;
            return meta.LastSignInTimestamp - meta.CreationTimestamp < MS_FOR_FIRST_FACEBOOK_LOGIN;
        }
    }
}