## [3.0.1] - 2020-08-27
### Fixed
- Prevented NREs during configs loading

## [3.0.0] - 2020-08-20
### Changed
- Looking for `PurposeStringConfig`  in HUFConfigs folder only
- HLogs and correct namespace for `iOSMissingPurposeStringFixer`

##### **Important**: `PurposeStringConfig` must be moved to HUFConfigs folder.

## [2.9.0] - 2020-08-21
### Added
- CanvasBlockerPanel


## [2.8.0] - 2020-08-06
### Added
- CanvasBlocker and DebugButtonsScreen


## [2.7.9] - 2020-08-06
### Added
- PostProcessBuildNumbers


## [2.7.8] - 2020-07-29
### Added
- Possibility to show log time on device using showTimeInNativeLogs
- New class HideFilesDuringBuild to hide files/directories during build process


## [2.7.7] - 2020-07-27
### Fixed
- Fixed not passing default value in PlayerPrefsSaver


## [2.7.6] - 2020-07-16
### Fixed
- Double slashes in Unity 2019.3+ Fixer


## [2.7.5] - 2020-07-09
### Added
- Abstract config removes object references when copying to json or applying it.


## [2.7.4] - 2020-07-06
### Fix
- Fix secure arrays getting values


## [2.7.3] - 2020-07-02
### Fix
- Fix build process on iOS 2019.3+


## [2.7.2] - 2020-07-02
### Fix
- Fix build process on iOS


## [2.7.1] - 2020-06-26
### Added
- unity 2019.3+ project fixer


## [2.7.0] - 2020-06-17
### Added
- BaseDummyPreprocessBuild


## [2.6.0] - 2020-06-08
### Added
- Plist Parser


## [2.5.0] - 2020-06-05
### Added
- Extended UnityEvents


## [2.4.2] - 2020-05-29
### Fixed
- Disable BITCODE for Unity 2019.3


## [2.4.1] - 2020-05-14
### Fixed
- Endless config loading loop


## [2.4.0] - 2020-05-14
### Added
- Possibility to log on iOS native log


## [2.3.1] - 2020-05-14
### Fixed
- optimized BuildConfigMap usage in ConfigsModel
- config refresh on Windows


## [2.3.0] - 2020-05-08
### Added
- `HSecureValueVault` Security system

### Deprecated
- `SecureCustomPP` as not secure enough

### Fixed
- Heading typo in changelog


## [2.2.5] - 2020-05-04
### Fixed
- Endless loop in configs init


## [2.2.4] - 2020-04-29
### Added
- Stopwatch manager for measuring performance


## [2.2.3] - 2020-04-29
### Changed
- Configs use HLog
- HLog formatting to be more descriptive


## [2.2.2] - 2020-04-17
### Fixed
- Detecting configs when building from command line


## [2.2.1] - 2020-04-06
### Fixed
- UCB config preset namespace fix


## [2.2.0] - 2020-04-03
### Changed
- FeatureConfigBase class can register Initializers with HIPs

### Fixed
- Handling HUF Configs deletion and creation in editor
- Define collector supports both UNIX and Windows


## [2.1.1] - 2020-03-25
### Added
- Added tool for applying define symbols to project from _define.hufdefine_ files under **HUF/Utils/RebuildDefines** menu
- Define symbols are applied before build


## [2.0.1] - 2020-03-18
### Added
- Added ability to deserialize plain lists in HUFJson


## [2.0.0] - 2020-03-16
### Changed
- Self Containing Package
- Namespaces

### Added
- [Required] attribute and special drawer to mark serializable fields as mandatory
- [Required] attribute allows to specify required type (for instance interfaces)
- AbstractConfigs are checked before builds to have all [Required] fields
- AbstractConfigs have custom (optional) validation methods

### Fixed
- Broken OnValidate shadowing in AbstractConfigs


## [1.9.6] - 2020-03-11
### Added
- Dispatch supports Actions and Functions

### Changed
- Dispatch's Trycatch only in editor and debug builds


## [1.9.5] - 2020-02-25
### Changed
- Android Build dose not need to have Main AndroidManifest.xml


## [1.9.4] - 2020-02-21
### Added
- Added LogImportant in HLog - enable loging in even if Log filter is will be on
- Added copying Android assets from HUF package to Plugins/Android folder
- Added new way of android manifest generation from template
- Added LogConfig to enable debug huf logs and logs filters


## [1.9.3] - 2020-02-20
### Added
- Added new class ScreenSize for geting same screen size in editor and end devices

### Fixed
- Fixed SafeAreaScaleRectTransform


## [1.9.2] - 2020-02-18
### Added
- Added new event in PauseManager to detect application focus


## [1.9.1] - 2020-02-04
### Added
- Added HLogPrefix to keep prefixes consistent and hierarchical

### Deprecated
- HLog.Log methods that do not use HLogPrefix

### Changed
- Moved Logging tools to separate namespace


## [1.9.0] - 2020-01-09
### Added
- Added package manifest and changelog.

### Changed
- Changed structure of package.