## [1.3.1] - 2021-05-17
### Fixed
- Fixed overriding setting if changed when building from commandline or CI


## [1.3.0] - 2021-04-28
### Added
- Automatic handling of player Anonymization


## [1.2.2] - 2021-04-28
### Fixed
- Removed text validator error on null config.


## [1.2.1] - 2021-03-31
### Fixed
- Hardcoded color in personalized ads text.
- iOS compilation error.


## [1.2.0] - 2021-03-23
### Added
- Possibility to add the event system for UI if it does not exist using the `creatUnityUIEventSystemIfNotExist` flag.
- Pop-up text validator to check if the pop-up texts were changed on purpose. 
- Custom pop-up sample.

### Changed
- IMPORTANT! Links color was removed and there is a new tag for color `{COLOR}`.

### Removed
- Links color in policy guard config.
- Links color in pop-up config.


## [1.1.1] - 2021-03-18
### Added
- Possibility to set a custom links color in policy guard config.
- Possibility to set a pop-ups custom flow in policy guard config.

### Fixed
- The GDPR links colors.


## [1.1.0] - 2021-03-12
### Added
- Possibility to set custom prefab for pop-ups in policy guard config.
- Possibility to delay closing pre ATT popup after native ATT is accepted.

### Changed
- Prefabs clean-up.

### Fixed
- Build warning clean-up.


## [1.0.3] - 2021-02-25
### Fixed
- Prefabs compatible with Unity 2018.4.
- Fixed personalized ads popup showing after the GDPR with ads.
- Fixed showing pre-ATT if user set to never allow ATT permission.
- Fixed event `OnEndCheckingPolicy` not called always.


## [1.0.2] - 2021-02-18
### Changed
- Policy's flow, from now the GDPR is first.
- The config variable changed from `showAdsConsentAfterGDPR` to `showAdsConsent`

### Added
- New event `OnEndCheckingPolicy` to communicate the checking flow has ended.


## [1.0.1] - 2021-02-18
### Fixed
- Spelling errors in documentation
- Building for Android

### Added
- Define symbol for testing in the editor in the module's define file (commented out by default)


## [1.0.0] - 2021-02-18
Initial commit
