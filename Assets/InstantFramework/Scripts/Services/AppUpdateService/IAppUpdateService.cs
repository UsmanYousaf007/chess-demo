namespace TurboLabz.InstantFramework
{
    public interface IAppUpdateService
    {
        void Init();
        void Terminate();
        void CheckForUpdate();
        void OnIsUpdateAvailableResult(bool isUpdateAvailable);

        /*Method for getting update*/
        //void GoToStore(string url);
        //void StartUpdate(int availableVersionCode);
        //void OnUpdateDownloaded();

    }
}
