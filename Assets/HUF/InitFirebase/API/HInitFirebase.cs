using HUF.InitFirebase.Implementation;
using JetBrains.Annotations;
using UnityEngine.Events;

namespace HUF.InitFirebase.API
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
        /// Initialize Firebase. Initialization is asynchronous and could be done later. <para />
        /// After calling Init method you should wait for <para />
        /// <see cref="OnInitializationSuccess"/> if initialization complete with success <para />
        /// or for <see cref="OnInitializationFailure"/> if initialization fail.
        /// </summary>
        [PublicAPI]
        public static void Init()
        {
            Initializer.Init();
        }
    }
}
