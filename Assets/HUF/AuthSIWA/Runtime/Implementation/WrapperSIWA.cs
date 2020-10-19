using System;
using AppleAuth;
using AppleAuth.Native;
using HUF.Utils.Runtime;
using UnityEngine;

namespace HUF.AuthSIWA.Runtime.Implementation
{
    public class WrapperSIWA : MonoBehaviour
    {
        public IAppleAuthManager AuthManager => appleAuthManager;
        private IAppleAuthManager appleAuthManager;

        void Awake()
        {
            if (!AppleAuthManager.IsCurrentPlatformSupported)
                return;
            
            appleAuthManager = new AppleAuthManager(new PayloadDeserializer());
            gameObject.AddComponent<DontDestroyOnLoad>();
        }

        private void Update()
        {
            appleAuthManager?.Update();
        }
    }
}