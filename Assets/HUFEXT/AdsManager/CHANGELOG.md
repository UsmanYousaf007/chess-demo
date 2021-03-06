## [1.6.1] - 2021-03-08
### Added
- New event to communicate that the service is initialized.
- The `IsAdReady` function added.

### Fixed
- Fixed `ShowBannerPersistent` function to be able to be set before any ad is loaded.


## [1.6.0] - 2021-01-14
### Changed
- UnityActions to Actions in Ads packages


##[1.5.4] - 2020-12-30
## Fixed
- BaseAdMediation warning


##[1.5.3] - 2020-12-10
## Fixed
- Send correct placement id when trying to show all ad types.


##[1.5.2] - 2020-12-09
## Fixed
- Send correct placement id when trying to show rewarded ad.


##[1.5.1] - 2020-12-09
## Fixed
- Editor ads when using AppLovin MAX


##[1.5.0] - 2020-09-18
## Changed
- Handling change in banner show event


## [1.4.5] - 2020-09-17
### Added
- Config installer

### Changed
- Hufdefine file
- Documentation
- Code refactoring


## [1.4.4] - 2020-08-18
### Fixed
- Handling null placementId value in HUFAdsService


## [1.4.3] - 2020-08-04
### Fixed
- Fixed not showing banners and setting banner as showing when it was fetched


## [1.4.2] - 2020-07-15
### Fixed
- Freeze hiding banner when banner view is created for first time


## [1.4.1] - 2020-04-16
### Fixed
- Freeze on show banner with fail response


## [1.4.0] - 2020-04-02
### Added
- HIP support
- Init log


## [1.3.0] - 2020-03-21
### Change
- Support for more mediations


## [1.1.3] - 2020-02-21
### Fixed
- Disconnect old ads events, when changing config

### Removed
- Removed creating new ads on consent status change (related to disconnection old ads events) 


## [1.1.2] - 2020-02-21
### Added
- Added check if ad did not expire


## [1.1.1] - 2020-02-20
### Change
- Allow call HideBanner event before ad will show, but it need to be created


## [1.1.0] - 2020-02-04
### Added
- Reinit on consent changed to false


## [1.0.0] - 2020-01-10
### Added
- Base implementation of Ads Manager
