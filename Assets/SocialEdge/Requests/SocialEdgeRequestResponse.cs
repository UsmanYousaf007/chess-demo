/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

namespace SocialEdge.Requests
{
    public abstract class SocialEdgeRequestResponse
    {
        public bool isSuccess;

        protected object hresult;

        public abstract void BuildSuccess(object o);
        public abstract void BuildFailure(object o);
    }
}