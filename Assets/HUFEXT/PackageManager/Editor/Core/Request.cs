using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace HUFEXT.PackageManager.Editor.Core
{
    public enum RequestStatus
    {
        Success,
        Failure
    }
    
    public class WebResponse
    {
        public RequestStatus status;
        public byte[] bytes;
        public string text;
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
    }

    public class Request
    {
        readonly UnityWebRequest request;
        readonly UnityAction<WebResponse> callback;

        public Request( string route, UnityAction<WebResponse> onComplete )
        {
            route = route.Replace( "\r", string.Empty );
            request = Models.Token.CreateSignedRequest( route );
            callback = onComplete;
        }
        
        public static RouteBuilder CreateRoute( string path ) => new RouteBuilder( path );

        public void Send()
        {
            if ( request == null )
            {
                Utils.Common.LogError( "Unable to send null request. Please check your authorization token." );
                return;
            }

            request.timeout = Models.Keys.Routing.TIMEOUT;
            request.SendWebRequest();
            EditorApplication.update += UpdateRequest;
        }

        void UpdateRequest()
        {
            if ( request == null || request.isHttpError || request.isNetworkError )
            {
                EditorApplication.update -= UpdateRequest;
                callback?.Invoke( new WebResponse()
                {
                    status = RequestStatus.Failure,
                    bytes = request?.downloadHandler.data,
                    text = request?.downloadHandler.text
                });
                request?.Dispose();
                return;
            }
            
            if ( request != null && !request.isDone )
            {
                return;
            }
            
            callback?.Invoke( new WebResponse()
            {
                status = request.isHttpError || request.isNetworkError ? RequestStatus.Failure : RequestStatus.Success,
                bytes = request?.downloadHandler.data,
                text = request?.downloadHandler.text
            });

            request?.Dispose();
            EditorApplication.update -= UpdateRequest;
        }
    }
}
