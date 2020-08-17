using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDownloadablesModel
{
    List<DownloadableItem> downloadableItems { get; set; }
    DownloadableItem Get(string name);

}
