using UnityEngine.Events;

namespace HUF.Auth.API
{
    public interface IAuthService
    {
        string Name { get; }
        bool IsSignedIn { get; }
        string UserId { get; }
        bool IsInitialized { get; }
        
        event UnityAction<string> OnInitialized;
        event UnityAction<string> OnSignInSuccess;
        event UnityAction<string> OnSignInFailure;
        event UnityAction<string> OnSignOutComplete;

        void Init();
        bool SignIn();
        bool SignOut();
    }
}