using UnityEngine.Events;

namespace HUF.Auth.Runtime.API
{
    public interface IAuthModel
    {
        event UnityAction<string> OnInitialized;
        event UnityAction<string, bool> OnSignIn;
        event UnityAction<string, AuthSignInResult> OnSignInResult;
        event UnityAction<string> OnSignOutComplete;

        bool TryRegisterService( IAuthService service );

        bool IsServiceRegistered( string serviceName );
        bool IsInitialized( string serviceName );
        bool SignIn( string serviceName );
        bool SignOut( string serviceName );
        bool IsSignedIn( string serviceName );
        string GetUserId( string serviceName );
    }
}