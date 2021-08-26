## [3.0.0] - 2021-07-23
### Updated
- Google Play Billing Library to v3 (Remember to contact SIM before updating to this package version. Delete Assets\Plugins\UnityPurchasing folder after updating)

### Fixed
- UpdateSubscription in editor purchases
- Nulls in OnPurchaseSuccess response in editor purchases
- OnSubscriptionExpired not being raised


## [2.9.9] - 2021-06-29
### Fixed
- OnPurchaseInit is now called in test editor purchases
- In case of unavailable product, failure is no longer called twice


## [2.9.8] - 2021-06-14
### Fixed
- Instantaneous initialization in test editor purchases


## [2.9.7] - 2021-06-04
### Fixed
- Instantaneous OnPurchaseSuccess callback in test editor purchases
- Changed default price to 1 in test editor purchases


## [2.9.6] - 2021-05-13
### Fixed
- Purchasing consumables getting stuck when Apple sandbox have problems


## [2.9.5] - 2021-04-14
### Fixed
- Restoring non-consumables only restored some of them


## [2.9.4] - 2021-03-25
### Fixed
- Potential division by zero error


## [2.9.3] - 2021-03-23
### Fixed
- Null references of subscriptionSpecificInfo in ProductInfo class


## [2.9.2] - 2021-03-08
### Changed
- Editor test purchase window now offer a Success, Fail and Cancel buttons with proper events send.
- Removed Google Play billing library support - it is implemented in Unity IAP package v2.2.7


## [2.9.1] - 2021-03-04
### Fixed
- A warning


## [2.9.0] - 2021-02-24
### Added
- Added Google Play billing library support


## [2.8.2] - 2021-02-17
### Fixed
- Compilation error in dummy version


## [2.8.1] - 2021-02-11
### Added
- Price in cents parameter added to ProductInfo constructor


## [2.8.0] - 2021-01-22
### Added
- Option to just get the conversion rate with a callback

### Updated
- Documentation

### Changed
- UnityActions to Actions
- GetLocalizedPrice returns rounded values for display

### Removed
- Warnings


## [2.7.2] - 2020-12-21
### Fixed
- New IAP interrupted flow handled.


## [2.7.1] - 2020-12-14
### Added
- New Error type for HuuugeIAPServerError


## [2.7.0] - 2020-10-16
### Fixed
- wrong duration of subscription in debug builds

### Added
- Documentation
- Update subscription API

### Removed
- enableSubscriptionsService field from config


## [2.6.1] - 2020-09-18
### Changed
- Code refactoring


## [2.6.0] - 2020-09-17
### Added
- TryGetStoreProductInfo method in HPurchases
- Additional logs for purchase fail


## [2.5.6] - 2020-08-06
### Added
- Blocking input on purchase debug screen in Editor


## [2.5.5] - 2020-08-05
### Fixed
- Fixed not removed callback in PurchasesModel causing instant PurchaseSuccess.


## [2.5.4] - 2020-07-29
### Added
- Raw receipt in PurchaseReceiptData


## [2.5.3] - 2020-07-21
### Fixed
- Fixed null during subscription validation.

### Added
- Added hufdefine file.

### Changed
- move part of validation from PurchasesService to HuuugeIAPServerValidator


## [2.5.2] - 2020-07-06
### Fixed
- null check for recipe.


## [2.5.1] - 2020-06-29
### Fixed
- fixed wrong if statement for subscription validation.


## [2.5.0] - 2020-06-10
### Added
- Editor debug menu
- Editor debug options in config


## [2.4.0] - 2020-05-21
### Added
- Subscriptions support for Huuuge Server IAP

### Changed
- use more HLogs instead of default log


## [2.3.0] - 2020-05-11
### Added
- Alternative method for initialization with custom product list.

### Changed
- Store ID and Transaction ID added to receipt data.


## [2.2.0] - 2020-05-06
### Added
- Get currency code function


## [2.1.0] - 2020-04-06
### Added
- Support for optional Huuuge Server IAP verification


## [2.0.0] - 2020-03-23
### Removed
- Unity IAP package was removed from package.

### Fixed
- Fixed receipt data parsing on android and ios.


## [1.9.1] - 2020-01-31
### Fixed
- Casting problem when using external server for price conversion


## [1.9.0] - 2020-01-09
### Added
- Added package manifest and changelog.

### Changed
- Changed structure of package.

### Updated
- Unity IAP to 1.23.1
