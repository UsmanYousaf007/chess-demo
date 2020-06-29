/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
 
using strange.extensions.command.impl;
using TurboLabz.InstantFramework;
using UnityEngine.SignInWithApple;

public class InitSignInWithAppleCommand : Command
{
    //Services
    [Inject] public ISignInWithAppleService signInWithAppleService { get; set; }
    [Inject] public IBackendService backendService { get; set; }
    [Inject] public IAnalyticsService analyticsService { get; set; }

    public override void Execute()
    {
        if (!signInWithAppleService.IsSignedIn())
        {
            return;
        }

        Retain();

        signInWithAppleService.GetCredentialState().Then(OnCredentialStateReceived);
    }

    private void OnCredentialStateReceived(SignInWithApple.CallbackArgs args)
    {
        if (string.IsNullOrEmpty(args.error))
        {
            backendService.AuthSignInWithApple(args.userInfo.idToken, true);
        }
        Release();
    }
}
