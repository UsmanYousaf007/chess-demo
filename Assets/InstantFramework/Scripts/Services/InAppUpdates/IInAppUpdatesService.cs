/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.promise.api;

namespace TurboLabz.InstantFramework
{
    public interface IInAppUpdatesService
    {
        void Init();
        bool IsUpdateAvailable();
#if UNITY_IOS
        void GoToAppStore();
#endif
#if UNITY_ANDROID
        void CheckForUpdate();
        void OnUpdateAvailable(int availableVersionCode);
        void StartUpdate(int availableVersionCode);
        void OnUpdateDownloaded();
        void DisableListeners();
#endif

    }
}
