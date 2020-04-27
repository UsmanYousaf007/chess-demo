using strange.extensions.command.impl;

namespace TurboLabz.InstantFramework
{
    public class ManageBlockedFriendsCommand : Command
    {
        // Parameters
        [Inject] public string filter { get; set; }

        //Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateManageBlockedFriendsViewSignal updateManageBlockedFriendsViewSignal { get; set; }

        //Models
        [Inject] public IPlayerModel playerModel { get; set; }

        public override void Execute()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MANAGE_BLOCKED_FRIENDS);

            var blockedFriends = playerModel.blocked;

            if (!string.IsNullOrEmpty(filter))
            {
                foreach (var entry in playerModel.blocked)
                {
                    if (!entry.Value.publicProfile.name.ToLower().StartsWith(filter.ToLower()))
                    {
                        blockedFriends.Remove(entry.Key);
                    }
                }
            }

            updateManageBlockedFriendsViewSignal.Dispatch(blockedFriends);
        }
    }
}
