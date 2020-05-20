## [2.2.5] - 2020-05-04
### Fixed
- Endless loop in configs init


# [2.2.4] - 2020-04-29
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