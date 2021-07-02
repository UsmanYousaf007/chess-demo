## [3.14.1] - 2021-06-24
### Added
- New string extensions


## [3.14.0] - 2021-06-11
### Added
- Applying selected JSON array element to a config
- Breaking up JSON array to parts in HUFMiniJson


## [3.13.3] - 2021-06-08
### Fixed
- iOS Debug flag adding on Unity 2019+


## [3.13.2] - 2021-06-08
### Fixed
- ExtractCorePrefix ArgumentOutOfRange exception


## [3.13.1] - 2021-06-08
### Fixed
- ExtractCoreNamespace null reference exception


## [3.13.0] - 2021-05-30
### Changed
- Usage of LogPrefix is obsolete and will be eventually removed
- Log prefixes are generated automagically


## [3.12.1] - 2021-05-06
### Fixed
- Checking if Notifications Firebase is installed when MessagingUnityPlayerActivity is used


## [3.12.0] - 2021-04-29
### Added
- Providing function as message to HLog


## [3.11.2] - 2021-04-27
### Fixed
- Warning in Unity 2021 caused by creating deprecated `mcs.rsp` file


## [3.11.1] - 2021-04-26
### Changed
- Moved OnBackButtonPress from PauseManager to a new InteractionManager class


## [3.11.0] - 2021-04-20
### Added
- Unity 2020.3+ Build fix button


## [3.10.2] - 2021-04-16
### Fixed
- `FrameworksToAddToMainTarget` property in `iOSProjectBaseFrameworkManager` for Unity 2019.3+  changed to virtual


## [3.10.1] - 2021-04-07
### Changed
- Made iOS fixer `projectPath` protected
- Made Dictionary helper


## [3.10.0] - 2021-03-18
## Warning
- `AbstractConfig` **class now implements virtual `Reset` method. If any custom inheritor defines it too, it must be changed into an override.**

### Added
- Button for applying only non-empty properties from a default preset (if any)

### Removed
- Unused, derelict property `CallbackOrder` of `AbstractConfig`.


## [3.9.1] - 2021-03-19
### Added
- Possibility to add iOS frameworks to both main and framework target on Unity 2019.3+


## [3.9.0] - 2021-03-11
### Added
- Expanded GameServerResponse class to eliminate repeating code patterns


## [3.8.8] - 2021-03-09
### Fixed
- A warning


## [3.8.7] - 2021-03-08
### Fixed
- Config preset error during build process when config is empty.
- `AsyncRequestCoroutine` is waiting for all retries to end in the `GameServerUtils` class  


## [3.8.6] - 2021-02-26
### Fixed
- Safe Area scaling


## [3.8.5] - 2021-02-25
### Fixed
- Added missing reference to DisablingAlwaysEmbedSwiftStandardLibrariesiOS


## [3.8.4] - 2021-02-17
### Fixed
- Swift library errors and warnings when uploading to App Store


## [3.8.3] - 2021-02-17
### Fixed
- AAB build support string fixed


## [3.8.2] - 2021-02-17
### Fixed
- HLog config not required


## [3.8.1] - 2021-02-15
### Fixed
- Warning drawers are autofitted to their contents


## [3.8.0] - 2021-02-04
### Added
- IntervalManager

### Fixed
- Logs formatting for TextMeshPro


## [3.7.1] - 2021-02-03
### Added
- Build android ABB from command line.


## [3.7.0] - 2021-02-03
### Added
- TryGetConfig method


## [3.6.1] - 2021-01-29
### Fixed
- SafeArea not refreshing correctly


## [3.6.0] - 2021-01-29
### Added
- Checking if the main AndroidManifest activity is correct


## [3.5.2] - 2021-01-23
### Added
- Back button event in Pause Manager


## [3.5.1] - 2021-01-22
### Added
- Option to invert log filter (to exclude certain logs)


## [3.5.0] - 2021-01-21
### Added
- A way to stop most of HUF logs while debugging issues (only in debug builds) 


## [3.4.3] - 2021-01-08
### Fixed
- Null reference when LogConfig is not present


## [3.4.2] - 2021-01-08
### Fixed
- Logging on production


## [3.4.1] - 2021-01-08
### Fixed
- CoroutineManager null reference error when quitting in the editor
- ProjectBuilder compilation error on Windows


## [3.4.0] - 2021-01-08
### Changed
- Moved some GameServer classes to Utils


## [3.3.3] - 2021-01-07
### Changed
- ConfigsModel implements `IPreprocessBuildWithReport` instead of `AbstractConfig`


## [3.3.2] - 2020-12-15
### Fixed
- Path separators in rsp file comments are now '/' on all platforms


## [3.3.1] - 2020-11-19
### Added
- `ProjectBuilder` script to support local and CI game building


## [3.3.0] - 2020-11-03
### Added
- HUFJson support for `ISerializationCallbackReceiver`
- Made UTC start date public in `DateTimeUtils`
- Made serialization of string-thing dictionaries possible


## [3.2.0] - 2020-10-30
### Added
- UI None drawing graphic
- MultiLanguageText class for translations purposes
- Dictionary<string,string> serialization and deserialization


## [3.1.0] - 2020-10-14
### Added
- Generating permissions in AndroidManifestAutofixer using new attribute

### Fixed
- Error about missing directory in HUFBuildAssetsResolver.CreateProjectPropertyFile method


## [3.0.5] - 2020-10-03
### Added
- Documentation

### Fixed
- PauseManager OnAppPause and OnApplicationFocus in the editor


## [3.0.4] - 2020-09-24
### Added
- Opening application settings on iOS


## [3.0.3] - 2020-09-10
### Added
- Compatibility with GameServer Profile Services


## [3.0.2] - 2020-08-21
### Added
- CanvasBlockerPanel


## [3.0.1] - 2020-08-27
### Fixed
- Prevented NREs during configs loading

## [3.0.0] - 2020-08-20
### Changed
- Looking for `PurposeStringConfig`  in HUFConfigs folder only
- HLogs and correct namespace for `iOSMissingPurposeStringFixer`

##### **Important**: `PurposeStringConfig` must be moved to HUFConfigs folder.


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
