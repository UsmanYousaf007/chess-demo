 # Package Manager
<!--- Auto:date -->**Updated:** 03-08-2021 \
<!--- Auto:version -->**version:** 1.10.0 \
[Changelog][Ref.Changelog]

**Table of contents**

1. [Overview](#Sec.Overview)
1. [Requirements](#Sec.Requirements)
1. [Installation](#Sec.Installation)
1. [Implementation](#Sec.Implementation)
1. [Config](#Sec.Config)
1. [Usage](#Sec.Usage)
1. [Technical Description](#Sec.Technical)

## Overview <a name="Sec.Overview"></a>
**Package Manager** allows to install, update and manage HUF packages that are provided by Huuuge developers. HUF provides solutions for
various challenges that are encountered during the development process of games. It saves time and gives a possibility to unify
solutions between games. Thanks to that it enables more control over quality of the end product and quick support when 
any problems appear.
 
## Requirements <a name="Sec.Requirements"></a>
- Developer ID (example 485762269)
- Access Key (example hJkFPBCCuY0DqnOj0OeK8QDwpAd8VtDaSpLzjTn1)
<!--- Auto:dependency_start -->

### Dependencies
- com.huuuge.huf.firebase.wrapper
- com.huuuge.huf.utils
- com.huuuge.huf.remoteconfig

<!--- Auto:dependency_end -->
 
## Installation <a name="Sec.Installation"></a>

1. Open the project in the editor. 
1. Import `.unitypackage` that can be downloaded from this <a href="https://drive.google.com/drive/folders/1EMwwAr7qFzsK6EzYfsfereluwIEvramU?usp=sharing">link</a>
1. When the import is completed, a new menu item called `HUF` will be created.
1. Open **Package Manager** by selecting _HUF/Package Manager_ menu.
1. Enter the credentials.
    ![License][Fig.License]
1. Accept terms and conditions.
1. After server validation succeeds, a token will be saved on the machine and **Package Manager** will be ready for use.
 
## Usage <a name="Sec.Usage"></a>
 
![Package Manager][Fig.MainView]

This is the main window of **Package Manager**. It shows three different views:
- **Toolbar** at the top
- **Package List** on the left side
- **Package Info** on the right side

**Package Manager** can be updated from this window.

### Toolbar

Toolbar shows developer ID of existing authorization token for the project, enables changing sorting category, searching for a specific package and using some advanced options from settings dropdown at the end.

### Packages List

**Package List** is a scrollable area that contains all available packages depending on the license scope and sorting options. It lists simple package items that contain package name, labels, version and status icon.

#### Types of labels:

| Label | Description |
| :---: | :---: |
| **Rollout X** | Rollout number of the package. If the package is up to date it should be in the highest available rollout section. |
| **Preview** | Packages that were tested by other developers, tested by QA and released on iOS and Android if applicable, but aren't yet verified as stable. |
| **Development** | Packages that were reviewed in pull requests and tested by its developer. |
| **Experimental** | Packages in early development stage or that have exclusive features not available in a stable version. |
| **Undefined** | Packages that have undefined rollout tag or that are older than rollout 1.9. |
| **Not Installed** | Available packages that are not yet installed. |

#### Package Status

| Icon | Title | Description |
| :---: | --- | -------- |
| ![Not Installed icon][Fig.NotInstalled] | Not Installed | White square without the background indicates that the package is found on the remote server, but is not added in the project. |  
| ![Installed icon][Fig.Installed] | Installed | Filled square indicates that the package is installed and up to date with the remote package if it exist. |
| ![Update icon][Fig.Update] | Update Available | Orange circle icon indicates that the package have a newer version on the remote server. |
| ![Force Update icon][Fig.ForceUpdate] | Required Update | Red icon indicates that the update is mandatory. It should be updated as soon as possible. |
| ![Migration icon][Fig.Migration] | Migration | An icon pointing to the right indicates that the package have an invalid or missing manifest, but there is a remote package with same path. |
| ![Error icon][Fig.Error] | Error | An icon with an exclamation mark indicates that there is an error with obtaining package info. |


### Package Info

**Package Info** shows a detailed information about the package.

On the top are buttons to manage the package: **install, update or remove**.

On the left side is a menu for changing package version and package metadata: its name, detailed version, build time and commit, rollout tag, the latest available version number, the minimum required version, the path to the package in the project view and its external SDK dependencies (for example ads adapters used by specific mediation).

On the right side is a detailed description of the package and its changelog.

### Unity Dependencies
If the package has Unity dependencies, they will be installed automatically with the package. 

![Install Unity Dependencies][Fig.InstallUnityDependencies]
To reinstall its dependencies, press `Install Unity dependencies`.
To reinstall all Unity dependencies used in the project, press `Install all Unity dependencies`.

### Preview packages

If a HUF package that isn't installed doesn't appear on the packages list, try clicking cogwheel icon and select "Show preview packages". Packages list will then include packages that aren't yet stable.

### Issues
When reporting issues to HUF append generated packages report of the project, so it will be easier to find its cause. To generate it click 
the cogwheel icon, select _GenerateReport_ and choose one of the options. The report will be saved to the system clipboard.

![Generate report][Fig.Logs]

- **Only HUF** will generate a list of HUF packages.
- **Full** will generate a full log including project info, HUF data and Unity packages list.

Click **Help** in the menu to contact HUF via a helpshift form.

## Technical Description <a name="Sec.Technical"></a>
The package uses reflections to rebuild defines if **Utils** package is present. Outside of that it does not use other HUF packages.

In Package view there are shown supported Unity versions. This is how to interpret them:
- `2018.4` - all Unity versions from 2018.4.0
- `2018.4-2019` - all Unity 2018 versions from 2018.4.0 and all Unity 2019 versions
- `2018.4-2019, 2021.2` - all Unity 2018 versions from 2018.4.0, all Unity 2019 versions and all Unity versions from 2021.2

<!--- DEFAULT REFERENCES -->
[Ref.Changelog]: ./../../../CHANGELOG.md

<!--- IMAGE REFERENCES -->
[Fig.License]: ./../../static/PackageManager/license_view.png
[Fig.MainView]: ./../../static/PackageManager/main_view.png
[Fig.NotInstalled]: ./../../static/PackageManager/huf_pm_not_installed.png
[Fig.Installed]: ./../../static/PackageManager/huf_pm_installed.png
[Fig.Error]: ./../../static/PackageManager/huf_pm_error.png
[Fig.Update]: ./../../static/PackageManager/huf_pm_upgrade.png
[Fig.ForceUpdate]: ./../../static/PackageManager/huf_pm_force_upgrade.png
[Fig.Migration]: ./../../static/PackageManager/huf_pm_migration.png
[Fig.Logs]: ./../../static/PackageManager/logs.png
[Fig.InstallUnityDependencies]: ./../../static/PackageManager/InstallUnityDependencies.png
 
<!--- Use following prefixes for certain reference types -->
<!--- Fig. for images -->
<!--- Ref. for referencing other files -->
<!--- Url. for referencing websites -->