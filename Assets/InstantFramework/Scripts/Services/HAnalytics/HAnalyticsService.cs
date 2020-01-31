using System.Collections.Generic;
using HUF.Analytics.API;

namespace TurboLabz.InstantFramework
{
    public class HAnalyticsService : IHAnalyticsService
    {
        //Models
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        public void LogEvent(string name)
        {
            var analyticsEvent = GetParametersDictionary();
            analyticsEvent.Add(AnalyticsEvent.EventConsts.EVENT_NAME_KEY, name);
            HAnalytics.LogEvent(analyticsEvent);
        }

        public void LogEvent(string name, string ST1)
        {
            var analyticsEvent = GetParametersDictionary();
            analyticsEvent.Add(AnalyticsEvent.EventConsts.EVENT_NAME_KEY, name);
            analyticsEvent.Add(AnalyticsEvent.EventConsts.ST1_KEY, ST1);
            HAnalytics.LogEvent(analyticsEvent);
        }

        public void LogEvent(string name, string ST1, string ST2)
        {
            var analyticsEvent = GetParametersDictionary();
            analyticsEvent.Add(AnalyticsEvent.EventConsts.EVENT_NAME_KEY, name);
            analyticsEvent.Add(AnalyticsEvent.EventConsts.ST1_KEY, ST1);
            analyticsEvent.Add(AnalyticsEvent.EventConsts.ST2_KEY, ST2);
            HAnalytics.LogEvent(analyticsEvent);
        }

        public void LogEvent(string name, string ST1, string ST2, string ST3)
        {
            var analyticsEvent = GetParametersDictionary();
            analyticsEvent.Add(AnalyticsEvent.EventConsts.EVENT_NAME_KEY, name);
            analyticsEvent.Add(AnalyticsEvent.EventConsts.ST1_KEY, ST1);
            analyticsEvent.Add(AnalyticsEvent.EventConsts.ST2_KEY, ST2);
            analyticsEvent.Add(AnalyticsEvent.EventConsts.ST3_KEY, ST3);
            HAnalytics.LogEvent(analyticsEvent);
        }

        public void LogMonetizationEvent(string name, int value)
        {
            var analyticsEvent = GetParametersDictionary();
            analyticsEvent.Add(AnalyticsEvent.EventConsts.EVENT_NAME_KEY, name);
            analyticsEvent.Add(AnalyticsEvent.EventConsts.VALUE_KEY, value);
            analyticsEvent.Add(AnalyticsMonetizationEvent.CENTS_KEY, value);
            HAnalytics.LogMonetizationEvent(analyticsEvent);
        }

        public void LogMonetizationEvent(string name, int value, string ST1)
        {
            var analyticsEvent = GetParametersDictionary();
            analyticsEvent.Add(AnalyticsEvent.EventConsts.EVENT_NAME_KEY, name);
            analyticsEvent.Add(AnalyticsEvent.EventConsts.VALUE_KEY, value);
            analyticsEvent.Add(AnalyticsMonetizationEvent.CENTS_KEY, value);
            analyticsEvent.Add(AnalyticsEvent.EventConsts.ST1_KEY, ST1);
            HAnalytics.LogMonetizationEvent(analyticsEvent);
        }

        public void LogMonetizationEvent(string name, int value, string ST1, string ST2)
        {
            var analyticsEvent = GetParametersDictionary();
            analyticsEvent.Add(AnalyticsEvent.EventConsts.EVENT_NAME_KEY, name);
            analyticsEvent.Add(AnalyticsEvent.EventConsts.VALUE_KEY, value);
            analyticsEvent.Add(AnalyticsMonetizationEvent.CENTS_KEY, value);
            analyticsEvent.Add(AnalyticsEvent.EventConsts.ST1_KEY, ST1);
            analyticsEvent.Add(AnalyticsEvent.EventConsts.ST2_KEY, ST2);
            HAnalytics.LogMonetizationEvent(analyticsEvent);
        }

        public void LogMonetizationEvent(string name, int value, string ST1, string ST2, string ST3)
        {
            var analyticsEvent = GetParametersDictionary();
            analyticsEvent.Add(AnalyticsEvent.EventConsts.EVENT_NAME_KEY, name);
            analyticsEvent.Add(AnalyticsMonetizationEvent.CENTS_KEY, value);
            analyticsEvent.Add(AnalyticsEvent.EventConsts.ST1_KEY, ST1);
            analyticsEvent.Add(AnalyticsEvent.EventConsts.ST2_KEY, ST2);
            analyticsEvent.Add(AnalyticsEvent.EventConsts.ST3_KEY, ST3);
            HAnalytics.LogMonetizationEvent(analyticsEvent);
        }

        private Dictionary<string, object> GetParametersDictionary()
        {
            var parameters = new Dictionary<string, object>();

            parameters.Add("sku", "chess");
            parameters.Add("sessions", preferencesModel.sessionCount);
            parameters.Add("games_started", preferencesModel.gameStartCount);
            parameters.Add("games_finished", preferencesModel.gameFinishedCount);
            parameters.Add("online", true);
            parameters.Add("trial_subscription_used", playerModel.subscriptionExipryTimeStamp > 0);
            parameters.Add("subscription_active", playerModel.HasSubscription() && !playerModel.isPremium);
            parameters.Add("axgroup", "");

            return parameters;
        }
    }
}
