using Firebase;
using Firebase.Extensions;
using HUF.InitFirebase.Runtime.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using UnityEngine.Events;

namespace HUF.InitFirebase.Runtime.Implementation
{
    public class FirebaseInitializer : IFirebaseInitializer
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(FirebaseInitializer) );

        FirebaseApp firebaseApp;

        public bool IsInitialized => firebaseApp != null;
        bool isInitializing = false;

        public event UnityAction OnInitializationSuccess;
        public event UnityAction OnInitializationFailure;

        public void Init()
        {
            if (IsInitialized || isInitializing)
                return;

            isInitializing = true;

            HLog.Log(logPrefix, "Firebase init start.");
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var status = task.Result;
                if (status == DependencyStatus.Available)
                {
                    InitializationSuccessCallback();
                }
                else
                {
                    HLog.LogError( logPrefix, $"Could not resolve all Firebase dependencies: {status}. " +
                                   "Firebase SDK not safe for usage.");
                    InitializationFailureCallback();
                }

                isInitializing = false;
            });
        }

        void InitializationSuccessCallback()
        {
            firebaseApp = FirebaseApp.DefaultInstance;
            OnInitializationSuccess.Dispatch();
            HLog.Log( logPrefix, "Firebase initialized" );
        }

        void InitializationFailureCallback()
        {
            OnInitializationFailure.Dispatch();
            HLog.LogError( logPrefix, "Unable to initialize Firebase" );
        }
    }
}