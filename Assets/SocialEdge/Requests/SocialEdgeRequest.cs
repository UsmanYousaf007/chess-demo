/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;

namespace SocialEdge.Requests
{
    public abstract class SocialEdgeRequest<TREQUEST, TRESPONSE> where TRESPONSE : new()
    {
        protected TREQUEST request;
        protected TRESPONSE response;
        protected Action<TRESPONSE> actionSuccess;
        protected Action<TRESPONSE> actionFailure;
        
        protected long timeoutMillis;

        /// <summary>
        /// Constructor
        /// </summary>
        public SocialEdgeRequest()
        {
            response = new TRESPONSE();
        }

        protected TREQUEST Base(TREQUEST thisRequest)
        {
            request = thisRequest;
            return request;
        }

        /// <summary>
        /// Sets the request time out period
        /// </summary>
        public TREQUEST SetTimeout(long t)
        {
            timeoutMillis = t;
            return request;
        }

        /// <summary>
        /// Sets the request success callback
        /// </summary>
        public TREQUEST SetSuccessCallback(Action<TRESPONSE> successCB)
        {
            actionSuccess = successCB;
            return request;
        }

        /// <summary>
        /// Sets the request failure callback
        /// </summary>
        public TREQUEST SetFailureCallback(Action<TRESPONSE> failureCB)
        {
            actionFailure = failureCB;
            return request;
        }

        /// <summary>
        /// Submit the request
        /// </summary>
        public abstract void Send();
    }
}