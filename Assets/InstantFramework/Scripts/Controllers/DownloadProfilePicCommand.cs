

using System;
using strange.extensions.command.impl;
using UnityEngine;
using UnityEngine.Networking;

namespace TurboLabz.InstantFramework
{
    public class DownloadProfilePicCommand : Command
    {
        [Inject] public string fileId { get; set; }

        [Inject] public IPicsModel picsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        [Inject] public IBackendService backendService { get; set; }

        [Inject] public UpdateFriendPicSignal updateFriendPicSignal { get; set; }
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }

        public override void Execute()
        {
            Retain();
            
            backendService.GetDownloadUrl(fileId).Then(OnUrlDownloadComplete);
        }

        private void OnUrlDownloadComplete(BackendResult result)
        {
            bool isValidUrl = !String.IsNullOrEmpty(backendService.downloadUrl);
            if (result == BackendResult.SUCCESS && isValidUrl)
            {
                //backendService.GetProfilePicture(backendService.downloadUrl, playerModel.id).Then(OnPictureDownloadComplete);
            }

            else if (result == BackendResult.DOWNLOAD_URL_GET_FAILED)
            {
                backendErrorSignal.Dispatch(result);
                Release();
            }   
        }

        private void OnPictureDownloadComplete(BackendResult result, Sprite sprite, string playerId)
        {
            if (result == BackendResult.SUCCESS && sprite != null)
            {
                //updateFriendPicSignal.Dispatch(playerModel.id, sprite);
            }

            else {

            }

            Release();
        }
    }
}
