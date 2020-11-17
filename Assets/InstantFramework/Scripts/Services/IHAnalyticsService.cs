using System.Collections.Generic;
//using HUF.AdsAdMobMediation.Runtime.Implementation;

namespace TurboLabz.InstantFramework
{
    public interface IHAnalyticsService
    {
        void LogEvent(string name);
        void LogEvent(string name, string ST1);
        void LogEvent(string name, string ST1, string ST2);
        void LogEvent(string name, string ST1, string ST2, string ST3);
        void LogEvent(string name, string ST1, params KeyValuePair<string, object>[] additionalParamters);
        void LogEvent(string name, string ST1, string ST2, params KeyValuePair<string, object>[] additionalParamters);
        void LogEvent(string name, string ST1, string ST2, string ST3, params KeyValuePair<string, object>[] additionalParamters);

        void LogMonetizationEvent(string name, int value);
        void LogMonetizationEvent(string name, int value, string ST1);
        void LogMonetizationEvent(string name, int value, string ST1, string ST2, string storeItemKey);
        void LogMonetizationEvent(string name, int value, string ST1, string ST2, string ST3, string storeItemKey);
        void LogMonetizationEvent(string name, int value, string ST1, string ST2, string ST3, string storeItemKey, params KeyValuePair<string, object>[] additionalParamters);

        void LogMultiplayerGameEvent(string name, string ST1, string ST2, string challengeId);
        void LogCpuGameEvent(string name, string ST1, string ST2);

        void LogAppsFlyerEvent(string name, Dictionary<string, object> eventData);
        void LogAppsFlyerMonetizationEvent(string name, int cents);
        string GetAppsFlyerId();
    }
}


