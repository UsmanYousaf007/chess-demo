using System;
using System.Collections;
using HUF.Storage.Runtime.API;
using HUF.Storage.Runtime.Implementation.Structs;
using HUF.Utils.Runtime;
using UnityEngine;
using UnityEngine.Networking;

namespace HUF.Storage.Runtime.Implementation.ActionHandlers
{
    public class TextureLocalHandler: BaseActionHandler<Texture2D>
    {
        public TextureLocalHandler(string fileId, Action<ObjectResultContainer<Texture2D>> completeHandler)
            : base(fileId, completeHandler)
        {
        }

        public TextureLocalHandler(string fileId, Action<ObjectResultContainer<Texture2D>> completeHandler, string filePath)
            : base(fileId, completeHandler)
        {
            FilePath = filePath;
        }

        public override void DownloadFile()
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