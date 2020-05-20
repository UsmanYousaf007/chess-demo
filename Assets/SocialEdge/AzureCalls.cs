using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using BestHTTP;
public class AzureCalls
{
    class RoutineRunnerBehavior : MonoBehaviour { }
    RoutineRunnerBehavior monoBehavior;
    public AzureCalls()
    {
    }

    public void Exectute(Action response)
    {
        //monoBehavior.StartCoroutine(ExecuteAzureWebFunction());
    }

    public void ExecuteAzureWebFunction()
    {
        WWWForm form = new WWWForm();
        form.AddField("PLU", 4011);
        form.AddField("Rating", 5);
        var requestTime = DateTime.UtcNow;
        using (UnityWebRequest www = UnityWebRequest.Get("https://chessstars.azurewebsites.net/api/TestFunc"))
        {
            
            www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                var timeTook = (DateTime.UtcNow - requestTime).TotalMilliseconds;
                Debug.Log($"Client took {timeTook}ms to complete");
                Debug.Log($"Result: {www.downloadHandler.text}");
            }
        }

        HTTPRequest request = new HTTPRequest(new Uri("https://chessstars.azurewebsites.net/api/TestFunc"),HTTPMethods.Post, OnRequestFinished);
        request.AddField("name", "omer");
        request.Send();
    }

    void OnRequestFinished(HTTPRequest request, HTTPResponse response)
    {
        Debug.Log("Request Finished! Text received: " + response.DataAsText);
    }
}
