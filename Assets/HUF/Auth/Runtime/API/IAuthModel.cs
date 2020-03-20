using UnityEngine.Events;

namespace HUF.Auth.API
{
    public interface IAuthModel
    {
        event UnityAction<string> OnInitialized;
        event UnityAction<string> OnSignInSuccess;
        event UnityAction<string> OnSignInFailure;
        event UnityAction<string> OnSignOutComplete;

        bool TryRegisterService(IAuthService service);

        bool IsServiceRegistered(string serviceName);
        bool IsInitialized(string serviceName);
        bool SignIn(string serviceName);
        bool SignOut(string serviceName);
        bool IsSignedIn(string serviceName);
        string GetUserId(string serviceName);
    }
}