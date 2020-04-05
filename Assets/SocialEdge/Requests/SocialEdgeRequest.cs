using System;

namespace SocialEdge.Requests
{
    public abstract class SocialEdgeRequest<TREQUEST, TRESPOINSE>
    {
        protected object hrequest;

        protected TRESPOINSE response;
        protected Action<TRESPOINSE> actionSuccess;
        protected Action<TRESPOINSE> actionFailure;
        protected long timeoutMillis;
        protected TREQUEST request;

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
        public TREQUEST SetSuccessCallback(Action<TRESPOINSE> successCB)
        {
            actionSuccess = successCB;
            return request;
        }

        /// <summary>
        /// Sets the request failure callback
        /// </summary>
        public TREQUEST SetFailureCallback(Action<TRESPOINSE> failureCB)
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