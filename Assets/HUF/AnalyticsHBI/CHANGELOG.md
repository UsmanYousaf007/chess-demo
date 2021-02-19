## Internal SDK
HDS SDK Android v1.3.1
HDS SDK iOS v1.3.1


## [2.5.2] - 2021-02-03
### Fixed
- Removed unnecessary warnings for type casting


## [2.5.1] - 2021-01-19
### Fixed
- Trying to download PlayerAttributes with an unfilled config


## [2.5.0] - 2021-01-14
### Added
- Logging event to AppsFlyer when HDS revenue level changes


## [2.4.1] - 2021-01-13
### Fixed
- Utils dependency


## [2.4.0] - 2020-12-07
### Added 
- Getting player attributes and ARPI level events


## [2.3.1] - 2020-12-15
### Changed
-  using dispose instead of destructor in managed library


## [2.3.0] - 2020-11-25
### Changed
- HDS SDK updated to 1.3.1
### Added
-  Support for App Tracking Transparency 


## [2.2.7] - 2020-11-27
### Fixed
- Fix for native OSX library on 2018.4 - unsupported file extension


## [2.2.6] - 2020-10-28
### Fixed
- Removed unicode characters from documentation


## [2.2.5] - 2020-09-09
### Added
- HDS SDK updated to 1.2.2
- Reset session id if application is unfocused (minimized) for longer than 5 minutes
- A different duration may be set using newly added function SetMinutesToResetSessionId


## [2.2.5] - 2020-09-03
### Changed
- Initializing the module only once


## [2.2.4] - 2020-08-26
### Fixed
- Issue preventing cloud package builder from attaching the documentation


## [2.2.3] - 2020-07-24
### Added
- Parameters with invalid values (null, empty string, NaN) will not be logged


## [2.2.2] - 2020-07-13
### Added
- Documentation


## [2.2.1] - 2020-07-09
### Changed
- HDS SDK iOS updated to 1.2.1


## [2.2.0] - 2020-06-25
### Added
- Dummy service


## [2.1.0] - 2020-06-01
### Added
- Error logs if accessing uninitialized service
- Exposed Session ID

### Changed
- HBI to HDS in descriptions


## [2.0.2] - 2020-05-28
### Changed
- HBI SDK updated to 1.2.0


## [2.0.1] - 2020-05-04
### Changed
- HBI SDK updated to 1.1.0


## [2.0.0] - 2020-04-02
### Added
- HIP support

### Changed
- Self Containing Package
- Namespaces

### Fixed
- Service reports initialization failure

### Removed
- Define from HAnalyticsHBI


## [1.10.0] - 2020-02-20
### Changed
- Adapt package to new analytics core (HUF Analytics 1.11.0).


## [1.9.0] - 2020-01-13
### Added
- Added package manifest and changelog.

### Changed
- Changed structure of package.
