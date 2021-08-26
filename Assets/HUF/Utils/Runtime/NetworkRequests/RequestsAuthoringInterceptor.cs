using System;
using System.Collections;
using System.Net;
using System.Text;
using HUF.Utils.Runtime.UI.CanvasBlocker;
using UnityEngine;
using UnityEngine.Networking;

namespace HUF.Utils.Runtime.NetworkRequests
{
    public class RequestsAuthoringInterceptor
    {
        const int LOCAL_PORT = 45003;
        const int TIMEOUT = 3;

        const string RESPONSE = @"HTTP/1.1 {0}
Date: Thu, 03 Jan 2019 11:11:11 GMT
Server: fake
Accept-Ranges: bytes
Content-Length: {1}
Content-Type: text/html;
charset=UTF-8

{2}";

        static TcpServer server;

        static RequestPrompt prompt = new RequestPrompt();
        static bool isBusy;
        readonly UnityWebRequest originalRequest;
        bool isResolved = false;

        public RequestsAuthoringInterceptor( UnityWebRequest originalRequest )
        {
            this.originalRequest = originalRequest;
        }

        public bool IsOverriden { get; private set; }

        public IEnumerator Prompt()
        {
            if ( server == null )
            {
                server = new TcpServer( LOCAL_PORT, HandleRequest );
                yield return null;
            }

            while ( !DebugButtonsScreen.IsAvailable || isBusy )
            {
                yield return null;
            }

            isBusy = true;

            ShowPrompt( originalRequest.url );
            //TODO: Move prompts to HDebugger
            //HDebug.Prompt<RequestPrompt>( prompt, Resolve );

            yield return new WaitUntil( () => isResolved );

            IsOverriden = prompt.Override;

            if ( !IsOverriden )
            {
                yield return originalRequest.SendWebRequest();

                isBusy = false;
                yield break;
            }

            originalRequest.url = $"localhost:{LOCAL_PORT}";
            originalRequest.timeout = TIMEOUT;

            yield return originalRequest.SendWebRequest();

            yield return null;

            isBusy = false;
        }

        static string GetLabel( HttpStatusCode code )
        {
            return $"{code} ({(int)code})";
        }

        void ShowPrompt( string title )
        {
            prompt.Override = true;

            DebugButtonsScreen.Instance.AddGUIButton( "Pass through", HandleCancelledPrompt );

            DebugButtonsScreen.Instance.AddGUIButton( GetLabel( HttpStatusCode.OK ),
                Resolve( HttpStatusCode.OK ) );
            DebugButtonsScreen.Instance.AddGUIButton( GetLabel( HttpStatusCode.NoContent ),
                Resolve( HttpStatusCode.NoContent ) );
            DebugButtonsScreen.Instance.AddGUIButton( GetLabel( HttpStatusCode.NotFound ),
                Resolve( HttpStatusCode.NotFound ) );
            DebugButtonsScreen.Instance.AddGUIButton( GetLabel( HttpStatusCode.Conflict ),
                Resolve( HttpStatusCode.Conflict ) );
            DebugButtonsScreen.Instance.AddGUIButton( GetLabel( HttpStatusCode.BadRequest ),
                Resolve( HttpStatusCode.BadRequest ) );
            DebugButtonsScreen.Instance.AddGUIButton( GetLabel( HttpStatusCode.BadGateway ),
                Resolve( HttpStatusCode.BadGateway ) );
            DebugButtonsScreen.Instance.AddGUIInput( "Data", HandlePrompt );
            DebugButtonsScreen.Instance.Show( title );
        }

        void HandleCancelledPrompt()
        {
            prompt.Override = false;
            Resolve();
        }

        void HandlePrompt( string data )
        {
            prompt.Data = data;
        }

        void Resolve()
        {
            isResolved = true;
        }

        Action Resolve( HttpStatusCode code )
        {
            return () =>
            {
                prompt.ResponseCode = (int)code;
                Resolve();
            };
        }

        static string HandleRequest( string incoming )
        {
            int length = prompt.Data != null ? Encoding.ASCII.GetBytes( prompt.Data ).Length : 0;
            return string.Format( RESPONSE, prompt.ResponseCode, length, prompt.Data );
        }

        class RequestPrompt
        {
            public bool Override { get; set; }
            public int ResponseCode { get; set; }
            public string Data { get; set; }
        }
    }
}
