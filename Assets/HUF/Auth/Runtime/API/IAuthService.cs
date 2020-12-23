using UnityEngine.Events;

namespace HUF.Auth.Runtime.API
{
    public interface IAuthService
    {
        string Name { get; }
        bool IsSignedIn { get; }
        string UserId { get; }
        bool IsInitialized { get; }

        event UnityAction<string> OnInitialized;
        event UnityAction OnInitializationFailure;
        event UnityAction<string, AuthSignInResult> OnSignInResult;
        event UnityAction<string> OnSignOutComplete;

        void Init();
        bool SignIn();
        bool SignOut();
    }
}