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
