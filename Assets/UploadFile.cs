
using UnityEngine;
using System.Collections;
using GameSparks.Api.Requests;

using GameSparks.Api.Messages;
using TurboLabz.InstantFramework;

public class UploadFile : MonoBehaviour
{

    private string lastUploadId;
    public Texture2D downloadedImage;

    [Inject] public IPhotoService photoService { get; set; }
    public void Start()
    {
        UploadCompleteMessage.Listener += GetUploadMessage;
        //photoService.PickPhoto(400, "jpeg");
        UploadScreenShot();
    }
    public void UploadScreenShot()
    {
        new GetUploadUrlRequest().Send((response) =>
        {
            Debug.Log(response.Url);
            UploadAFile(response.Url);

        });
    }

    //Our coroutine takes a screenshot of the game
    public void UploadAFile(string uploadUrl)
    {
        /*texture for testing*/
        int width = Screen.width;
        int height = Screen.height;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();
        byte[] bytes = tex.EncodeToPNG();
        Destroy(tex);

        var form = new WWWForm();
        form.AddField("somefield", "somedata");
        form.AddBinaryData("file", bytes, "profilepicture.png", "image/png");

        WWW w = new WWW(uploadUrl, form);

        if (w.error != null)
        {
            Debug.Log(w.error);
        }
        else
        {
            Debug.Log(w.text);
        }
    }

    public void GetUploadMessage(GSMessage message)
    {
        lastUploadId = message.BaseData.GetString("uploadId");
        DownloadAFile();
    }

    public void DownloadAFile()
    {
        new GetUploadedRequest().SetUploadId(lastUploadId).Send((response) =>
        {
            DownloadImage(response.Url);
        });
    }


    public void DownloadImage(string downloadUrl)
    {
        var www = new WWW(downloadUrl);
        //yield return www;
        downloadedImage = new Texture2D(200, 200);
        www.LoadImageIntoTexture(downloadedImage);
        
    }
}