using System;
using strange.extensions.command.impl;
using TurboLabz.InstantFramework;

namespace TurboLabz.InstantGame
{
    public class RefreshCommunityCommand : Command
    {
        [Inject] public IBackendService backendService { get; set; }

        public override void Execute()
        {
           // backendService.FriendsOp(

        }
    }
}

