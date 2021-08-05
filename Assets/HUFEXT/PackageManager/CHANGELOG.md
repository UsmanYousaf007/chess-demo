## [1.10.0] - 2021-08-03
### Added
- Showing development channel

### Updated
- Documentation


## [1.9.0] - 2021-07-21
### Added
- Showing popup with new package updates


## [1.8.0] - 2021-07-21
### Added
- Downloading changelog for selected package version (won't work on older package versions)


## [1.7.3] - 2021-07-01
### Fixed
- Updating not working


## [1.7.2] - 2021-06-28
### Added
- Displaying minimum package version

### Fixed
- Updating dependencies when it is not needed
- Removed "Manifest wasn't found in the remote package" error in non-HUF packages


## [1.7.1] - 2021-06-14
### Fixed
- Not checking in optional dependencies if other packages need to be updated


## [1.7.0] - 2021-05-06
### Added
- Button to remove GPR Scoped dependencies

### Changed
- Stopped for a time checking if packages are in conflict


## [1.6.8] - 2021-04-12
### Fixed
- Install Unity Dependencies Buttons are shown properly
- Automatically adding scoped registries when installing packages


## [1.6.7] - 2021-04-09
### Fixed
- Issues with Update Packages window


## [1.6.6] - 2021-03-17
### Added
- Other versions menu in packages from git repository


## [1.6.5] - 2021-03-02
### Fixed
- Compile errors in Unity 2018


## [1.6.4] - 2021-03-01
### Added
- Using at least minimum version of dependencies when updating multiple packages

### Fixed
- Missing versions in some packages
- Install Unity button for HUF developers
- Authors in Unity packages


## [1.6.3] - 2021-02-25
### Fixed
- Internet connection issue causing log outs from HUF Package Manager


## [1.6.2] - 2021-02-24
### Fixed
- Showing which version is currently selected in "Other versions" menu


## [1.6.1] - 2021-02-23
### Fixed
- Infinite loop when downloading PolicyGuard package


## [1.6.0] - 2021-02-18
### Added
- Internal SDKs to generated reports


## [1.5.4] - 2021-01-12
### Added
- Showing manager version refreshed when it is rebuilt


## [1.5.3] - 2020-12-23
### Added
- Installing HUF Package Manager update before other packages
- Hiding Documentation button for remote packages

### Fixed
- Warning in ProcessPackageLockCommand


## [1.5.2] - 2020-12-22
### Fixed
- Showing if the package is supported by the current Unity


## [1.5.1] - 2020-12-08
### Added
- Installing Unity dependencies in repositories


## [1.5.0] - 2020-11-16
### Added
- Showing optional dependencies and if updating is required based on them
- Better support of Unity Packages
- Packages supporting certain Unity versions
- Searching for dependencies in all channels and scopes


## [1.4.1] - 2020-11-12
### Fixed
- Not showing newest packages in the newest rollout
- Missing documentation message


## [1.4.0] - 2020-10-23
### Added
- Refreshing packages in the background after opening the window
- Asking if the repository should be replaced with a newer package
- Better search function
- Rebuilding defines after a package was installed or removed

### Fixed
- "Documentation" button being disabled
- "Other versions" button workflow
- Installing wrong versions of the dependencies
- "Update Packages" bug fixes
- Not installing needed dependencies when updating a package


## [1.3.2] - 2020-10-19
### Added
- Hufdefine file

### Fixed
- Documentation errors


## [1.3.1] - 2020-07-01
### Fixed
- Install path


## [1.3.0] - 2020-06-08
### Added 
- Unity packages displayed in HPM.
- Support for external scoped registries.
- Stability and performance improvements.

### Fixed
- Packages doesn't refresh after install/remove.
- Fixed incorrect flow of packages.


## [1.2.1] - 2020-05-05
### Changed
- Changed HBI endpoint.


## [1.2.0] - 2020-04-24
### Added
- Detection for packages with git repository.
- Added support for scoped registries.

### Fixed
- Issues with search bar.


## [1.1.0] - 2020-03-31
### Added
- Added menu item to change package scope between stable and preview.
- Added ability to change package version to any other.
- Added package dependency resolving.

### Changed
- UI & UX tweaks.
- Stability and performance improvements.


## [1.0.1] - 2020-02-06
### Fixed
- Fixed issue with socket exception on MacOS.
- Fixed issue with incorrect policy window layout.


## [1.0.0] - 2020-01-29
### Added
- Added package manifest, changelog and license files.
- Added new graphics and layout.
- Added HUF license validation.
- Added build analytics event with information's about packages.

### Changed
- Major refactoring. Changed structure of package.