using System.Collections.Generic;
using HUF.Auth.Runtime.API;
using HUF.Utils.Runtime.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.Auth.Runtime.Implementation
{
    public class AuthModel : IAuthModel
    {
        readonly string logPrefix;

        readonly Dictionary<string, IAuthService> services = new Dictionary<string, IAuthService>();

        public event UnityAction<string> OnInitialized;
        public event UnityAction<string,bool> OnSignIn;
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
            service.OnSignIn += SignIn;
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

        void SignIn(string serviceName, bool success)
        {
            OnSignIn.Dispatch(serviceName,success);
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