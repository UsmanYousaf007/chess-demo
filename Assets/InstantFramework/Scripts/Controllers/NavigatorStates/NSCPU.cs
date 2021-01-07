/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-12-14 13:37:03 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class NSCPU : NS
    {
        long timeAtScreenShown;

        public override void RenderDisplayOnEnter()
        {
            timeAtScreenShown = cmd.backendService.serverClock.currentTimestamp;
            ShowView(NavigatorViewId.CPU);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            cmd.gameModesAnalyticsService.ProcessTimeSpent((cmd.backendService.serverClock.currentTimestamp - timeAtScreenShown) / 1000);

            if (evt == NavigatorEvent.SHOW_CPU_EXIT_DLG)
            {
                return new NSCPUExitDlg();
            }
            else if (evt == NavigatorEvent.ESCAPE)
            {
                if (cmd.cpuGameModel.inProgress)
                {
                    return new NSCPUExitDlg(); 
                }
                else
                {
                    return new NSCPUResultsDlg();
                }
            }
            else if (evt == NavigatorEvent.SHOW_CPU_RESULTS_DLG)
            {
                return new NSCPUResultsDlg();
            }
            else if (evt == NavigatorEvent.SHOW_CPU_PROMO_DLG)
            {
                return new NSCPUPromo();
            }
            else if (evt == NavigatorEvent.SHOW_CPU_SAFE_MOVE_DLG)
            {
                return new NSCPUSafeMoveDlg();
            }
            else if (evt == NavigatorEvent.SHOW_CPU_INFO_DLG)
            {
                return new NSCPUInfoDlg();
            }
            else if (evt == NavigatorEvent.SHOW_LOBBY)
            {
                return new NSLobby();
            }
            else if (evt == NavigatorEvent.SHOW_SUBSCRIPTION_DLG)
            {
                return new NSSubscriptionDlg();
            }
            else if (evt == NavigatorEvent.SHOW_SPOT_PURCHASE)
            {
                return new NSSpotPurchase();
            }
            else if (evt == NavigatorEvent.SHOW_PROMOTION_REMOVE_ADS_DLG)
            {
                return new NSPromotionRemoveAdsDlg();
            }
            else if (evt == NavigatorEvent.SHOW_PROMOTION_REMOVE_ADS_SALE_DLG)
            {
                return new NSPromotionRemoveAdsSaleDlg();
            }
            else if (evt == NavigatorEvent.SHOW_SUBSCRIPTION_SALE_DLG)
            {
                return new NSSubscriptionSaleDlg();
            }
            else if (evt == NavigatorEvent.SHOW_SPOT_INVENTORY)
            {
                return new NSSpotInventory();
            }
            else if (evt == NavigatorEvent.SHOW_CPU_POWER_PLAY)
            {
                return new NSCPUPowerplay();
            }

            return null;
        }
    }
}

