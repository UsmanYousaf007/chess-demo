using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public interface IDownloadablesModel
    {
        Dictionary<string, DownloadableItem> downloadableItems { get; set; }

        void Prepare();
        bool IsUpdateAvailable(string shortCode);
        void MarkUpdated(string shortCode);
    }
}
