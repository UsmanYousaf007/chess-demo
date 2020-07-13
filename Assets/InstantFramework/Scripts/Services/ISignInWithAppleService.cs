/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.promise.api;

namespace TurboLabz.InstantFramework
{
    public interface ISignInWithAppleService
    {
        IPromise<bool, string> Login();
        bool IsSignedIn();
        bool IsSupported();
        string GetDisplayName();
        string GetTokenId();
    }
}
