using System;
using HUFEXT.PackageManager.Editor.Implementation.Remote.Auth;
using HUFEXT.PackageManager.Editor.Implementation.Remote.Data;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace HUFEXT.PackageManager.Editor.Implementation.Remote.Requests
{
    public enum RequestStatus
    {
        Unknown,
        Success,
        Failure
    }
    public class WebResponse
    {
        public RequestStatus status;
        internal DownloadHandler handler;

        public byte[] Bytes => handler.data;
        public string Text => handler.text;
    }

    public class BaseRequest : IDisposable
    {
        private UnityWebRequest request;
        public event UnityAction<WebResponse> OnComplete;

        public Guid ID { private set; get; }
        
        public BaseRequest( string route, UnityAction<WebResponse> onComplete )
        {
            ID = Guid.NewGuid();
            request = PrepareRequest( route );
            OnComplete = onComplete;
        }

        private static UnityWebRequest PrepareRequest( string route )
        {
            route = route.Replace( "\r", "" );
            var request = UnityWebRequest.Get( route );
            var token = Token.LoadExistingToken();
            if ( token != null && token.IsValid )
            {
                request.SetRequestHeader( "x-developer-id", token.DeveloperID );
                request.SetRequestHeader( "x-api-key", token.AccessKey );
            }
            return request;
        }
        
        public void Send()
        {
            EditorApplication.update += UpdateRequest;
            if ( request != null )
            {
                request.timeout = RoutingScheme.TimeoutInSeconds;
                request.SendWebRequest();
            }
        }

        private void UpdateRequest()
        {
            if ( request == null )
            {
                EditorApplication.update -= UpdateRequest;
                Complete( RequestStatus.Failure );
                return;
            }

            if ( request != null && !request.isDone )
            {
                return;
            }

            var status = RequestStatus.Success;
            if ( request.isHttpError )
            {
                status = RequestStatus.Failure;
            }
            else if ( request.isNetworkError )
            {
                status = RequestStatus.Failure;
            }
            
            EditorApplication.update -= UpdateRequest;
            Complete( status );
        }
        
        private void Complete( RequestStatus status )
        {
            OnComplete?.Invoke( new WebResponse()
            {
                status = status,
                handler = request.downloadHandler
            });
        }
        
        public void Dispose()
        {
            request?.Dispose();
            request = null;
        }
    }
}
