namespace TurboLabz.InstantFramework
{
    public interface IGameModesAnalyticsService
    {
        void ProcessTimeSpent(float timeSpent, MatchInfo matchInfo = null);
        void ProcessGameCount(MatchInfo matchInfo = null);
        void LogTimeSpent();
        void LogInstallDayData();
    }
}
 