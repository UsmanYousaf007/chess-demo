/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.promise.api;

namespace TurboLabz.InstantFramework
{
    public interface IAppUpdatesService
    {
        bool updateLater { get; set; }
        void Init();
        void Terminate();
        bool IsUpdateAvailable();
        void GoToStore(string url);
        void CheckForUpdate();
        void OnUpdateAvailable(int availableVersionCode);
        void StartUpdate(int availableVersionCode);
        void OnUpdateDownloaded();
    }
}
