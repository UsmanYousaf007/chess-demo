
using System;
using System.Collections;
using System.Threading;
using BestHTTP;
using BestHTTP.SignalRCore;
using BestHTTP.SignalRCore.Encoders;
using SocialEdge.Communication;
using UnityEngine;
using UnityEngine.Networking;

namespace SocialEdge.Communication
{
    public class SignalRHub //: CommunicationHub<HubConnection>
    {
        public HubConnection connection;
        bool isConnected;
        private const string AZURE_TITLE_URL = "https://chessstars.azurewebsites.net/api";
        //private const string AZURE_TITLE_URL = "http://localhost:7071/api";
   

        public void Setup(string user)
        {
            IProtocol protocol = null;
        #if BESTHTTP_SIGNALR_CORE_ENABLE_GAMEDEVWARE_MESSAGEPACK
                protocol = new MessagePackProtocol();
        #else
                protocol = new JsonProtocol(new LitJsonEncoder());
        #endif

            connection = new HubConnection(new Uri(AZURE_TITLE_URL), protocol,user);
            connection.Options.PingInterval = new TimeSpan(0, 0, 5000);
            connection.AuthenticationProvider = new AzureSignalRServiceAuthenticator(connection);
        }

        public void Connect()
        {
            connection.StartConnect();
        }

        public bool SendMoveNotification(string receiver)
        {
            //if (connection.On)

            return false;
        }

        public  void Send(string receiverId)
        {
            Thread.Sleep(2000);
            var request = new HTTPRequest(new Uri(AZURE_TITLE_URL + "/addToGroup"), HTTPMethods.Post);
            request.AddHeader("Content-Type", "application/json");
            string json = LitJson.JsonMapper.ToJson(new Group { user = connection.userOptions.UserId, name = "Gamegroup" });
            request.RawData = System.Text.Encoding.UTF8.GetBytes(json);
            request.Send();

            Thread.Sleep(2000);
            var request2 = new HTTPRequest(new Uri(AZURE_TITLE_URL + "/messages"), HTTPMethods.Post);
            request2.AddHeader("Content-Type", "application/json");
            string json2 = LitJson.JsonMapper.ToJson(new TestMessage { sender = "Bob", text = "something" });
            request2.RawData = System.Text.Encoding.UTF8.GetBytes(json2);
            request2.Send();
            //if (connection.On)

            //return false;
        }

        public  void SetConnectedCallback(Action<HubConnection> connectedCB)
        {
            connection.OnConnected += connectedCB;
            //return request;
        }

        /// <summary>
        /// Sets error callback
        /// </summary>
        public  void SetErrorCallback(Action<HubConnection, string> errorCB)
        {
            connection.OnError += errorCB;
            //return request;
        }

        /// <summary>
        /// Sets disconnected callback
        /// </summary>
        public  void SetDisconnectedCallback(Action<HubConnection> errorCB)
        {
            connection.OnClosed += errorCB;
            //return request;
        }

        /// <summary>
        /// Sets reconnected callback
        /// </summary>
        public  void SetReconnectedCallback(Action<HubConnection> errorCB)
        {
            connection.OnReconnected += errorCB;
            //return request;
        }


        public void SetHook(string hookName, Action<dynamic> hookMethod)
        {
            connection.On<dynamic>(hookName, message => {
                hookMethod(message);
            });
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


        public void PrepareNegotiationRequest(BestHTTP.HTTPRequest request)
        {
            request.SetHeader("x-ms-signalr-userid", "user1");
        }
        /// <summary>
        /// Prepares the request by adding two headers to it
        /// </summary>
        /// 
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

    public class Group
    {
        public string name;
        public string user;
    }
}
