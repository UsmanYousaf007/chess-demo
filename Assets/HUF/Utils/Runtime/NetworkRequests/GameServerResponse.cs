using System;
using HUF.GameServer.Runtime.Response;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace HUF.Utils.Runtime.NetworkRequests
{
    public class GameServerResponse<TStatus, TData> : GameServerResponse<TStatus> where TStatus : Enum
    {
        public TData data;

        public GameServerResponse( TStatus status ) : base( status )
        {
            data = default;
        }

        public GameServerResponse( TStatus status, long responseCode ) : base( status, responseCode )
        {
            data = default;
        }

        public GameServerResponse( TStatus status, TData data, long responseCode ) : base( status, responseCode )
        {
            this.data = data;
        }

        public GameServerResponse( GameServerResponse<TStatus> other ) : this( other.status, other.responseCode ) { }

        public static bool TryGetFailedResponse( UnityWebRequest request, TStatus failedStatus, out GameServerResponse<TStatus, TData>
            response )
        {
            response = null;

            if ( request.isNetworkError )
            {
                response = new GameServerResponse<TStatus, TData>( failedStatus );
                return true;
            }

            if ( !request.isHttpError )
                return false;

            response = new GameServerResponse<TStatus, TData>( failedStatus, request.responseCode );
            return true;
        }
    }

    public class GameServerResponse<T> : GameServerResponse where T : Enum
    {
        /// <summary>
        /// A custom status of the response provided by the service after analyzing the server's response.
        /// </summary>
        [PublicAPI]
        public readonly T status;

        public GameServerResponse( T status, long responseCode ) : base( responseCode )
        {
            this.status = status;
        }

        public GameServerResponse( T status )
            : base()
        {
            this.status = status;
        }

        public GameServerResponse( GameServerResponse<T> other ) : this( other.status, other.responseCode ) { }

        public static bool TryGetFailedResponse( UnityWebRequest request, T failedStatus, out GameServerResponse<T> response )
        {
            response = null;

            if ( request.isNetworkError )
            {
                response = new GameServerResponse<T>( failedStatus );
                return true;
            }

            if ( !request.isHttpError )
                return false;

            response = new GameServerResponse<T>( failedStatus, request.responseCode );
            return true;
        }
    }

    public abstract class GameServerResponse
    {
        /// <summary>
        /// <para>A response provided by the server.</para>
        /// <para>See: https://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html</para>
        /// </summary>
        [PublicAPI]
        public readonly long responseCode;

        /// <summary>
        /// States whether a request failed due to a network error implying a connectivity issue.
        /// </summary>
        [PublicAPI]
        public readonly bool isNetworkError;

        protected GameServerResponse()
        {
            isNetworkError = true;
        }

        protected GameServerResponse( long responseCode )
        {
            this.responseCode = responseCode;
        }
    }
}
