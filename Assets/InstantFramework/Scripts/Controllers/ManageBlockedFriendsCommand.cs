using strange.extensions.command.impl;
using System.Linq;

namespace TurboLabz.InstantFramework
{
    public class ManageBlockedFriendsCommand : Command
    {
        // Parameters
        [Inject] public string filter { get; set; }

        //Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateManageBlockedFriendsViewSignal updateManageBlockedFriendsViewSignal { get; set; }
        [Inject] public GetSocialPicsSignal getSocialPicsSignal { get; set; }

        //Models
        [Inject] public IPlayerModel playerModel { get; set; }

        public override void Execute()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MANAGE_BLOCKED_FRIENDS);

            var blockedFriends = playerModel.blocked;

            if (!string.IsNullOrEmpty(filter))
            {
                blockedFriends = blockedFriends
                    .Where(f => f.Value.publicProfile.name.ToLower().Replace(" ", string.Empty).StartsWith(filter.ToLower().Replace(" ", string.Empty)))
                    .ToDictionary(f => f.Key, f => f.Value);
            }

            getSocialPicsSignal.Dispatch(blockedFriends);
            updateManageBlockedFriendsViewSignal.Dispatch(blockedFriends);
        }
    }
}
