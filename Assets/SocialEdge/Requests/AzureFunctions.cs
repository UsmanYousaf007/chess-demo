/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AzureFunctionResponse
{
    public bool isSuccess;
    public long responseTime;
}

public class AzureFunctionRequest
{
    // Todo: Prefab for settings
    private const string AZURE_TITLE_URL = "https://chessstars.azurewebsites.net/api/";

    class RoutineRunnerBehavior : MonoBehaviour {}
    RoutineRunnerBehavior monoBehavior;
    WWWForm form;
    DateTime requestTimestamp;
    long responseTime;
    Action<AzureFunctionResponse> successCB;
    Action<AzureFunctionResponse> failureCB;
    AzureFunctionResponse response;
    string functionName;
    string titleURL;
    string postURL;

    public enum Functions
    {
        ProductReviewTest           // Test
    };

    public AzureFunctionRequest()
    {
        form = new WWWForm();
        response = new AzureFunctionResponse();
    }

    public AzureFunctionRequest SetFunction(Functions fn)
    {
        functionName = fn.ToString();
        postURL = titleURL + functionName;
        return this;
    }

    public AzureFunctionRequest AddField(string fieldName, int intValue)
    {
        form.AddField(fieldName, intValue);
        return this;
    }

    public AzureFunctionRequest AddField(string fieldName, string stringValue)
    {
        form.AddField(fieldName, stringValue);
        return this;
    }

    public AzureFunctionRequest SetSuccessCallback(Action<AzureFunctionResponse> responseCallback)
    {
        successCB = responseCallback;
        return this;
    }

    public AzureFunctionRequest SetFailureCallback(Action<AzureFunctionResponse> responseCallback)
    {
        failureCB = responseCallback;
        return this;
    }

    public void Send()
    {
        monoBehavior.StartCoroutine(ExecuteAzureWebFunction());
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
