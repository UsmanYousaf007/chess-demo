using System.Collections;
using System.Net.Http;
using System.Text;
using HUF.Utils.Runtime.Logging;
using UnityEngine;
using UnityEngine.Networking;
using WebRequestAction = UnityEngine.Events.UnityAction<UnityEngine.Networking.UnityWebRequest>;

namespace HUF.Utils.Runtime.NetworkRequests
{
    public static class GameServerUtils
    {
        public const long UNINITIALIZED_CODE = (long)System.Net.HttpStatusCode.ServiceUnavailable;

        const ushort SERVER_ERROR_THRESHOLD = (ushort)System.Net.HttpStatusCode.InternalServerError;
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(GameServer.Runtime.Utils) );
        public static readonly HttpMethod patchMethod = new HttpMethod( "PATCH" );

        public static RouteBuilder CreateRoute( string path )
        {
            return new RouteBuilder( path );
        }

        public static RequestData CreateRequestData( GameServerBaseConfig config, HttpMethod method, string route )
        {
            return new RequestData()
            {
                method = method.Method,
                url = config.ServerUrl,
                api = route,
                token = config.ValidationToken,
                timeout = config.ConnectionTimeOutInSeconds,
                errorRetryLimit = config.ErrorRetryCount,
                errorRetryWait = config.ErrorRetryWait
            };
        }

        public static Coroutine AsyncRequest( RequestData data, WebRequestAction response )
        {
            return CoroutineManager.StartCoroutine( AsyncRequestCoroutine( data, response ) );
        }

        static IEnumerator AsyncRequestCoroutine( RequestData data,
            WebRequestAction response,
            int retryCount = 0 )
        {
            var www = PrepareRequest( data );
            yield return www.SendWebRequest();

#if HUF_TIMESERVER
            TimeServer.Runtime.Implementation.ExternalWebrequestConnector.HandleRequest( www );
#endif

            if ( www.isNetworkError || www.isHttpError && IsServerError( www.responseCode ) )
            {
                if ( retryCount < data.errorRetryLimit )
                {
                    yield return new WaitForSecondsRealtime( data.errorRetryWait );

                    yield return CoroutineManager.StartCoroutine( AsyncRequestCoroutine( data, response, ++retryCount ) );
                    yield break;
                }

                HLog.LogWarning( logPrefix,
                    $"ServerError: code {www.responseCode}, error: {www.error} \nurl: {data.url}{data.api}\nmessage:{www.downloadHandler.text}\nRequest body:{data.postData}" );
            }
            else
            {
                HLog.Log( logPrefix,
                    $"Received server response: code {www.responseCode} \nurl: {data.url}{data.api}\nmessage:{www.downloadHandler.text}\nRequest body:{data.postData}" );
            }

            response.Invoke( www );
        }

        static bool IsServerError( long code )
        {
            return code >= SERVER_ERROR_THRESHOLD;
        }

        static UnityWebRequest PrepareRequest( RequestData data )
        {
            HLog.Log( logPrefix, $"New UnityWebRequest: {data.url}{data.api}  {data.method}" );
            var request = new UnityWebRequest( data.url + data.api, data.method );

            if ( !string.IsNullOrEmpty( data.postData ) )
            {
                byte[] postDataBytes = Encoding.UTF8.GetBytes( data.postData );
                request.uploadHandler = new UploadHandlerRaw( postDataBytes );
            }

            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader( "Content-Type", "application/json" );

            if ( !string.IsNullOrEmpty( data.token ) )
            {
                request.SetRequestHeader( "Authorization", data.token );
            }

            request.timeout = data.timeout;
            return request;
        }

        [System.Serializable]
        public struct RequestData
        {
            public string method;
            public string url;
            public string api;
            public string token;
            public string postData;
            public int timeout;
            public int errorRetryLimit;
            public float errorRetryWait;
        }

        [System.Serializable]
        public class RouteBuilder
        {
            string route;
            public string Value => route;

            internal RouteBuilder( string path )
            {
                route = path;
            }

            public RouteBuilder Set( string pattern, string value )
            {
                route = route.Replace( pattern, value );
                return this;
            }

            public RouteBuilder Set( string pattern, int value )
            {
                route = route.Replace( pattern, value.ToString() );
                return this;
            }
        }
    }
}