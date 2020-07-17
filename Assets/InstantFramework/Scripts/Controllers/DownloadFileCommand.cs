

using System;
using strange.extensions.command.impl;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class DownloadFileCommand : Command
    {
        [Inject] public string fileId { get; set; }

        [Inject] public IPicsModel picsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        [Inject] public IBackendService backendService { get; set; }

        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }

        public override void Execute()
        {
            Retain();

            backendService.GetDownloadUrl(fileId).Then(OnComplete);
        }

        private void OnComplete(BackendResult result)
        {
            bool isValidUrl = !String.IsNullOrEmpty(backendService.downloadUrl);
            if (result == BackendResult.SUCCESS && isValidUrl)
            {
              
            }

            else if (result == BackendResult.DOWNLOAD_URL_GET_FAILED)
            {
                backendErrorSignal.Dispatch(result);
            }

            Release();
        }
    }
}
