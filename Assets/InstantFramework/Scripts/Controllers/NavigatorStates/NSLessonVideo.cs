/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class NSLessonVideo : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.LESSON_VIDEO);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            NavigatorViewId viewId = CameFrom(NavigatorViewId.TOPICS_VIEW, NavigatorViewId.LESSONS_VIEW, NavigatorViewId.LOBBY);

            if (evt == NavigatorEvent.ESCAPE)
            {
                if (viewId == NavigatorViewId.TOPICS_VIEW)
                {
                    cmd.loadTopicsViewSignal.Dispatch();
                    return null;
                }
                else if (viewId == NavigatorViewId.LESSONS_VIEW)
                {
                    cmd.loadLessonsViewSignal.Dispatch(cmd.lessonsModel.lastViewedTopic);
                    return null;
                }
                else if (viewId == NavigatorViewId.LOBBY)
                {
                    cmd.loadLobbySignal.Dispatch();
                    return null;
                }
            }
            else if (evt == NavigatorEvent.SHOW_SUBSCRIPTION_DLG)
            {
                return new NSSubscriptionDlg();
            }
            else if (evt == NavigatorEvent.SHOW_LESSONS_VIEW)
            {
                return new NSLessonsView();
            }
            else if (evt == NavigatorEvent.SHOW_TOPICS_VIEW)
            {
                return new NSLessonTopics();
            }
            else if (evt == NavigatorEvent.SHOW_CHAT)
            {
                return new NSChat();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER)
            {
                return new NSMultiplayer();
            }
            else if (evt == NavigatorEvent.SHOW_INBOX)
            {
                return new NSInboxView();
            }
            else if (evt == NavigatorEvent.SHOW_ARENA)
            {
                return new NSArenaView();
            }
            else if (evt == NavigatorEvent.SHOW_SUBSCRIPTION_SALE_DLG)
            {
                return new NSSubscriptionSaleDlg();
            }

            return null;
        }
    }
}

