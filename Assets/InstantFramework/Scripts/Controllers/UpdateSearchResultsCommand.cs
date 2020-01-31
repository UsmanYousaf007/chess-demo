using strange.extensions.command.impl;
using TurboLabz.InstantFramework;

namespace TurboLabz.InstantGame
{
    public class UpdateSearchResultsCommand : Command
    {
        // Params
        [Inject] public bool isSuccess { get; set; }

        // dispatch signals
        [Inject] public AddFriendsSignal addFriendsSignal { get; set; }
        [Inject] public GetSocialPicsSignal getSocialPicsSignal { get; set; }
        [Inject] public SortSearchedSignal sortSearchedSignal { get; set; }
        [Inject] public UpdateFriendBarSignal updateFriendBarSignal { get; set; }

        // models
        [Inject] public IPlayerModel playerModel { get; set; }

        public override void Execute()
        {
            if (isSuccess == false)
            {
                sortSearchedSignal.Dispatch(false);
            }
            else
            {
                addFriendsSignal.Dispatch(playerModel.search, FriendCategory.SEARCHED);
                getSocialPicsSignal.Dispatch(playerModel.search);

                foreach (string key in playerModel.search.Keys)
                {
                    updateFriendBarSignal.Dispatch(playerModel.search[key], key);
                }

                sortSearchedSignal.Dispatch(true);
            }
        }
    }
}
