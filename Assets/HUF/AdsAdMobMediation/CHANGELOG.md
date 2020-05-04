## Internal SDK
- Google Mobile Ads Unity Mediation - v5.0.1
- Google Play services - v19.0.0
- Google Mobile Ads iOS SDK - v7.56.0

- Vungle Unity Adapter - v3.2.0
- Vungle Native Adapter (Android) - v6.4.11.1
- Vvungle SDK (Android) - v6.4.11
- Vungle Native Adapter (iOS) - v6.4.11.1
- Vungle SDK (iOS) - v6.11.4

- AppLovin Unity Adapter - v4.4.1
- AppLovin Native Adapter (Android) - v9.11.4.0
- AppLovin SDK (Android) - v9.11.4
- AppLovin Native Adapter (iOS) - v4.4.0
- AppLovin SDK (iOS) - v6.11.1

- Facebook Unity Adapter - v2.8.0
- Facebook Native Adapter (Android) - v5.8.0.0
- Facebook Native Adapter (iOS) - v5.8.0.1
- Facebook SDK - v5.8.0
- FAN SDK - v5.8.0

- Ironsource Unity Adapter - v1.5.0
- Ironsource Native Adapter - v6.13.0.1.0
- Ironsource SDK - v6.13.0.1.0

- AdColony Unity Adapter - v2.0.1
- AdColony Native Adapter (Android) - v4.1.4.0
- AdColony SDK (Android) - v4.1.4
- AdColony Native Adapter (iOS) - v4.1.4.0
- AdColony SDK (iOS) - v4.1.4

- Chartboost Unity Adapter - v2.0.1
- Chartboost Native Adapter (Android) - v7.5.0.1
- Chartboost SDK (Android) - v7.5.0
- Chartboost Native Adapter (iOS) - v8.0.4.0
- Chartboost SDK (iOS) - v8.0.4

- InMobi Unity Adapter - v2.4.0
- InMobi Native Adapter (Android) - v7.3.0.1
- InMobi SDK (Android) - v7.3.0
- InMobi Native Adapter (iOS) - v7.4.0.0
- InMobi SDK (iOS) - v7.4.0

- Tapjoy Unity Adapter - v2.4.1
- Tapjoy Native Adapter - v12.4.2.0
- Tapjoy SDK - v12.4.2

- Unity Ads Unity Adapter - v2.4.1
- Unity Ads Native Adapter - v3.4.2.0
- Unity Ads SDK - v3.4.2

- Mopub Unity Adapter - v2.9.0
- Mopub Native Adapter - v5.10.0.0
- MoPub SDK - v5.10.0.0

- Verizon Unity Adapter - v1.2.0
- Verizon Native Adapter - v1.4.0.0
- Verizon SDK - v1.4.0

- Mediation Test Suite Unity - v1.3.0
- Mediation Test Suite Native SDK - v1.3.0

- Ogury Native Adapter (iOS) - v03.02.2020
- Ogury SDK (iOS) - v1.0.8
- Ogury Native Adapter (Android) - v4.1.2
- Ogury SDK (Android) - v5.1.0

- Mintegral Native Adapter (iOS) - v9.8.0 - for now removed
- Mintegral SDK (iOS) - v5.8.7 - for now removed
- Mintegral Native Adapter (Android) - v2.2.4
- Mintegral SDK (Android) - v5.8.7

## [2.0.4] - 2020-04-21
### Removed
- Mintegral removed due to pod incompatible change

## [2.0.3] - 2020-04-17
### Fixed
- Detecting configs when building from command line


## [2.0.2] - 2020-04-08
### Changed
- Paid event adjusted to HBI/HDS requirements


## [2.0.1] - 2020-04-08
### Added
- Option to pause/unpause game on iOS during ad play

### Changed
- Way of getting test device Id

### Removed
- Yandex - not supporting google-ads 19.0

### Updated
- Facebook Adapter v2.8.0


## [2.0.0] - 2020-04-02
### Changed
- Self Containing Package
- Namespaces

### Added
- Code to get test device id for ads 
- HIP support
- Init log

### Added EXPERIMENTAL
- Verizon Unity Adapter - v1.2.0

### Updated
- Google Mobile Ads v5.0.1
- AppLovin Adapter v4.4.1
- Facebook Adapter v2.7.1
- AdColonyAdapter v2.0.1
- Tapjoy Adapter v2.4.1
- Unity Ads Adapter - v2.4.1
- Verizon Adapter - v1.2.0
- Mediation Test Suite - v1.3.0
- Ogury - removed UIWebView


## [1.10.1] - 2020-03-02
### Fixed
- Rewarded ad completed event on ad close


## [1.10.0] - 2020-02-03
### Added
- Auto fixer for configs

### Added EXPERIMENTAL
- Ogury SDK (iOS) 1.0.4
- Ogury Adapter (iOS) 03.02.2020
- Ogury SDK (Android) 4.3.7
- Ogury Adapter (Android) 4.1.2
- Mintegral SDK (iOS) 5.8.7
- Mintegral Adapter (iOS) 9.8.0
- Mintegral SDK (Android) 5.8.7
- Mintegral Adapter (Android) 2.2.4
- Yandex SDK (iOS) 2.14
- Yandex Adapter (iOS) 0.4.0
- Yandex SDK (Android) 2.112
- Yandex Adapter (Android) 0.2.0

### Updated
- AppLovin Adapter v4.4.0
- IronSource Adapter v1.5.0
- Tapjoy Adapter v2.4.0
- Unity Ads Adapter v2.4.0
- Facebook Adapter v2.6.1

### Changed
- Removed README
- Plugin Versions in package.json
- Plist fixer script now remove unsupported entries
- Using new rewarded ads downloading system.
- New AdMob rewarded ads are not supporting OnRewardedClicked event.
- Use HLog instead of Debug.Log


## [1.9.0] - 2020-01-20
### Added
- Added package manifest and changelog.

### Changed
- Changed structure of package.

### Updated
- Chartboost v2.0.1
- proguard template files with new adapters


## [1.8.88] - 2019-12-30
### Added
- Adaptive Banner Size


## [1.8.87] - 2019-12-30
### Updated
- Google Mobile Ads Unity Plugin  v4.2.0

### Changed
- All events on main unity thread


## [1.8.1] - 2019-12-30
### Updated
GoogleMobileAdsMediationTestSuite v1.2.2


## [1.8.0] - 2019-12-30
### Updated
- Google Mobile Ads Unity Plugin v4.1.0
- Vungle Adapter v3.2.0
- AppLovin Adapter v4.3.0
- Facebook Adapter v2.6.0
- Ironsource Adapter v1.3.2
- AdColony Adapter v2.0.0
- Chartboost Adapter v2.0.0
- InMobi Adapter v2.4.0
- AdsTapjoy Adapter v2.3.1
- Unity Ads Adapter v2.3.0
- MoPub Adapter v2.9.0
- Mediation Test Suite v1.2.0


## [1.7.0] - 2019-02-04
### Updated
- Google Mobile Ads Unity Plugin v3.17.0
- Mediation Test Suite v1.0.0


## [1.7.0] - 2019-02-04
### Updated
- Google Mobile Ads Unity Plugin v3.15.0
- Vungle Adapter v3.2.0
- AppLovin Adapter v4.3.0
- Facebook Adapter v2.6.0
- Ironsource Adapter v1.3.2
- AdColony Adapter v2.0.0
- Chartboost Adapter v2.4.0
- InMobi Adapter v2.4.0
- Tapjoy Adapter v2.3.1
- UnityAds Adapter v2.3.0
- Mediation Test Suite v0.9.2