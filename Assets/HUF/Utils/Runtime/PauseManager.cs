using HUF.Utils.Runtime.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.Utils.Runtime
{
    public class PauseManager : HSingleton<PauseManager>
    {
        public event UnityAction<bool> OnAppPause;
        public event UnityAction<bool> OnApplicationFocusChange;
        public event UnityAction<ScreenOrientation> OnScreenOrientationChange;
        
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
        
        void Awake()
        {
            lastScreenOrientation = Screen.orientation;
        }

        void Update()
        {
            if (lastScreenOrientation != Screen.orientation)
            {
                lastScreenOrientation = Screen.orientation;
                OnScreenOrientationChange.Dispatch(lastScreenOrientation);
            }
        }
    }
}