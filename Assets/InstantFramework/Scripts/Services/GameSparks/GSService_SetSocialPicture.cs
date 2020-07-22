/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections;
using UnityEngine;
using Facebook.Unity;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using TurboLabz.TLUtils;
using System;
using UnityEngine.Networking;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        private IPromise<BackendResult> promise = new Promise<BackendResult>();
        //private IRoutineRunner routineRunner = new NormalRoutineRunner();


        public IPromise<BackendResult> UploadProfilePic(string filename, byte[] stream, string mimeType)
        {
            routineRunner.StartCoroutine(UploadProfilePicCR(filename,stream,mimeType));
            return promise;
        }

        private IEnumerator UploadProfilePicCR(string filename, byte[] stream, string mimeType)
        {
            var form = new WWWForm();
            form.AddBinaryData("file", stream, filename, mimeType);
            UnityWebRequest www = UnityWebRequest.Post(uploadUrl, form);
            yield return www.SendWebRequest();

            if (string.IsNullOrEmpty(www.error)) // Success
            {

                promise.Dispatch(BackendResult.SUCCESS);
            }
            else // Failure
            {
                promise.Dispatch(BackendResult.UPLOAD_PICTURE_FAILED);
            }
        }
    }
}
