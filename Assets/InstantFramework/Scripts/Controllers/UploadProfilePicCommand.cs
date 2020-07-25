

using System;
using GameSparks.Api.Messages;
using strange.extensions.command.impl;

namespace TurboLabz.InstantFramework
{
    public class UploadProfilePicCommand : Command
    {
        [Inject] public UploadFileVO uploadFileVO { get; set; }
        
        // Services
        [Inject] public IBackendService backendService { get; set; }
        //Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }
        // Signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public UpdatePlayerDataSignal updatePlayerDataSignal { get; set; }

        public override void Execute()
        {
            Retain();
            UploadCompleteMessage.Listener += OnUploadSuccess;
            backendService.GetUploadUrl().Then(OnGetUploadUrlComplete);
        }

        private void OnGetUploadUrlComplete(BackendResult result)
        {
            bool isValidUrl = !String.IsNullOrEmpty(backendService.uploadUrl);
            if (result == BackendResult.SUCCESS && isValidUrl)
            {
                backendService.UploadProfilePic(uploadFileVO.fileName, uploadFileVO.stream, uploadFileVO.mimeType).Then(OnUploadProcessComplete);
            }

            else if (result == BackendResult.UPLOAD_URL_GET_FAILED)
            {
                backendErrorSignal.Dispatch(result);
            }
        }

        public void OnUploadSuccess(GSMessage message)
        {
            string uploadedPicId = message.BaseData.GetString("uploadId");
            playerModel.uploadedPicId = uploadedPicId;
        }

        private void OnUploadProcessComplete(BackendResult result)
        {
            if (result != BackendResult.SUCCESS)
            {
                backendErrorSignal.Dispatch(result);
            }

            Release();
        }
    }
}
