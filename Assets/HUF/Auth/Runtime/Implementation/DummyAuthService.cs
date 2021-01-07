using HUF.Auth.Runtime.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using UnityEngine.Events;

namespace HUF.Auth.Runtime.Implementation
{
    public class DummyAuthService : IAuthService
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( HAuth.logPrefix, nameof(DummyAuthService) );
        public string Name { get; }
        public bool IsSignedIn => false;
        public string UserId => string.Empty;
        public string UserName => string.Empty;
        public bool IsInitialized => true;
        public event UnityAction<string> OnInitialized;
        public event UnityAction OnInitializationFailure;
        public event UnityAction<string, AuthSignInResult> OnSignInResult;
        public event UnityAction<string> OnSignOutComplete;

        public DummyAuthService( string nameSuffix )
        {
            Name = nameSuffix;
        }

        public void Init()
        {
            HLog.Log( logPrefix, $"Init {Name}" );
            OnInitialized.Dispatch( Name );
        }

        public bool SignIn()
        {
            HLog.Log( logPrefix, $"SignIn {Name}" );
            OnSignInResult.Dispatch( Name, AuthSignInResult.UnspecifiedFailure );
            return true;
        }

        public bool SignOut()
        {
            HLog.Log( logPrefix, $"SignOut {Name}" );
            OnSignOutComplete.Dispatch( Name );
            return true;
        }
    }
}