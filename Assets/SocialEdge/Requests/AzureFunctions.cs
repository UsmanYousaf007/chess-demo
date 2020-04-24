/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using System.Collections;
using BestHTTP;
using BestHTTP.SignalRCore;
using BestHTTP.SignalRCore.Encoders;
using UnityEngine;
using UnityEngine.Networking;

//using SocialEdge.Utils;

namespace SocialEdge.Requests
{
    /// <summary>
    /// Azure function request result response
    /// </summary>
    public class AzureFunctionResponse
    {
        public bool isSuccess;
        public long responseTime;
    }

    public class SocialEdgeRoutineRunner
    {
        public MonoBehaviour monoBehavior;
    }

   

    /// <summary>
    /// Azure function request
    /// </summary>
    public class AzureFunctionRequest
    {
        class TestMessage
        {
            public string sender;
            public string text;
        }

        HubConnection connection;
        // Todo: Prefab for settings
        private const string AZURE_TITLE_URL = "https://chessstars.azurewebsites.net/api";
        //private const string AZURE_TITLE_URL = "http://localhost:7071/api";
        protected SocialEdgeRoutineRunner routineRunner;
        Coroutine coroutineFunction;
        WWWForm form;
        long responseTime;
        DateTime requestTimestamp;
        string functionName;
        string titleURL;
        string postURL;
        AzureFunctionResponse response;
        Action<AzureFunctionResponse> successCB;
        Action<AzureFunctionResponse> failureCB;

        public enum Functions
        {
            ProductReviewTest,           // Test
            Test
        };



        /// <summary>
        /// Azure function constructor
        /// </summary>
        public AzureFunctionRequest()
        {
            titleURL = AZURE_TITLE_URL; // place holder
            form = new WWWForm();
            response = new AzureFunctionResponse();
            routineRunner = new SocialEdgeRoutineRunner();

        }

        /// <summary>
        /// Set the function to call
        /// </summary>
        public AzureFunctionRequest SetFunction(Functions fn)
        {
            functionName = fn.ToString();
            postURL = titleURL + functionName;
            return this;
        }

        /// <summary>
        /// Add an integer function parameter field
        /// </summary>
        public AzureFunctionRequest AddField(string fieldName, int intValue)
        {
            form.AddField(fieldName, intValue);
            return this;
        }

        /// <summary>
        /// Add a string function parameter field
        /// </summary>
        public AzureFunctionRequest AddField(string fieldName, string stringValue)
        {
            form.AddField(fieldName, stringValue);
            return this;
        }

        /// <summary>
        /// Set request success callback
        /// </summary>
        public AzureFunctionRequest SetSuccessCallback(Action<AzureFunctionResponse> responseCallback)
        {
            successCB = responseCallback;
            return this;
        }

        /// <summary>
        /// Set request failure callback
        /// </summary>
        public AzureFunctionRequest SetFailureCallback(Action<AzureFunctionResponse> responseCallback)
        {
            failureCB = responseCallback;
            return this;
        }

        /// <summary>
        /// Send function request
        /// </summary>
        public void Send()
        {
            coroutineFunction = routineRunner.monoBehavior.StartCoroutine(ExecuteAzureWebFunctionPostRequest());
            Debug.Log("AZURE FUNCTION SEND");
        }

        private IEnumerator ExecuteAzureWebFunctionPostRequest()
        {
            requestTimestamp = DateTime.UtcNow;
            using (UnityWebRequest request = UnityWebRequest.Post(postURL, form))
            {
                yield return request.SendWebRequest();

                responseTime = (long)((DateTime.UtcNow - requestTimestamp).TotalMilliseconds);
                response.responseTime = responseTime;

                ProcessResponse(request);
            }

        }

        private IEnumerator ExecuteAzureWebFunctionGetRequest()
        {
            requestTimestamp = DateTime.UtcNow;
            using (UnityWebRequest request = UnityWebRequest.Get(postURL))
            {
                yield return request.SendWebRequest();

                responseTime = (long)((DateTime.UtcNow - requestTimestamp).TotalMilliseconds);
                response.responseTime = responseTime;

                ProcessResponse(request);
            }

        }

        private void ProcessResponse(UnityWebRequest request)
        {
            if (request.isNetworkError || request.isHttpError)
            {
                if (failureCB != null)
                {
                    response.isSuccess = false;
                    failureCB(response);
                }
            }
            else if (successCB != null)
            {
                response.isSuccess = true;
                successCB(response);
            }
        }
       
    }
    
}


