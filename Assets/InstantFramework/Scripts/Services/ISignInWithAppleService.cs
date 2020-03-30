/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.promise.api;
using UnityEngine.SignInWithApple;

namespace TurboLabz.InstantFramework
{
    public interface ISignInWithAppleService
    {
        IPromise<SignInWithApple.CallbackArgs> Login();
        IPromise<SignInWithApple.CallbackArgs> GetCredentialState();
        bool IsSignedIn();
        bool IsSupported();
        string GetDisplayName();
    }
}
