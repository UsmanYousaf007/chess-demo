/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


namespace SocialEdge.Requests
{
    public class AzureRoutineRunnerBehavior : MonoBehaviour { }
    public class AzureRoutineRunner
    {
        private const string GAME_OBJECT_NAME = "AzureRoutineRunnerBehavior";
        public MonoBehaviour monoBehavior;

        public AzureRoutineRunner()
        {
            GameObject go = GameObject.Find(GAME_OBJECT_NAME);

            if (go == null)
            {
                go = new GameObject(GAME_OBJECT_NAME);
            }

            monoBehavior = go.GetComponent<AzureRoutineRunnerBehavior>();

            if (monoBehavior == null)
            {
                monoBehavior = go.AddComponent<AzureRoutineRunnerBehavior>();
            }
        }
    }

    /// <summary>
    /// Azure function request result response
    /// </summary>
    public class AzureFunctionResponse
    {
        public bool isSuccess;
        public long responseTime;
    }

    /// <summary>
    /// Azure function request
    /// </summary>
    public class AzureFunctionRequest
    {
        // Todo: Prefab for settings
        private const string AZURE_TITLE_URL = "https://chessstars.azurewebsites.net/api/";

        protected AzureRoutineRunner routineRunner;
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
            routineRunner = new AzureRoutineRunner();
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
            coroutineFunction = routineRunner.monoBehavior.StartCoroutine(ExecuteAzureWebFunction());
            Debug.Log("AZURE FUNCTION SEND");
        }

        private IEnumerator ExecuteAzureWebFunction()
        {
            requestTimestamp = DateTime.UtcNow;
            using (UnityWebRequest www = UnityWebRequest.Post(postURL, form))
            {
                yield return www.SendWebRequest();

                responseTime = (long)((DateTime.UtcNow - requestTimestamp).TotalMilliseconds);
                response.responseTime = responseTime;

                if (www.isNetworkError || www.isHttpError)
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
}


