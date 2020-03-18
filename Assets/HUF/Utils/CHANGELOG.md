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