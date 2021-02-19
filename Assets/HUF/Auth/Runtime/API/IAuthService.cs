using System;
using UnityEngine.Events;

namespace HUF.Auth.Runtime.API
{
    public interface IAuthService
    {
        string Name { get; }
        bool IsSignedIn { get; }
        string UserId { get; }
        bool IsInitialized { get; }

        event Action<string> OnInitialized;
        event Action OnInitializationFailure;
        event Action<string, AuthSignInResult> OnSignInResult;
        event Action<string> OnSignOutComplete;

        void Init();
        bool SignIn();
        bool SignOut();
    }
}