using System;
using System.Collections.Generic;
//using HUF.AdsAdMobMediation.Runtime.Implementation;
using HUF.Analytics.Runtime.API;
using HUF.AnalyticsAppsFlyer.Runtime.API;
using TurboLabz.Chess;
using TurboLabz.InstantGame;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class HAnalyticsService : IHAnalyticsService
    {
        //Models
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ICPUStatsModel statsModel { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public Multiplayer.IChessboardModel chessboardModel { get; set; }
        [Inject] public CPU.IChessboardModel cpuChessboardModel { get; set; }
        [Inject] public CPU.ICPUGameModel cpuGameModel { get; set; }

        //Services
        [Inject] public IAnalyticsService analyticsService { get; set; }

        private Dictionary<string, object> analyticsEvent;

        public void LogEvent(string name)
        {
            analyticsEvent = new Dictionary<string, object>();
            _LogEvent(name);
            analyticsService.HEvent(name);
        }

        public void LogEvent(string name, string ST1)
        {
            analyticsEvent = new Dictionary<string, object>();
            _LogEvent(name, ST1);
            analyticsService.HEvent(name, ST1);
        }

        public void LogEvent(string name, string ST1, string ST2)
        {
            analyticsEvent = new Dictionary<string, object>();
            _LogEvent(name, ST1, ST2);
            analyticsService.HEvent(name, ST1, ST2);
        }

        public void LogEvent(string name, string ST1, string ST2, string ST3)
        {
            analyticsEvent = new Dictionary<string, object>();
            _LogEvent(name, ST1, ST2, ST3);
            analyticsService.HEvent(name, ST1, ST2, ST3);
        }

        public void LogEvent(string name, string ST1, params KeyValuePair<string, object>[] additionalParamters)
        {
            analyticsEvent = new Dictionary<string, object>();
            _LogEvent(name, ST1, additionalParamters);
            analyticsService.HEvent(name, ST1);
        }

        public void LogEvent(string name, string ST1, string ST2, params KeyValuePair<string, object>[] additionalParamters)
        {
            analyticsEvent = new Dictionary<string, object>();
            _LogEvent(name, ST1, ST2, additionalParamters);
            analyticsService.HEvent(name, ST1, ST2);
        }

        public void LogEvent(string name, string ST1, string ST2, string ST3, params KeyValuePair<string, object>[] additionalParamters)
        {
            analyticsEvent = new Dictionary<string, object>();
            _LogEvent(name, ST1, ST2, ST3, additionalParamters);
            analyticsService.HEvent(name, ST1, ST2, ST3);
        }

        private void _LogEvent(string name)
        {
            analyticsEvent.Add(AnalyticsEvent.EventConsts.EVENT_NAME_KEY, name);
            AddDefaultParameters();
            HAnalytics.LogEvent(analyticsEvent, AnalyticsServiceName.HBI);
            analyticsEvent = null;
        }

        private void _LogEvent(string name, string ST1)
        {
            analyticsEvent.Add(AnalyticsEvent.EventConsts.ST1_KEY, ST1);
            _LogEvent(name);
        }

        private void _LogEvent(string name, string ST1, string ST2)
        {
            analyticsEvent.Add(AnalyticsEvent.EventConsts.ST2_KEY, ST2);
            _LogEvent(name, ST1);
        }

        private void _LogEvent(string name, string ST1, string ST2, string ST3)
        {
            analyticsEvent.Add(AnalyticsEvent.EventConsts.ST3_KEY, ST3);
            _LogEvent(name, ST1, ST2);
        }

        private void _LogEvent(string name, string ST1, params KeyValuePair<string, object>[] additionalParamters)
        {
            AddParameters(additionalParamters);
            _LogEvent(name, ST1);
        }

        private void _LogEvent(string name, string ST1, string ST2, params KeyValuePair<string, object>[] additionalParamters)
        {
            AddParameters(additionalParamters);
            _LogEvent(name, ST1, ST2);
        }

        private void _LogEvent(string name, string ST1, string ST2, string ST3, params KeyValuePair<string, object>[] additionalParamters)
        {
            AddParameters(additionalParamters);
            _LogEvent(name, ST1, ST2, ST3);
        }

        public void LogMonetizationEvent(string name, int value)
        {
            analyticsEvent = new Dictionary<string, object>();
            _LogMonetizationEvent(name, value);
            analyticsService.HEvent(name);
        }

        public void LogMonetizationEvent(string name, int value, string ST1)
        {
            analyticsEvent = new Dictionary<string, object>();
            _LogMonetizationEvent(name, value, ST1);
            analyticsService.HEvent(name, ST1);
        }

        public void LogMonetizationEvent(string name, int value, string ST1, string ST2)
        {
            analyticsEvent = new Dictionary<string, object>();
            _LogMonetizationEvent(name, value, ST1, ST2);
            analyticsService.HEvent(name, ST1, ST2);
        }

        public void LogMonetizationEvent(string name, int value, string ST1, string ST2, string ST3)
        {
            analyticsEvent = new Dictionary<string, object>();
            _LogMonetizationEvent(name, value, ST1, ST2, ST3);
            analyticsService.HEvent(name, ST1, ST2, ST3);
        }

        public void LogMonetizationEvent(string name, int value, string ST1, string ST2, string ST3, params KeyValuePair<string, object>[] additionalParamters)
        {
            analyticsEvent = new Dictionary<string, object>();
            _LogMonetizationEvent(name, value, ST1, ST2, ST3, additionalParamters);
            analyticsService.HEvent(name, ST1, ST2, ST3);
        }
    
        private void _LogMonetizationEvent(string name, int value)
        {
            analyticsEvent.Add(AnalyticsEvent.EventConsts.EVENT_NAME_KEY, name);
            analyticsEvent.Add(AnalyticsEvent.EventConsts.VALUE_KEY, value);
            analyticsEvent.Add(AnalyticsMonetizationEvent.CENTS_KEY, value);
            AddDefaultParameters();
            HAnalytics.LogMonetizationEvent(analyticsEvent, AnalyticsServiceName.HBI);
            analyticsEvent = null;
        }

        private void _LogMonetizationEvent(string name, int value, string ST1)
        {
            analyticsEvent.Add(AnalyticsEvent.EventConsts.ST1_KEY, ST1);
            _LogMonetizationEvent(name, value);
        }

        private void _LogMonetizationEvent(string name, int value, string ST1, string ST2)
        {
            analyticsEvent.Add(AnalyticsEvent.EventConsts.ST2_KEY, ST2);
            AddMonetizationParameters(ST2);
            _LogMonetizationEvent(name, value, ST1);
        }

        private void _LogMonetizationEvent(string name, int value, string ST1, string ST2, string ST3)
        {
            analyticsEvent.Add(AnalyticsEvent.EventConsts.ST3_KEY, ST3);
            _LogMonetizationEvent(name, value, ST1, ST2);
        }

        private void _LogMonetizationEvent(string name, int value, string ST1, string ST2, string ST3, params KeyValuePair<string, object>[] additionalParamters)
        {
            AddParameters(additionalParamters);
            _LogMonetizationEvent(name, value, ST1, ST2, ST3);
        }

        public void LogMultiplayerGameEvent(string name, string ST1, string ST2, string challengeId)
        {
            analyticsEvent = new Dictionary<string, object>();
            AddMultiplayerGameParamters(name, challengeId);
            _LogEvent(name, ST1, ST2);
            analyticsService.HEvent(name, ST1, ST2);
        }

        public void LogCpuGameEvent(string name, string ST1, string ST2)
        {
            analyticsEvent = new Dictionary<string, object>();
            AddCpuGameParameters(name);
            _LogEvent(name, ST1, ST2);
            analyticsService.HEvent(name, ST1, ST2);
        }


        private void AddDefaultParameters()
        {
            analyticsEvent.Add("sku", "chess");
            analyticsEvent.Add("sessions", preferencesModel.sessionCount);
            analyticsEvent.Add("games_started", preferencesModel.gameStartCount);
            analyticsEvent.Add("games_finished", preferencesModel.gameFinishedCount);
            analyticsEvent.Add("online", true);
            analyticsEvent.Add("trial_subscription_used", playerModel.subscriptionExipryTimeStamp > 0);
            analyticsEvent.Add("axgroup", "");
            analyticsEvent.Add("sub_days_left", Math.Max((TimeUtil.ToDateTime(playerModel.subscriptionExipryTimeStamp) - DateTime.UtcNow).TotalDays, 0).ToString("0"));
            analyticsEvent.Add("player_level", statsModel.GetHighestDifficultyLevelBeaten());
            analyticsEvent.Add("power", playerModel.eloScore);
        }

        private void AddMonetizationParameters(string ST2)
        {
            var isYearly = ST2.ToLower().Contains("year");
            var storeItemKey = isYearly ? "SubscriptionAnnual" : "Subscription";
            var storeItem = metaDataModel.store.items[storeItemKey];

            analyticsEvent.Add("funnel_instance_id", string.Concat(playerModel.id, metaDataModel.store.lastPurchaseAttemptTimestamp));
            analyticsEvent.Add("iap_currency_id", storeItem.remoteProductCurrencyCode);
            analyticsEvent.Add("iap_amount", (double)storeItem.productPrice);
            analyticsEvent.Add("original_amount",(double)storeItem.originalPrice);
            analyticsEvent.Add("discount_perc", (int)(storeItem.discountedRatio * 100));
            analyticsEvent.Add("is_best_value", isYearly);
        }

        private void AddMultiplayerGameParamters(string name, string challengeId)
        {
            if (!matchInfoModel.matches.ContainsKey(challengeId))
            {
                return;
            }

            var gameInfo = matchInfoModel.matches[challengeId];
            var opponentId = gameInfo.challengerId.Equals(playerModel.id) ? gameInfo.challengedId : gameInfo.challengerId;

            analyticsEvent.Add("funnel_instance_id", string.Concat(gameInfo.challengerId, gameInfo.challengedId));
            analyticsEvent.Add("opponenet_player_id", opponentId);
            analyticsEvent.Add("difficulty", gameInfo.opponentPublicProfile.eloScore);
            analyticsEvent.Add("theme", playerModel.activeSkinId);

            if (name.Equals("game_finished"))
            {
                var endGameResult = gameInfo.winnerId == playerModel.id ? EndGameResult.PLAYER_WON :
                    gameInfo.winnerId == opponentId ? EndGameResult.OPPONENT_WON : EndGameResult.GAME_DRAWN;

                analyticsEvent.Add("gameplay_result", endGameResult.ToString());
                analyticsEvent.Add("score", gameInfo.playerEloScoreDelta);
                analyticsEvent.Add("duration", (int)gameInfo.gameDurationMs/1000);

                if (chessboardModel.chessboards.ContainsKey(challengeId))
                {
                    var chessboard = chessboardModel.chessboards[challengeId];
                    analyticsEvent.Add("steps_used", chessboard.moveList.Count);
                    analyticsEvent.Add("time_left", chessboard.backendPlayerTimer.TotalSeconds);
                    analyticsEvent.Add("end_type", chessboard.gameEndReason.ToString());
                }
            }
        }

        private void AddCpuGameParameters(string name)
        {
            analyticsEvent.Add("opponenet_player_id", "cpu");
            analyticsEvent.Add("difficulty", cpuGameModel.cpuStrength);
            analyticsEvent.Add("theme", playerModel.activeSkinId);

            if (name.Equals("game_finished"))
            {
                var endGameResult = cpuChessboardModel.winnerId == playerModel.id ? EndGameResult.PLAYER_WON :
                    cpuChessboardModel.gameEndReason == GameEndReason.STALEMATE || cpuChessboardModel.gameEndReason == GameEndReason.DRAW_BY_INSUFFICIENT_MATERIAL ? EndGameResult.GAME_DRAWN : EndGameResult.OPPONENT_WON;
                
                analyticsEvent.Add("gameplay_result", endGameResult.ToString());
                analyticsEvent.Add("steps_used", cpuChessboardModel.moveList.Count);
                analyticsEvent.Add("end_type", cpuChessboardModel.gameEndReason.ToString());
            }
        }

        private void AddParameters(params KeyValuePair<string, object>[] paramters)
        {
            foreach (var parameter in paramters)
            {
                analyticsEvent.Add(parameter.Key, parameter.Value);
            }
        }

        public void LogAppsFlyerEvent(string name, Dictionary<string, object> eventData)
        {
            analyticsEvent = new Dictionary<string, object>();
            analyticsEvent.Add(AnalyticsEvent.EventConsts.EVENT_NAME_KEY, name);

            foreach (var e in eventData)
            {
                analyticsEvent.Add(e.Key, e.Value);
            }

            HAnalytics.LogEvent(analyticsEvent, AnalyticsServiceName.APPS_FLYER);
            analyticsEvent = null;
        }

        public void LogAppsFlyerMonetizationEvent(string name, int cents)
        {
            analyticsEvent = new Dictionary<string, object>();
            analyticsEvent.Add(AnalyticsEvent.EventConsts.EVENT_NAME_KEY, name);
            analyticsEvent.Add(AnalyticsEvent.EventConsts.VALUE_KEY, cents);
            analyticsEvent.Add(AnalyticsMonetizationEvent.CENTS_KEY, cents);
            HAnalytics.LogMonetizationEvent(analyticsEvent, AnalyticsServiceName.APPS_FLYER);
            analyticsEvent = null;
        }


        public string GetAppsFlyerId()
        {
            return string.IsNullOrEmpty(HAnalyticsAppsFlyer.UserId) ? string.Empty : HAnalyticsAppsFlyer.UserId;
        }
    }
}
