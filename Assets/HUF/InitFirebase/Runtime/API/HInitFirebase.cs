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
        /// Returns whether Firebase is initialized.
        /// </summary>
        [PublicAPI]
        public static bool IsInitialized => Initializer.IsInitialized;

        /// <summary>
        /// Raised when Firebase initialization finishes successfully.
        /// </summary>
        [PublicAPI]
        public static event UnityAction OnInitializationSuccess
        {
            add => Initializer.OnInitializationSuccess += value;
            remove => Initializer.OnInitializationSuccess -= value;
        }

        /// <summary>
        /// Raised when Firebase initialization fails.
        /// </summary>
        [PublicAPI]
        public static event UnityAction OnInitializationFailure
        {
            add => Initializer.OnInitializationFailure += value;
            remove => Initializer.OnInitializationFailure -= value;
        }

        /// <summary>
        /// <para>Initializes Firebase.</para>
        /// <para>The initialization is asynchronous so
        /// subscribing to <see cref="OnInitializationSuccess"/>
        /// and <see cref="OnInitializationFailure"/> is recommended.</para>
        /// </summary>
        [PublicAPI]
        public static void Init()
        {
            Initializer.Init();
        }

        /// <summary>
        /// Initializes Firebase. The initialization is asynchronous.
        /// </summary>
        /// <param name="callback">A callback invoked after the initialization finishes regardless of the outcome.</param>
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
