using HUF.Utils.Runtime.Extensions;
using UnityEngine.Events;

namespace HUF.Utils.Runtime
{
    public class PauseManager : HSingleton<PauseManager>
    {
        public event UnityAction<bool> OnAppPause;
        public event UnityAction<bool> OnApplicationFocusChange;

        bool currentFocus;

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
    }
}