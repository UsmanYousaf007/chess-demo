---
description: ''
sidebar: 'docs'
prev: '/docs/'
---
 
# Package Manager

**version:** 1.3.0
 
## Overview
 
Package Manager is a tool that allows you to install, update and manage HUF packages that are provided by Huuuge developers. HUF provide solutions for various challenges that you encounter during development process of your game. It saves your time and give us posibility to unify solutions between a lot of games, thanks to that we have more control over quality of end product and we are in front of supporting you when any problems appears.
 
## Requirements
- Developer ID (example 485762269)
- Access Key (example hJkFPBCCuY0DqnOj0OeK8QDwpAd8VtDaSpLzjTn1)
 
## Installation
Currently this tool is supported for Unity versions 2018.4 and higher.

![License][Fig.License]

1. First thing that you need to do is open your project in Unity editor. 
2. After that you should import `.unitypackage` that you received from us.
3. When import is completed you should see new menu item called `HUF`.
4. Then just open package manager via `HUF -> Package Manager` option.

You should see license validation window.

If you have this package already installed, you can update it directly from Package Manager window inside unity.
 
## Usage
 
![Package Manager][Fig.MainView]

This is main window of Package Manager. You can see three different views:
- Toolbar at the top
- Package list on the left side
- Package info on the right side

## Toolbar

In this section you can check developer id for existing authorization token for your project, you can change sorting category, search for specific package and use some advanced options from settings dropdown at the end.

## Packages List

Package list is a scrollable area that contains all available packages depending on your license scope and sorting options. It lists simple package items that contain package name, labels, version and status icon. We explain that in next sections.

Another thing that is quite important are rollout labels. Those labels have different color than packages and contain only rollout identifier.

There are few types of rollout labels:

| Label | Description |
|:--:|:--:|
| **Experimental** | packages in early development stage or for packages that have exclusive feature not available in stable version |
| **Rollout X** | normal packages that have defined rollout version, if package is up to date it should be in higest available rollout section |
| **Development** | packages that was build by developer on local machine, it's not important section from your side |
| **Undefined** | packages that have undefined rollout tag or for packages that are older than rollout 1.9 |
| **Not Installed** | section for all available packages that are not installed |

## Package Info

Package info is a detailed informations about package. You can find there package metadata like package name, detailed version, build commit and time, rollout tag, latest available version number, path to package in your project view and detailed description of package and external SDK versions (for example ads adapters used by specific mediation). There are also options to manage that package: **install, update or remove**.

## Package Status

| Icon | Title | Description |
|:-:|-|-|
| ![][Fig.NotInstalled] | Not Installed | White square without background is for packages that are found on remote server but you don't have them in project. |  
| ![][Fig.Installed] | Installed | Filled square is used to indicate that this package is installed and up to date with remote package if it exist. |
| ![][Fig.Update] | Update Available | Orange icon means that this package have new version on remote server. |
| ![][Fig.ForceUpdate] | Mandatory Update | Red icon indicates that this update is mandatory. You should update it asap. |
| ![][Fig.Migration] | Migration | Green icon is for packages that have invalid or missing manifest, but there is remote package with same path. |
| ![][Fig.Error] | Error | If there is any error with obtaining package info this status will be displayed. |

## Issues

Sometimes there can be a need to get current package report of your project, you can simply generate log for that via settings button in a toolbar section.

![][Fig.Logs]

- **Only HUF** will generate list of HUF packages.
- **Full** is for obtaining full log including project info, HUF data and unity packages list.

You can also see **Help** option that is our helpshift form. You can contact us via your browser.
 
```cs
using System.Collections.Generic;
using HUF.Utils.Editor.BuildSupport;
using HUF.Utils.Runtime.Logging;

namespace HUF.AnalyticsHBI.Editor
{
    public class HBIDummyPreprocessBuild : BaseDummyPreprocessBuild
    {
#if HUF_ANALYTICS_FIREBASE_DUMMY
        public override bool Enabled => true;
#else
        public override bool Enabled => false;
#endif
        public override IEnumerable<string> DirectoriesToHide => new[]
        {
            "HUF/AnalyticsHBI/Plugins",
            "HUF/AnalyticsHBI/Runtime/Implementation"
        };
        public override HLogPrefix LogPrefix => new HLogPrefix(nameof(HBIDummyPreprocessBuild));
    }
}
```

<!--- IMAGE REFERENCES -->
[Fig.License]: ./../../static/package-manager/license_view.png
[Fig.MainView]: ./../../static/package-manager/main_view.png
[Fig.NotInstalled]: ./../../static/package-manager/huf_pm_not_installed.png
[Fig.Installed]: ./../../static/package-manager/huf_pm_installed.png
[Fig.Error]: ./../../static/package-manager/huf_pm_error.png
[Fig.Update]: ./../../static/package-manager/huf_pm_upgrade.png
[Fig.ForceUpdate]: ./../../static/package-manager/huf_pm_force_upgrade.png
[Fig.Migration]: ./../../static/package-manager/huf_pm_migration.png
[Fig.Logs]: ./../../static/package-manager/logs.png
 
<!--- Use following prefixes for certain reference types -->
<!--- Fig. for images -->
<!--- Ref. for referencing other files -->
<!--- Url. for referencing websites -->