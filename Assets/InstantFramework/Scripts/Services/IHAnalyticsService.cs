namespace TurboLabz.InstantFramework
{
    public interface IHAnalyticsService
    {
        void LogEvent(string name);
        void LogEvent(string name, string ST1);
        void LogEvent(string name, string ST1, string ST2);
        void LogEvent(string name, string ST1, string ST2, string ST3);

        void LogMonetizationEvent(string name, int value);
        void LogMonetizationEvent(string name, int value, string ST1);
        void LogMonetizationEvent(string name, int value, string ST1, string ST2);
        void LogMonetizationEvent(string name, int value, string ST1, string ST2, string ST3);
    }
}
