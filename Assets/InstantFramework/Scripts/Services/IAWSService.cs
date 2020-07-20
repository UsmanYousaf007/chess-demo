/// @license Propriety <http://license.url>
/// @copyright Copyright (C) DefaultCompany 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.promise.api;

namespace TurboLabz.InstantFramework
{
    public partial interface IAWSService
    {
        IPromise<BackendResult, string> GetSignedUrl(string unsignedURL);
    }
}