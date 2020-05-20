using System;
using BestHTTP.SignalRCore;
using SocialEdge.Communication;
using SocialEdge.Requests;
using UnityEngine;

namespace SocialEdge.Communication
{
    public class CommunicationHub
    {
        SignalRHub signalRHub;
        SocialEdgePushNotificationRequest pushNotificationHub;
        bool isConnected;
        public CommunicationHub()
        {
            signalRHub = new SignalRHub();
        }

        public CommunicationHub(string user)
        {
            signalRHub = new SignalRHub();
            signalRHub.Setup(user);

            pushNotificationHub = new SocialEdgePushNotificationRequest();
        }

        public void Setup() {

            /*
             * I was trying to hide hubconnection class which is in SignalRHub
             * We can use these too for setting callbacks if we make the hubclass object private inside SignalRHub
             * but the problem is the object is returned in the callback so it is exposed anyway
            signalRHub.SetConnectedCallback(OnConnectedCallBack);
            signalRHub.SetDisconnectedCallback(OnDisconnectedCallBack);
            signalRHub.SetReconnectedCallback(OnReconnectedCallBack);
            signalRHub.SetErrorCallback(OnErrorCallBack);

            signalRHub.SetHook("onNotificationReceived", onMoveNotificationReceived);
            */

            signalRHub.connection.OnConnected += OnConnectedCallBack;
            signalRHub.connection.OnClosed += OnDisconnectedCallBack;
            signalRHub.connection.OnError += OnErrorCallBack;
            signalRHub.connection.OnReconnected += OnReconnectedCallBack;

            signalRHub.connection.On<dynamic>("onNotificationReceived", message => {
                                onMoveNotificationReceived(message);
                            });

            Action<SocialEdgePushNotificationResponse> successTarget = PushNotificationSuccessCallBack;
            Action<SocialEdgePushNotificationResponse> failureTarget = PushNotificationFailureCallBack;
            pushNotificationHub.SetSuccessCallback(successTarget)
                               .SetFailureCallback(failureTarget);
            signalRHub.Connect();

           
        }

        public void PushNotificationSuccessCallBack(SocialEdgePushNotificationResponse resp)
        {
            Debug.Log("Pushed notification sent successfully");

        }

        public void PushNotificationFailureCallBack(SocialEdgePushNotificationResponse resp)
        {
            Debug.Log("Push notification failed to send");

        }

        private void onMoveNotificationReceived(dynamic message)
        {
            Debug.Log(String.Format("Move made by user {0} with message {1}", message["sender"], message["text"]));

        }

        public void OnConnectedCallBack(HubConnection resp)
        {
            Debug.Log("SignalR connected ");
            isConnected = true;

            Send("user1");
        }

        public void OnErrorCallBack(HubConnection resp, string error)
        {
            Debug.Log("SignalR connection failed due to " + error);
            isConnected = false;
        }

        public void OnReconnectedCallBack(HubConnection resp)
        {
            Debug.Log("SignalR reconnected");
            isConnected = true;
        }

        public void OnDisconnectedCallBack(HubConnection resp)
        {
            Debug.Log("SignalR disconnected");
            isConnected = false;
        }

        public void Send(string receiverId)
        {
            if (isConnected)
            {
                //check if receiver is connected
                if (1 == 1)
                    signalRHub.Send(receiverId);
                else
                {
                    pushNotificationHub.SetReceiver("user1").Send();
                }
            }
        }
    }
}