using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


public class AzureFunctionResponse
{
    UnityWebRequest www;
}

public class AzureFunctionRequest
{
    class RoutineRunnerBehavior : MonoBehaviour {}
    RoutineRunnerBehavior monoBehavior;

    public void Exectute(Action<AzureFunctionResponse> responseCallback)
    {
        monoBehavior.StartCoroutine(ExecuteAzureWebFunction());
    }

    private IEnumerator ExecuteAzureWebFunction()
    {
        WWWForm form = new WWWForm();
        form.AddField("PLU", 4011);
        form.AddField("Rating", 5);
        var requestTime = DateTime.UtcNow;
        using (UnityWebRequest www = UnityWebRequest.Post("https://chessstars.azurewebsites.net/api/ProductReviewTest", form))
        {
            yield return www.SendWebRequest();

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
    }
}
