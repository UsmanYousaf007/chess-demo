using System;
using HUF.InitFirebase.Runtime.Implementation;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEngine.Events;

namespace HUF.InitFirebase.Runtime.API
{
    public static class HInitFirebase
    {
        static IFirebaseInitializer initializer;
        static IFirebaseInitializer Initializer => initializer ?? (initializer = new FirebaseInitializer());

        /// <summary>
        /// Returns TRUE if Firebase is already initialized correctly.
        /// </summary>
        [PublicAPI]
        public static bool IsInitialized => Initializer.IsInitialized;

        /// <summary>
        /// Occurs when Firebase initialization finish with success.
        /// </summary>
        [PublicAPI]
        public static event UnityAction OnInitializationSuccess
        {
            add => Initializer.OnInitializationSuccess += value;
            remove => Initializer.OnInitializationSuccess -= value;
        }

        /// <summary>
        /// Occurs when Firebase initialization fail.
        /// </summary>
        [PublicAPI]
        public static event UnityAction OnInitializationFailure
        {
            add => Initializer.OnInitializationFailure += value;
            remove => Initializer.OnInitializationFailure -= value;
        }

        /// <summary>
        /// Initialize Firebase. Initialization is asynchronous so
        /// after calling Init method you should wait for <para />
        /// <see cref="OnInitializationSuccess"/> if initialization complete with success <para />
        /// or for <see cref="OnInitializationFailure"/> if initialization fail.
        /// </summary>
        [PublicAPI]
        public static void Init()
        {
            Initializer.Init();
        }

        /// <summary>
        /// Initialize Firebase. Initialization is asynchronous and could be done later. <para />
        /// </summary>
        /// <param name="callback">Callback invoked after initialization is finished regardless of the outcome</param>
        public static void Init( Action callback )
        {
            void HandleInitializationEnd()
            {
                OnInitializationFailure -= HandleInitializationEnd;
                OnInitializationSuccess -= HandleInitializationEnd;
                callback();
            }

            OnInitializationFailure += HandleInitializationEnd;
            OnInitializationSuccess += HandleInitializationEnd;

            try
            {
                Init();
            }
            catch ( Exception exception )
            {
                HLog.LogError( new HLogPrefix( nameof(HInitFirebase) ), exception.ToString() );
                HandleInitializationEnd();
            }
        }
    }
}
