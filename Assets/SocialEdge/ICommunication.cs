/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;

namespace SocialEdge.Communication
{
    public interface ICommunicationHub<T> //where TReturn : new()
    {
        //protected T request;
        //protected T response;
        //protected Action<T> actionConnected;
        //protected Action<T> actionDisconnected;
        //protected Action<T> actionReconnected;
        //protected Action<T,string> actionError;

        //protected long timeoutMillis;

        /// <summary>
        /// Constructor
        /// </summary>
        //public CommunicationHub()
        //{
        //    //response = new TReturn();
        //}

        //protected T Base(T thisRequest)
        //{
        //request = thisRequest;
        //return request;
        //}

        /// <summary>
        /// Sets the request time out period
        /// </summary>
        //public abstract T SetTimeout(long t);
        //{
        //    //timeoutMillis = t;
        //    //return request;
        //}

        /// <summary>
        /// Sets connected callback
        /// </summary>
        void SetConnectedCallback(Action<T> connectedCB);
        //{
        //    actionConnected = connectedCB;
        //    return request;
        //}

        /// <summary>
        /// Sets error callback
        /// </summary>
          void SetErrorCallback(Action<T, string> errorCB);
        //{
        //    actionError = errorCB;
        //    return request;
        //}
        /// <summary>
        /// Sets disconnected callback
        /// </summary>
          void SetDisconnectedCallback(Action<T> errorCB);
        //{
        //    actionDisconnected = errorCB;
        //    return request;
        //}

        /// <summary>
        /// Sets reconnected callback
        /// </summary>
          void SetReconnectedCallback(Action<T> errorCB);
        //{
        //    actionReconnected = errorCB;
        //    return request;
        //}
        /// <summary>
        /// Submit the request
        /// </summary>
         void Send();
    }
}