using UnityEngine.Events;

namespace HUF.InitFirebase.Runtime.API
{
    public interface IFirebaseInitializer
    {
        bool IsInitialized { get; }
        string FCM_TOKEN { get;  }

        event UnityAction OnInitializationSuccess;
        event UnityAction OnInitializationFailure;

        void Init();
    }
}