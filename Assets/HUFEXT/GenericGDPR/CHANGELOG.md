## [2.4.6] - 2020-09-17
### Added
- Hufdefine file
- Config installer


## [2.4.5] - 2020-09-16
### Fixed
- NRE when using properties


## [2.4.4] - 2020-09-01
### Fixed
- Documentation errors

## [2.4.3] - 2020-08-17
### Added
- Documentation

## [2.4.2] - 2020-07-29
### Changed
- es texts update

## [2.4.1] - 2020-06-05
### Changed
- One frame delay on AutoInit - fix wrong GDPR displayment on some devices

## [2.4.0] - 2020-06-01
### Added
- Added optional feature to show GDPR only in specific country.

## [2.3.4] - 2020-05-21
### Fixed
- Missing characters in Japanese and Korean font assets

## [2.3.3] - 2020-05-13
### Changed
- Changed header text

## [2.3.2] - 2020-05-12
### Fixed
- HIP initialization check differing from autoinit ones

## [2.3.1] - 2020-04-22
### Added
- Added support for Unity system language API.

### Fixed
- Fixed issue with translations on ios.

## [2.3.0] - 2020-04-15
### Added
- Added ability to force language in editor and build.

### Changed
- Moved font assets to HUFEXT/GenericGDPR/Fonts directory.
- Moved font configuration from GDPR config to GDPRView component in prefab.

## [2.2.0] - 2020-03-30
### Added
- Added link for advertising network companies
- Added Initialization method with prefab as parameter

### Changed
- Dispose method will be called only if DestroyOnAccept flag is enabled

## [2.1.1] - 2020-03-30
### Changed
- fixed namespaces

## [2.1.0] - 2020-03-20
### Added
- Custom prefs key for personalized ads.
- Translations for following languages: en, pl, it, es, es-mx, ja, fr, de, ko, pt, pt-br, sv, no and ru.
- Custom fonts configuration in GDPR config.
- Event when policy window is opened.

### Changed
- Small adjustments in GDPR prefab.

## [2.0.1] - 2020-03-18
### Fixed
- GDPR window will not appear when personalized ads consent was revoked.

## [2.0.0] - 2020-02-26
### Added
- Analytics and ads consents are set automatically by GDPR component.

### Changed
- Breaking code changes.
- Updated policy text.
- New UI design.

## [1.1.1] - 2020-01-08
### Added
- Added changelog file.

### Fixed
- Moved OnPolicyAccepted callback invocation after player prefs flags are set.

## 3rd Party licenses:
### This package use NotoSans font family.

Copyright (c) 2020, Google Inc.

This Font Software is licensed under the SIL Open Font License, Version 1.1.

This license is located under Assets/HUFEXT/GenericGDPR/Fonts directory, and is also available with a FAQ at: https://scripts.sil.org/OFL