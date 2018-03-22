/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-04-05 14:57:59 UTC+05:00
///
/// @description
/// [add_description_here]

using UnityEngine;

using strange.extensions.command.impl;

using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class GetPlayerProfilePictureCommand : Command
    {
        // Dispatch signals
        [Inject] public UpdatePlayerProfilePictureSignal updatePlayerProfilePictureSignal { get; set; }

        // Services
        [Inject] public IFacebookService facebookService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IInventoryModel inventoryModel { get; set; }

        public override void Execute()
        {
            // TODO(mubeeniqbal): This looks like a potential assertion point.
            // If that really is the case then add an assertion here.
            //
            // If the user is authenticated using a social platform then and
            // only then can we get a profile picture of the user for that
            // platform.
            if (playerModel.hasExternalAuth)
            {
                Retain();

                string userId = playerModel.externalAuthentications[ExternalAuthType.FACEBOOK].id;
                facebookService.GetProfilePicture(userId).Then(OnGetProfilePicture);
            }
        }

        private void OnGetProfilePicture(FacebookResult result, Texture2D texture)
        {
            // In case of a failure we just don't set the profile picture.
            if (result == FacebookResult.SUCCESS)
            {
                Sprite sprite = Sprite.Create(texture,
                                              new Rect(0, 0, texture.width, texture.height),
                                              new Vector2(0.5f, 0.5f));
                sprite.name = texture.name;
                playerModel.profilePictureFB = sprite;

                // If a view is already showing the old profile picture then it
                // should update the picture.

                if(inventoryModel.activeAvatarsId == "AvatarFacebook")
                {
                    playerModel.profilePicture = sprite;
                    updatePlayerProfilePictureSignal.Dispatch(sprite);  
                }
            }
            else
            {
                LogUtil.LogWarning("Unable to get the profile picture. FacebookResult: " + result);
            }

            Release();
        }
    }
}
