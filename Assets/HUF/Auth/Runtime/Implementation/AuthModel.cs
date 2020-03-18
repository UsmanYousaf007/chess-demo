using System.Collections.Generic;
using HUF.Auth.API;
using HUF.Utils.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.Auth.Implementation
{
    public class AuthModel : IAuthModel
    {
        readonly string logPrefix;

        readonly Dictionary<string, IAuthService> services = new Dictionary<string, IAuthService>();

        public event UnityAction<string> OnInitialized;
        public event UnityAction<string> OnSignInSuccess;
        public event UnityAction<string> OnSignInFailure;
        public event UnityAction<string> OnSignOutComplete;

        public AuthModel()
        {
            logPrefix = GetType().Name;
        }

        public bool TryRegisterService(IAuthService service)
        {
            if (IsServiceRegistered(service))
            {
                Debug.LogWarning($"[{logPrefix}] Service {service.Name} already added to model.");
                return false;
            }

            services.Add(service.Name, service);
            service.OnInitialized += Initialized;
            service.OnSignInSuccess += SignInSuccess;
            service.OnSignInFailure += SignInFailure;
            service.OnSignOutComplete += SignOutCompete;
            service.Init();

            return true;
        }

        bool IsServiceRegistered(IAuthService service)
        {
            return IsServiceRegistered(service.Name);
        }

        public bool IsServiceRegistered(string serviceName)
        {
            return services.ContainsKey(serviceName);
        }

        void Initialized(string serviceName)
        {
            OnInitialized.Dispatch(serviceName);
        }
        
        void SignInSuccess(string serviceName)
        {
            OnSignInSuccess.Dispatch(serviceName);
        }

        void SignInFailure(string serviceName)
        {
            OnSignInFailure.Dispatch(serviceName);
        }

        void SignOutCompete(string serviceName)
        {
            OnSignOutComplete.Dispatch(serviceName);
        }

        public bool SignIn(string serviceName)
        {
            var service = GetService(serviceName);
            return service != null && service.SignIn();
        }

        public bool SignOut(string serviceName)
        {
            var service = GetService(serviceName);
            return service != null && service.SignOut();
        }

        public bool IsSignedIn(string serviceName)
        {
            var service = GetService(serviceName);
            return service != null && service.IsSignedIn;
        }

        public string GetUserId(string serviceName)
        {
            var service = GetService(serviceName);
            return service != null ? service.UserId : string.Empty;
        }
        
        public bool IsInitialized(string serviceName)
        {
            var service = GetService(serviceName);
            return service != null && service.IsInitialized;
        }

        IAuthService GetService(string serviceName)
        {
            if (!IsServiceRegistered(serviceName))
            {
                Debug.LogWarning($"[{logPrefix}] Can't find service {serviceName}");
                return null;
            }

            return services[serviceName];
        }
    }
}