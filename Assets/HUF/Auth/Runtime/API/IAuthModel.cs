using System;
using UnityEngine.Events;

namespace HUF.Auth.Runtime.API
{
    public interface IAuthModel
    {
        event Action<string> OnInitialized;
        event Action<string, bool> OnSignIn;
        event Action<string, AuthSignInResult> OnSignInResult;
        event Action<string> OnSignOutComplete;

        bool TryRegisterService( IAuthService service );

        bool IsServiceRegistered( string serviceName );
        bool IsInitialized( string serviceName );
        bool SignIn( string serviceName );
        bool SignOut( string serviceName );
        bool IsSignedIn( string serviceName );
        string GetUserId( string serviceName );
    }
}