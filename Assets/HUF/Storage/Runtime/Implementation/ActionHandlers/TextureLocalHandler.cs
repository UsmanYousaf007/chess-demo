using System;
using System.Collections;
using HUF.Storage.API;
using HUF.Storage.API.Structs;
using HUF.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace HUF.Storage.Implementation.ActionHandlers
{
    public class TextureLocalHandler: BaseActionHandler<Texture2D>
    {
        public TextureLocalHandler(string filePath, UnityAction<ObjectResultContainer<Texture2D>> completeHandler) 
            : base(filePath, completeHandler)
        {
        }

        public override void ReadLocalFile()
        {
            var url = new Uri($"file:///{StorageUtils.GetLocalFilePath(FilePath)}");
            CoroutineManager.StartCoroutine(DownloadFromUrl(url, false));
        }
        
        protected IEnumerator DownloadFromUrl(Uri uri, bool shouldSave = true)
        {
            var request = UnityWebRequestTexture.GetTexture(uri);
            yield return request.SendWebRequest();
            
            if (request.isNetworkError || request.isHttpError)
            {
                SendHandlerFail($"{request.error} \n on uri: {uri} ");
                yield break;
            }
            SendHandlerSuccess(DownloadHandlerTexture.GetContent(request), 
                shouldSave ? request.downloadHandler.data : null);
        }
    }
}