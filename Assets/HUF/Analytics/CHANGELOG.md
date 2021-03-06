## [2.5.1] - 2021-04-29
### Fixed
- Performance when logging is turned off


## [2.5.0] - 2021-03-04
### Added 
- LogSuccessfulPurchaseEvent and made LogMonetizationEvent obsolete


## [2.4.0] - 2021-02-17
### Removed 
- ATT plugins was moved to the Policy Guard package.

### Changed
- IMPORTANT! GDPR consent key was changed. All old users will need to accept the new GDPR policy one more time.


## [2.3.3] - 2020-08-20
### Added
- check current ATT status


## [2.3.2] - 2020-08-20
### Fixed
- crash on selecting options in native ATT popup


## [2.3.1] - 2020-08-20
### Fixed
- Unity 2019.2 and lower build fix for iOS


## [2.3.0] - 2020-08-20
### Added
- iOS 14 App Tracking Transparency Support


## [2.2.4] - 2020-09-03
### Changed
- Return type of TryRegisterService


## [2.2.3] - 2020-08-26
### Fixed
- Issue preventing cloud package builder from attaching the documentation


## [2.2.2] - 2020-08-12
### Fixed
- Change limit of cached events for AppsFlyer to small number


## [2.2.1] - 2020-08-10
### Fixed
- Checking GDPR instead of Personalized Ads consent


## [2.2.0] - 2020-07-20
### Changed
- Changed flow or running analytics modules and caching events


## [2.1.0] - 2020-06-16
### Added
- Support for dummy services


## [2.0.1] - 2020-05-21
### Fixed
- Improved initialization reliability


## [2.0.0] - 2020-04-02
### Added
- Service registration with callback
- Missing PM dependencies

### Changed
- Self Containing Package
- Namespacesfeature

### Removed
- Define from HAnalytics


## [1.11.0] - 2020-02-20
### Added
- Cache events if GDPR consent is null or false.

### Changed
- Services are initialized after CollectSensitiveData method invoke.


## [1.10.0] - 2020-02-04
### Added
- Store GDPR Consent in PlayerPrefs.

### Changed
- Changed structure of OnCollectSensitiveDataSetEvent event.


## [1.9.0] - 2020-01-13
### Added
- Added package manifest and changelog.

### Changed
- Changed structure of package.