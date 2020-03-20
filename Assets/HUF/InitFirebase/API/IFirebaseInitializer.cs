using UnityEngine.Events;

namespace HUF.InitFirebase.API
{
    public interface IFirebaseInitializer
    {
        bool IsInitialized { get; }

        event UnityAction OnInitializationSuccess;
        event UnityAction OnInitializationFailure;

        void Init();
    }
}