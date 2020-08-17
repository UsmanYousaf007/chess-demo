/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Linq;
using strange.extensions.promise.api;
using TurboLabz.InstantFramework;
using UnityEngine;

public class DownloadablesService : IDownloadablesService
{
    [Inject] public IBackendService backendService { get; set; }
    [Inject] public IDownloadablesModel downloadablesModel { get; set; }
    public IPromise<BackendResult, AssetBundle> GetDownloadableContent(string shortCode)
    {
        long lastModifiedTime = downloadablesModel.downloadableItems.Where(i => i.shortCode == shortCode).Select(i => i.lastModified).FirstOrDefault();
        return new GetDownloadableContentRequest().Send(backendService.GetDownloadableContentUrl, shortCode, lastModifiedTime);
    }

}
