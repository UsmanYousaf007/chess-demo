using System.Collections.Generic;

namespace HUFEXT.PackageManager.Editor.Models
{
    [System.Serializable]
    public enum EventType
    {
        Undefined,
        CopyDeveloperID,
        RefreshListView,
        RefreshPackages,
        ShowPreviewPackages,
        ShowUpdateWindow,
        ForceResolvePackages,
        AddScopedRegistry,
        AddDefaultRegistries,
        ClearCache,
        GenerateReportHUF,
        GenerateReportSDKs,
        GenerateReportFull,
        ContactSupport,
        RevokeLicense,
        SelectPackage,
        InstallPackage,
        RemovePackage,
        ChangePackagesChannel,
        TogglePreviewOrStableChannel,
        ChangeDevelopmentEnvPath,
        DisableDeveloperMode,
        BuildSelectedPackage,
        ShowUnityPackages,
    }
    
    [System.Serializable]
    public enum PackageManagerViewType
    {
        Unknown,
        ToolbarView,
        PackageListView,
        PackageView,
        DeveloperView
    }
    
    [System.Serializable]
    public class PackageManagerViewEvent
    {
        public PackageManagerViewType owner;
        public EventType eventType;
        public string data;
        public bool inProgress = false;
        public bool completed = false;
    }

    [System.Serializable]
    public class PackageManagerQueue
    {
        public List<PackageManagerViewEvent> events;
    }
}
