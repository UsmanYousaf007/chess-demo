/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

namespace SocialEdge.Requests
{
    public abstract class SocialEdgeRequestResponse<TSUCCESS, TFAILURE>
    {
        public bool isSuccess;

        public abstract void BuildSuccess(TSUCCESS o);
        public abstract void BuildFailure(TFAILURE o);
    }
}