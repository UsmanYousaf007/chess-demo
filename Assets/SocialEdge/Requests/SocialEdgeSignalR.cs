
using System;
using System.Collections;
using System.Threading;
using BestHTTP;
using BestHTTP.SignalRCore;
using BestHTTP.SignalRCore.Encoders;
using UnityEngine;
using UnityEngine.Networking;

namespace SocialEdge.SignalRNotifications
{
    public class SocialEdgeSignalR
    {
        HubConnection connection;

        private const string AZURE_TITLE_URL = "https://chessstars.azurewebsites.net/api";
        //private const string AZURE_TITLE_URL = "http://localhost:7071/api";
        public SocialEdgeSignalR()
        {
        }

           public void Run()
        {
            IProtocol protocol = null;

            #if BESTHTTP_SIGNALR_CORE_ENABLE_GAMEDEVWARE_MESSAGEPACK
                protocol = new MessagePackProtocol();
            #else
                protocol = new JsonProtocol(new LitJsonEncoder());
            #endif

            connection = new HubConnection(new Uri(AZURE_TITLE_URL), protocol);
            connection.AuthenticationProvider = new AzureSignalRServiceAuthenticator(connection);
            connection.OnConnected += Hub_OnConnected;
            connection.OnError += Hub_OnError;
            connection.OnClosed += Hub_OnClosed;
            //connection.On<System.Object>("onNotificationReceived", message =>
            //{
            //    onMoveNotificationReceived(message);
            //});
            connection.On<dynamic>("onNotificationReceived", message =>
            {
                onMoveNotificationReceived(message);
            });

            connection.StartConnect();            
        }

        private void onMoveNotificationReceived(dynamic message)
        {
            Debug.Log(String.Format("Move made by user {0} with message {1}", message["sender"], message["text"]));
            
        }

        /*This can be used with a defined type*/
        /*
        private void onMoveNotificationReceived(TestMessage message)
        {
            Debug.Log(String.Format("Move made by user {0} with message {1}", message.sender, message.text));
        }
        */

        private void Hub_OnClosed(HubConnection conn)
        {
            Debug.Log("Connection closed");
        }

        private void Hub_OnError(HubConnection conn, string arg)
        {
            Debug.Log("Error connecting");
        }

        private void Hub_OnConnected(HubConnection conn)
        {
            Debug.Log("Connected");

            /*simulate an event*/
            EventSimulation();
        }

        private static void EventSimulation()
        {
            Thread.Sleep(5000);
            var request = new HTTPRequest(new Uri(AZURE_TITLE_URL + "/messages"), HTTPMethods.Post);
            request.AddHeader("Content-Type", "application/json");
            string json = LitJson.JsonMapper.ToJson(new TestMessage { sender = "Bob", text = "something" });
            request.RawData = System.Text.Encoding.UTF8.GetBytes(json);
            request.Send();
        }
    }

    #region Authenticator
    public sealed class AzureSignalRServiceAuthenticator : IAuthenticationProvider
    {
        /// <summary>
        /// No pre-auth step required for this type of authentication
        /// </summary>
        public bool IsPreAuthRequired { get { return false; } }

#pragma warning disable 0067
        /// <summary>
        /// Not used event as IsPreAuthRequired is false
        /// </summary>
        public event OnAuthenticationSuccededDelegate OnAuthenticationSucceded;

        /// <summary>
        /// Not used event as IsPreAuthRequired is false
        /// </summary>
        public event OnAuthenticationFailedDelegate OnAuthenticationFailed;

#pragma warning restore 0067

        private HubConnection _connection;

        public AzureSignalRServiceAuthenticator(HubConnection connection)
        {
            this._connection = connection;
        }

        /// <summary>
        /// Not used as IsPreAuthRequired is false
        /// </summary>
        public void StartAuthentication()
        { }

        /// <summary>
        /// Prepares the request by adding two headers to it
        /// </summary>
        public void PrepareRequest(BestHTTP.HTTPRequest request)
        {
            if (this._connection.NegotiationResult == null)
                return;

            // Add Authorization header to http requests, add access_token param to the uri otherwise
            if (BestHTTP.Connections.HTTPProtocolFactory.GetProtocolFromUri(request.CurrentUri) == BestHTTP.Connections.SupportedProtocols.HTTP)
                request.SetHeader("Authorization", "Bearer " + this._connection.NegotiationResult.AccessToken);
            else
                request.Uri = PrepareUriImpl(request.Uri);
        }

        public Uri PrepareUri(Uri uri)
        {
            if (uri.Query.StartsWith("??"))
            {
                UriBuilder builder = new UriBuilder(uri);
                builder.Query = builder.Query.Substring(2);

                return builder.Uri;
            }

            return uri;
        }

        private Uri PrepareUriImpl(Uri uri)
        {
            string query = string.IsNullOrEmpty(uri.Query) ? "" : uri.Query + "&";
            UriBuilder uriBuilder = new UriBuilder(uri.Scheme, uri.Host, uri.Port, uri.AbsolutePath, query + "access_token=" + this._connection.NegotiationResult.AccessToken);
            return uriBuilder.Uri;
        }
    }
    #endregion
    public class TestMessage
    {
        public string sender;
        public string text;
    }

}
