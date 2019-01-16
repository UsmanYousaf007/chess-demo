/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using System;
using System.IO;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class NativeShareService : IShareService
    {
        [Inject] public IAnalyticsService analyticsService { get; set; }

        public void ShareApp (
            string emailSubject,
            string text,
            string androidSharePopupTitle)
        {

            NativeShare share = new NativeShare();
            share.SetSubject(emailSubject);
            share.SetText(text);
            share.SetTitle(androidSharePopupTitle);

            share.Share();
        }
    }
}

/*
- SetSubject( string subject ): sets the subject (primarily used in e-mail applications)
- SetText( string text ): sets the shared text. Note that the Facebook app will omit text, if exists
- AddFile( string filePath, string mime = null ): adds the file at path to the share action. You can add multiple files of different types. The MIME of the file is automatically determined if left null; however, if the file doesn't have an extension and/or you already know the MIME of the file, you can enter the MIME manually. MIME has no effect on iOS
- SetTitle( string title ): sets the title of the share dialog on Android platform. Has no effect on iOS
- SetTarget( string androidPackageName, string androidClassName = null ): shares content on a specific application on Android platform. If androidClassName is left null, list of activities in the share dialog will be narrowed down to the activities in the specified androidPackageName that can handle this share action (if there is only one such activity, it will be launched directly). Note that androidClassName, if provided, must be the full name of the activity (with its package). This function has no effect on iOS
*/