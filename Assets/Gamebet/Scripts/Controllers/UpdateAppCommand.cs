/// @license #LICENSE# <#LICENSE_URL#>
/// @copyright Copyright (C) #COMPANY# #YEAR# - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author #AUTHOR# <#AUTHOR_EMAIL#>
/// @company #COMPANY# <#COMPANY_URL#>
/// @date #DATE#
///
/// @description
/// #DESCRIPTION#
/// 
using UnityEngine;

using strange.extensions.command.impl;

namespace TurboLabz.Gamebet
{
    public class UpdateAppCommand : Command
    {
        public override void Execute()
        {
            TurboLabz.Common.LogUtil.Log("Updating Updating. I am in the google play store","red");

            #if UNITY_ANDROID
            Application.OpenURL("market://details?id=com.turbolabz.chess.android.googleplay");
            #endif
        }
    }
}
