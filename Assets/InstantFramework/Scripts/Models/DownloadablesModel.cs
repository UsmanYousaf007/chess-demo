using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DownloadablesModel : IDownloadablesModel
{
    public List<DownloadableItem> downloadableItems { get; set; }
    public DownloadableItem Get(string shortCode)
    {
        return downloadableItems.Select(i => i).Where(i => i.shortCode.Equals(shortCode)).FirstOrDefault();
    }
}

public class DownloadableItem
{
    public string url { get; set; }
    public string shortCode { get; set; }
    public int size { get; set; }
    public string lastModified { get; set; }
}
