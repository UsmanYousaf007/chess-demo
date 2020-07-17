

using System;
using GameSparks.Api.Messages;
using strange.extensions.command.impl;
using UnityEngine;
using UnityEngine.Networking;

namespace TurboLabz.InstantFramework
{
    public class UploadFileCommand : Command
    {
        [Inject] public byte[] fileStream { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        //Models
        [Inject] public IPlayerModel playerModel { get; set; }

        // Signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public UpdatePlayerDataSignal updatePlayerDataSignal { get; set; }

        public override void Execute()
        {
            Retain();
            UploadCompleteMessage.Listener += OnUploadSuccess;
            backendService.GetUploadUrl().Then(OnComplete);
        }

        private void OnComplete(BackendResult result)
        {
            bool isValidUrl = !String.IsNullOrEmpty(backendService.uploadUrl);

            if (result == BackendResult.SUCCESS && isValidUrl)
            {
                var form = new WWWForm();
                
                form.AddBinaryData("file", fileStream, "profilPicture.png", "image/png");
                using (UnityWebRequest www = UnityWebRequest.Post(backendService.uploadUrl, form))
                {
                    www.SendWebRequest();
                    if (www.isNetworkError || www.isHttpError)
                    {
                        Debug.Log(www.error);
                    }
                    else
                    {
                        Debug.Log("Form upload complete!");
                    }
                }
                
                new UnityWebRequest();
            }

            else if (result == BackendResult.UPLOAD_URL_GET_FAILED)
            {
                backendErrorSignal.Dispatch(result);
            }

            Release();
        }

        public void OnUploadSuccess(GSMessage message)
        {
            string uploadedPicId = message.BaseData.GetString("uploadId");
            playerModel.uploadedPicId = uploadedPicId;
            updatePlayerDataSignal.Dispatch();

        }
    }
}
