using Firebase;
using Firebase.Extensions;
using HUF.InitFirebase.API;
using HUF.Utils.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.InitFirebase.Implementation
{
    public class FirebaseInitializer : IFirebaseInitializer
    {
        readonly string logPrefix;
        FirebaseApp firebaseApp;

        public bool IsInitialized => firebaseApp != null;
        bool isInitializing = false;
        
        public event UnityAction OnInitializationSuccess;
        public event UnityAction OnInitializationFailure;

        public FirebaseInitializer()
        {
            logPrefix = GetType().Name;
        }

        public void Init()
        {
            if (IsInitialized || isInitializing)
                return;

            isInitializing = true;

            Debug.Log($"[{logPrefix}] Firebase init start.");
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var status = task.Result;
                if (status == DependencyStatus.Available)
                {
                    InitializationSuccessCallback();
                }
                else
                {
                    Debug.LogError($"[{logPrefix}] Could not resolve all Firebase dependencies: {status}. " +
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
        }

        void InitializationFailureCallback()
        {
            OnInitializationFailure.Dispatch();
        }
    }
}