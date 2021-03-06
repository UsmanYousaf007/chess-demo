/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-15 20:38:14 UTC+05:00
///
/// @description
/// [add_description_here]

using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using TurboLabz.InstantGame;
using System.Text;
using TurboLabz.CPU;
using TurboLabz.Multiplayer;

namespace TurboLabz.InstantFramework
{
    public class NavigatorCommand : Command
    {
        // Signal parameters
        [Inject] public NavigatorEvent navigatorEvent { get; set; }

        // Dispatch signals
        [Inject] public NavigatorShowViewSignal showViewSignal { get; set; }
        [Inject] public NavigatorHideViewSignal hideViewSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public ExitLongMatchSignal exitLongMatchSignal { get; set; }
        [Inject] public Multiplayer.CancelHintSingal cancelHintSingal { get; set; }
        [Inject] public DisableModalBlockersSignal disableModalBlockersSignal { get; set; }
        [Inject] public Multiplayer.ChessboardEventSignal chessboardEventSignal { get; set; }
        [Inject] public ShowViewBoardResultsPanelSignal showViewBoardResultsPanelSignal { get; set; }
        [Inject] public SkillSelectedSignal skillSelectedSignal { get; set; }
        [Inject] public LoadTopicsViewSignal loadTopicsViewSignal { get; set; }
        [Inject] public LoadLessonsViewSignal loadLessonsViewSignal { get; set; }
        [Inject] public LoadArenaSignal loadArenaSignal { get; set; }
        [Inject] public UnlockCurrentJoinedTournamentSignal unlockCurrentJoinedTournamentSignal { get; set; }
        [Inject] public LoginAsGuestSignal loginAsGuestSignal { get; set; }
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }

        // Models
        [Inject] public INavigatorModel navigatorModel { get; set; }
        [Inject] public ICPUGameModel cpuGameModel { get; set; }
        [Inject] public CPU.IChessboardModel cpuChessboardModel { get; set; }
        [Inject] public Multiplayer.IChessboardModel multiplayerChessboardModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public ILessonsModel lessonsModel { get; set; }

        // Services
		[Inject] public IAndroidNativeService androidNativeService { get; set; }
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IGameModesAnalyticsService gameModesAnalyticsService { get; set; }

        public override void Execute()
        {
            // LogUtil.Log("Navigator event: " + navigatorEvent, "yellow");

            NS newState = null;

            if (navigatorModel.currentState == null)
            {
                navigatorModel.Reset();
                navigatorModel.currentState = new NSStart();
                navigatorModel.currentState.SetCommand(this);
            }
            if (navigatorEvent == navigatorModel.ignoreEvent)
            {
                newState = null;
            }
            else
            {
                newState = navigatorModel.currentState.HandleEvent(navigatorEvent);    
            }

            if (newState != null)
            {
                newState.SetCommand(this);
                navigatorModel.previousState = navigatorModel.currentState;
                navigatorModel.currentState = newState;
                newState.RenderDisplayOnEnter();
                // LogUtil.Log("Navigator state: " + newState, "yellow");
            }
            else
            {
                // LogUtil.Log("Navigator event ignored.", "yellow");
            }
        }
    }
}
