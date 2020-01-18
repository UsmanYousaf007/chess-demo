using HUF.Utils.Extensions;
using UnityEngine.Events;

namespace HUF.Utils
{
    public class PauseManager : HSingleton<PauseManager>
    {
        public event UnityAction<bool> OnAppPause;

        void OnApplicationPause(bool pauseStatus)
        {
            OnAppPause.Dispatch(pauseStatus);
        }
    }
}