using strange.extensions.command.impl;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class GetProfilePictureCommand : Command
    {
        //Parameters
        [Inject] public GetProfilePictureVO vo { get; set; }

        //Dispatch Signals
        [Inject] public ProfilePictureLoadedSignal profilePictureLoadedSignal { get; set; }

        //Models
        [Inject] public IPicsModel picsModel { get; set; }

        //Services
        [Inject] public IFacebookService facebookService { get; set; }
        [Inject] public IProfilePicService profilePicService { get; set; }

        public override void Execute()
        {
            var cachedPic = picsModel.GetPlayerPic(vo.playerId);

            if (cachedPic != null)
            {
                profilePictureLoadedSignal.Dispatch(vo.playerId, cachedPic);
                return;
            }

            if (!string.IsNullOrEmpty(vo.uploadedPicId))
            {
                Retain();
                profilePicService.GetProfilePic(vo.playerId, vo.uploadedPicId).Then(OnGetProfilePic);
            }
            else if (!string.IsNullOrEmpty(vo.facebookUserId))
            {
                Retain();
                facebookService.GetSocialPic(vo.facebookUserId, vo.playerId).Then(OnGetSocialPic);
            }
        }

        private void OnGetProfilePic(BackendResult result, Sprite sprite, string playerId)
        {
            if (result == BackendResult.SUCCESS)
            {
                SetPic(playerId, sprite);
            }

            Release();
        }

        private void OnGetSocialPic(FacebookResult result, Sprite sprite, string playerId)
        {
            if (result == FacebookResult.SUCCESS)
            {
                SetPic(playerId, sprite);
            }

            Release();
        }

        private void SetPic(string playerId, Sprite sprite)
        {
            picsModel.SetPlayerPic(playerId, sprite, vo.saveOnDisk);
            profilePictureLoadedSignal.Dispatch(playerId, sprite);
        }
    }
}
