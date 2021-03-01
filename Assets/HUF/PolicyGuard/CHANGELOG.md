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