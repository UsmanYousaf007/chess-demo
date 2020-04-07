using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


public class AzureFunction
{
    class RoutineRunnerBehavior : MonoBehaviour {}
    RoutineRunnerBehavior monoBehavior;

    public class AzureFunctionResponse
        {
        }

    public void Exectute(Action response)
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
