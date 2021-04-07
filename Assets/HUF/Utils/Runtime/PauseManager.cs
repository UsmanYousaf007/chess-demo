using System;
using HUF.Utils.Runtime.Extensions;
using JetBrains.Annotations;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace HUF.Utils.Runtime
{
    public class PauseManager : HSingleton<PauseManager>
    {
#pragma warning disable 0067
        /// <summary>
        /// Raised when the game is paused.
        /// </summary>
        [PublicAPI]
        public event Action<bool> OnAppPause;

        /// <summary>
        /// Raised when the focus of the game changes.
        /// </summary>
        [PublicAPI]
        public event Action<bool> OnApplicationFocusChange;

        /// <summary>
        /// Raised when screen orientation of the game changes.
        /// </summary>
        [PublicAPI]
        public event Action<ScreenOrientation> OnScreenOrientationChange;

        /// <summary>
        /// Raised when the game is paused.
        /// </summary>
        [PublicAPI]
        public event Action OnBackButtonPress;
#pragma warning restore 0067

        bool currentFocus;
        ScreenOrientation lastScreenOrientation = ScreenOrientation.Portrait;

        void OnApplicationPause( bool pauseStatus )
        {
            OnAppPause.Dispatch( pauseStatus );
            OnApplicationFocus( !pauseStatus );
        }

        void OnApplicationFocus( bool hasFocus )
        {
            if ( currentFocus != hasFocus )
            {
                currentFocus = hasFocus;
                OnApplicationFocusChange.Dispatch( currentFocus );
            }
        }

#if UNITY_EDITOR
        void HandlePauseState( PauseState state )
        {
            if ( state == PauseState.Paused )
            {
                OnApplicationFocus( false ); //clicking pause button in the editor doesn't raise OnApplicationFocus
                OnApplicationPause( true );
            }
            else
                OnApplicationPause( false );
        }
#endif
        
        void Awake()
        {
            lastScreenOrientation = Screen.orientation;
#if UNITY_EDITOR
            EditorApplication.pauseStateChanged += HandlePauseState;
#endif
        }

        void Update()
        {
            if ( lastScreenOrientation != Screen.orientation )
            {
                lastScreenOrientation = Screen.orientation;
                OnScreenOrientationChange.Dispatch( lastScreenOrientation );
            }
#if UNITY_ANDROID
            if (Input.GetKeyDown(KeyCode.Escape))
                OnBackButtonPress.Dispatch();
#endif
        }
    }
}