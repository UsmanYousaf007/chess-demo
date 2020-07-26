/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections;
using GameSparks.Api.Messages;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using UnityEngine;
using UnityEngine.Networking;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> UploadProfilePic(string filename, byte[] stream, string mimeType)
        {
            IPromise<BackendResult> uploadProfilePicPromise = new Promise<BackendResult>();
            UploadCompleteMessage.Listener += OnUploadSuccess;
            routineRunner.StartCoroutine(UploadProfilePicCR(filename,stream,mimeType, uploadProfilePicPromise));
            return uploadProfilePicPromise;
        }

        private IEnumerator UploadProfilePicCR(string filename, byte[] stream, string mimeType, IPromise<BackendResult> uploadProfilePicPromise)
        {
            var form = new WWWForm();
            form.AddBinaryData("file", stream, filename, mimeType);
            UnityWebRequest www = UnityWebRequest.Post(uploadUrl, form);
            yield return www.SendWebRequest();

            if (string.IsNullOrEmpty(www.error))
            {
                uploadProfilePicPromise.Dispatch(BackendResult.SUCCESS);
            }
            else
            {
                UploadCompleteMessage.Listener -= OnUploadSuccess;
                uploadProfilePicPromise.Dispatch(BackendResult.UPLOAD_PICTURE_FAILED);
            }
        }

        public void OnUploadSuccess(GSMessage message)
        {
            string uploadedPicId = message.BaseData.GetString("uploadId");
            UploadCompleteMessage.Listener -= OnUploadSuccess;
            playerModel.uploadedPicId = uploadedPicId;
        }
    }
}
