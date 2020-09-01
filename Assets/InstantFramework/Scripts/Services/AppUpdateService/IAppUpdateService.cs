namespace TurboLabz.InstantFramework
{
    public interface IAppUpdateService
    {
        void Init(int appVersion);
        void Terminate();
        void CheckForUpdate();
        void OnIsUpdateAvailableResult(bool isUpdateAvailable);

        //bool updateLater { get; set; }
        //bool IsUpdateAvailable();

        /*Method for getting update*/
        //void GoToStore(string url);

        //void StartUpdate(int availableVersionCode);
        //void OnUpdateDownloaded();

    }
}
