## [2.6.2] - 2021-05-17
### Fixed
- Banner heights in BannerCallbackData from mock banner


## [2.6.1] - 2021-05-07
### Added
- New SKANetworks added to list.


## [2.6.0] - 2021-03-16
### Added
- Mock ads simulation on devices


## [2.5.2] - 2021-03-11
### Added
- `CollectSensitiveData` now handles lack of ATT acceptation and can open iOS app settings.


## [2.5.1] - 2021-03-08
### Added
- Events in editor banners.
- New SKANetworks added to list.


## [2.5.0] - 2021-02-17
### Added
- Function `bool? HasConsent()` to check if the ads consent is granted or set overall.
- Function `bool CanChangeAdsConsent()` to check if the ads consent can be changed.

### Changed
- IMPORTANT! The Ads consent key was changed. All old users will need to accept the new ads policy one more time. In addition to this, there is a new flow related to teh ATT permissions.
- Now function `HasPersonalizedAdConsent` is checking also the ATT permission on iOS. The ATT pop-up needs to be accepted to give the player the possibility to accept the Ads consent.


## [2.4.0] - 2021-01-25
### Added
- Adding SkAdNetworks to the Info.plist file on iOS


## [2.3.0] - 2021-01-14
### Changed
- UnityActions to Actions in Ads packages

### Fixed
- Some warnings appearing during import and usage of the package


## [2.2.0] - 2020-09-18
### Added
- Banner show event with isRefresh flag


## [2.1.1] - 2020-09-10
### Changed
- Utils dependency to an existing package


## [2.1.0] - 2020-08-20
### Added
- Ad banner with test buttons in the Editor


## [2.0.2] - 2020-08-12
### Added
- Documentation


## [2.0.1] - 2020-08-06
### Added
- Blocking input on ad debug screen in the Editor


## [2.0.0] - 2020-03-16
### Changed
- Self Containing Package
- Namespaces


## [1.10.1] - 2020-02-18
### Added
- Main HLogPrefix

### Changed
- Using HLogPrefix


## [1.10.0] - 2020-02-04
### Added
- Store GDPR Consent in PlayerPrefs
- Added OnCollectSensitiveDataSetEvent event


## [1.9.0] - 2020-01-29
### Added
- Added package manifest and changelog.
- Events for ad initialization.
- Ability to check if mediation is initialized.

### Changed
- Changed structure of package.
