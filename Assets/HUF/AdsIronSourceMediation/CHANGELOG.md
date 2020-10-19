## Internal SDK
- IronSource Unity Mediation v7.0.1
- IronSource SDK (Android) v7.0.1
- IronSource SDK (iOS) v7.0.1

- AdColony Unity Adapter v4.3.4.1
- AdColony Native Adapter (Android) v4.3.2
- AdColony SDK (Android) v4.2.2
- AdColony Native Adapter (iOS) v4.3.3.1
- AdColony SDK (iOS) v4.3.1

- AdMob Unity Adapter v4.3.18
- AdMob Native Adapter (Android) v4.3.13
- AdMob SDK (Android) v19.3.0
- AdMob Native Adapter (iOS) v4.3.15.1
- AdMob SDK (iOS) v7.64.0

- Amazon Unity Adapter v4.3.5.6
- Amazon Native Adapter (Android) v4.3.2
- Amazon SDK (Android) v5.9.0
- Amazon Native Adapter (iOS) v4.3.4.6
- Amazon SDK (iOS) v3.3.0

- AppLovin Unity Adapter v4.3.19.1
- AppLovin Native Adapter (Android) v 4.3.16
- AppLovin SDK (Android) v9.13.4
- AppLovin Native Adapter (iOS) v4.3.16.1
- AppLovin SDK (iOS) v6.13.4

- Chartboost Unity Adapter v4.3.6.1
- Chartboost Native Adapter (Android) v4.3.4
- Chartboost SDK (Android) v8.1.0
- Chartboost Native Adapter (iOS) v4.3.3.1
- Chartboost SDK (iOS) v8.2.1

- Facebook Unity Adapter v4.3.28
- Facebook Native Adapter (Android) v4.3.20
- Facebook SDK (Android) v6.0.0
- Facebook Native Adapter (iOS) v4.3.18.5
- Facebook SDK (iOS) v5.10.1

<!--- For now removed - not working in Unity 2019.4
- Fyber Unity Adapter v4.3.7.1
- Fyber Native Adapter (Android) v4.3.5
- Fyber SDK (Android) v7.5.3
- Fyber Native Adapter (iOS) v4.3.5.1
- Fyber SDK (iOS) v7.5.4
-->

- HyperMX Ads Unity Adapter v4.1.10.5
- HyperMX Ads Native Adapter (Android) v4.1.6
- HyperMX Ads SDK (Android) v5.1.0
- HyperMX Ads Native Adapter (iOS) v4.1.6.5
- HyperMX Ads SDK (iOS) v5.3.0

- InMobi Ads Unity Adapter v4.3.9.1
- InMobi Ads Native Adapter (Android) v4.3.6
- InMobi Ads SDK (Android) v9.0.8
- InMobi Ads Native Adapter (iOS) v4.3.7.6
- InMobi Ads SDK (iOS) v9.0.7

- Tapjoy Ads Unity Adapter v4.1.17.1
- Tapjoy Ads Native Adapter (Android) v4.1.13
- Tapjoy Ads SDK (Android) v12.6.1
- Tapjoy Ads Native Adapter (iOS) v4.1.13.1
- Tapjoy Ads SDK (iOS) v12.6.1

- Unity Ads Unity Adapter v4.3.7.2
- Unity Ads Native Adapter (Android) v4.3.6
- Unity Ads SDK (Android) v3.4.8
- Unity Ads Native Adapter (iOS) v4.3.4.2
- Unity Ads SDK (iOS) v3.4.8

- Vungle Ads Unity Adapter v4.3.6
- Vungle Ads Native Adapter (Android) v4.3.3
- Vungle Ads SDK (Android) v6.7.1
- Vungle Ads Native Adapter (iOS) v4.3.5.0
- Vungle Ads SDK (iOS) v6.7.1


## [2.5.0] - 2020-09-18
### Changed
- Handling change in banner show event


## [2.4.5] - 2020-09-17
### Added
- Hufdefine file 


## [2.4.4] - 2020-09-08
### Updated
- IronSource Unity Plugin v7.0.1
- Fixed building error on iOS caused by mismatching adapters


## [2.4.3] - 2020-09-02
### Changed
- More pronounced error logging if HBI is not initialized before IS
- Set UserID before Init


## [2.4.2] - 2020-08-31
### Updated
- IronSource Unity Plugin v7.0.0


## [2.4.1] - 2020-08-26
### Added
- Documentation

### Changed
- Minor refactoring.


## [2.4.0] - 2020-08-03
### Updated
- IronSource Unity Plugin v6.18.0.1
- Adapter AdColony v4.3.3.3
- Adapter AdMob v4.3.15.3
- Adapter Amazon v4.3.5.3
- Adapter AppLovin v4.3.18.2
- Adapter Chartboost v4.3.5.2
- Adapter Facebook v4.3.26.2
- Adapter HyperMX v4.1.10.2
- Adapter InMobi v4.3.8.3
- Adapter Tapjoy v4.1.16.2
- Adapter Unity v4.3.6.3
- Adapter Vungle v4.3.3.3

### Required when conflict with AppsFlyer
Add this snippet to an `application` section of the `Assets/Plugins/Android/AndroidManifest.xml` file.

```
tools:replace="android:fullBackupContent" android:fullBackupContent="@xml/appsflyer_backup_rules"
```

If there is not `tools` in  section `manifest xmlns` add:

```
xmlns:tools="http://schemas.android.com/tools"
```

## [2.3.1] - 2020-06-18
### Removed
- Vungle, Mintegral - red-flagged by Google.

## [2.3.0] - 2020-06-18
### Added
- Adapter HyperMX v4.1.7.1 - require exclude 'META-INF/proguard/coroutines.pro'


## [2.2.0] - 2020-06-10
### Updated
- IronSource Unity Plugin v6.16.2
- Adapter AdColony v4.3.1.0
- Adapter AdMob v4.3.12.1
- Adapter AppLovin v4.3.14
- Adapter Chartboost v4.3.2.1
- Adapter Facebook v4.3.20.1
- Adapter InMobi v4.3.5.1
- Adapter Tapjoy v4.1.13.2
- Adapter Unity v4.3.3.1
- Adapter Vungle v4.3.0.1

### Added
- Adapter Amazon v4.3.3.2
- Adapter Fyber v4.3.5.1
- Adapter Mintegral v4.3.2.1

## [2.1.0] - 2020-05-28
### Updated
- IronSource Unity Plugin v6.16.1
- Adapter AdMob v4.3.12
- Adapter AppLovin v4.3.13
- Adapter Facebook v4.3.20
- Adapter Unity v4.3.3

### Changed
- Changed user id to set HBI user id


## [2.0.5] - 2020-05-12
### Updated
- Adapter AppLovin v4.3.12
- Adapter Chartboost v4.3.2
- Adapter InMobi v4.3.5

##Fixed
- Ad Close event after ad screen close, not with rewarded on iOS 

### Changed
- Code changes to match coding guidelines


## [2.0.4] - 2020-05-04
### Updated
- IronSource Unity Plugin v6.16.1
- Adapter AdColony v4.3.0.1
- Adapter AdMob v4.3.11.1
- Adapter AppLovin v4.3.11.1
- Adapter Chartboost v4.3.1.1
- Adapter Facebook v4.3.18.1
- Adapter InMobi v4.3.3.2
- Adapter Tapjoy v4.1.13.1
- Adapter Unity Ads v4.3.2
- Adapter Vungle v4.3.0


## [2.0.3] - 2020-04-21
### Updated
- IronSource Unity Plugin v6.16.0
- Adapter Facebook v4.3.18
- Adapter Unity v4.3.1

### Added
- Adapter Tapjoy v4.1.12
- manifest command to remove permission - READ_PHONE_STATE 


## [2.0.2] - 2020-04-17
### Fixed
- Detecting configs when building from command line


## [2.0.1] - 2020-04-16
### Added
- Adapter AdColony v4.3.0
- Adapter Chartboost v4.3.1
- Adapter InMobi v4.3.3
- Adapter Tapjoy v4.1.12

### Updated
- IronSource Unity Plugin v6.16.0
- Adapter Facebook v4.3.17
- Adapter Vungle v4.1.12


## [2.0.0] - 2020-04-03
### Added
- HIP support

### Changed
- Self Containing Package
- Namespaces
- removing WRITE_EXTERNAL_STORAGE and READ_EXTERNAL_STORAGE from Android

### Updated
- IronSource Unity Plugin v6.15.0.1
- Adapter AdMob v4.3.11
- Adapter AppLovin v4.3.11
- Adapter Facebook v4.3.16
- Adapter Unity Ads v4.3.0


## [1.9.1] - 2020-02-11
### Updated
- IronSource Unity Plugin v6.14.0.0
- Adapter AppLovin v4.3.9
- Adapter Facebook v4.3.13
- Adapter Vungle v4.1.11


## [1.9.0] - 2020-01-21
### Added
- Added package manifest and changelog.

### Changed
- Changed structure of package.

### Updated
- IronSource Unity Plugin v6.13.0.1
- Adapter AdMob v4.3.9
- Adapter AppLovin v4.3.8
- Adapter Facebook v4.3.12
- Adapter Unity Ads v4.1.9
- Adapter Vungle v4.1.0


## [1.7.0] - 2019-10-30
### Added
- IronSource plugin ver. 6.8.3.1
- Adapter AdMob mediation ver. ios.7.42.2-android.17.2.0
- Adapter AppLovin mediation ver. ios.6.3.0-android.9.2.1
- Adapter Facebook Audience mediation ver. ios.5.1.1-android5.1.1
- Adapter Unity Ads mediation ver. ios.3.0.0-android.3.0.0
- Adapter Vungle mediation ver. ios.6.3.2-android.6.3.24