using System;
using System.Collections;
using System.IO;
using HUF.Storage.API;
using HUF.Storage.API.Structs;
using HUF.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace HUF.Storage.Implementation.ActionHandlers
{
    public class AudioClipLocalHandler : BaseActionHandler<AudioClip>
    {
        public AudioClipLocalHandler(string filePath, UnityAction<ObjectResultContainer<AudioClip>> completeHandler)
            : base(filePath, completeHandler)
        {
        }

        public override void ReadLocalFile()
        {
            var localFilePath = $"file:///{StorageUtils.GetLocalFilePath(FilePath)}";
            CoroutineManager.StartCoroutine(DownloadFromUrl(new Uri(localFilePath), false));
        }

        protected IEnumerator DownloadFromUrl(Uri uri, bool shouldSave = true)
        {
            var request = UnityWebRequestMultimedia.GetAudioClip(uri, GetAudioType(Path.GetExtension(uri.LocalPath)));
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                SendHandlerFail($"{request.error} \n on uri: {uri} ");
                yield break;
            }

            SendHandlerSuccess(DownloadHandlerAudioClip.GetContent(request),
                shouldSave ? request.downloadHandler.data : null);
        }

        protected static AudioType GetAudioType(string fileExtension)
        {
            switch (fileExtension)
            {
                case ".mp3": return AudioType.MPEG;
                case ".ogg": return AudioType.OGGVORBIS;
                case ".wav": return AudioType.WAV;
                default: return AudioType.UNKNOWN;
            }
        }
    }
}