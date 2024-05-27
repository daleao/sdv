# PRFS Changelog

## 0.2.5

### Changed

* Updated Chinese translations.

### Fixed

* Fixed issue preventing location interactions introduced in 0.2.3.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 0.2.4

### Fixed

* Fixed possible Null-Ref exception in MineShaftCheckStoneForItemsPatcher.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 0.2.3 

### Fixed

* Fixed max-sized fish not being counted correctly.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 0.2.2

### Added

* Added Chinese, French and Korean translations. Credits added to [README](../Professions).

### Changed

* Additional slingshot ammo slot now draws horizontally in the slingshot's tooltip instead of vertically, matching the style of the Advanced Iridium Rod and saving some vertical space.
* "Memorized" fishing rod tackle now draw correctly in the rod's tooltip.

### Fixed

* Fixed various issues with custom skills.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 0.2.1

### Fixed

* Fixed possible crash when selecting Prestiged Harvester profession.
* Fixed possible Out-Of-Range exception in FarmerCurrentToolSetterPatcher.
* Fixed possible Null-Ref exception in MonsterFindPlayerPatcher.

### Changed

* Changed Sewer Statue logic to be more compatible with different configurations of Skill Reset / Prestige / Limit Break.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 0.2.0 - Beta release for 1.6

* No changes.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 0.1.1

### Added

* Added Mastery Limit Select menu when mastering the Combat skill.

### Fixed

* Fixed player not gaining experience.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 0.1.0 - Alpha release for 1.6

### Changed

* Prestige levels no longer require having all professions in the skill. It is now a reward for Mastering each individual skill. The Statue of Prestige has been renamed to Statue of Transcendence to avoid confusion. 
* Skill reset feature is unchanged. But note that choosing to Master a skill will prevent it from being reset further, effectively locking you out of any unobtained profession. *This may change in the future, but for now you have been warned.*
* Similarly, the Limit Break is now a reward for Mastering the Combat skill.
* All prestiged profession variants, and a few base variants, have been reworked. Prestige professions are now much more impactful end-game rewards. Please review the section [Professions](README.md#professions) of the README to learn more.
* Treasure Hunts are now triggered on Time Change instead of Player Warped. The chance to start a Treasure Hunt now depends on how far you have traveled or how many rocks you have broken since the previous hunt.
* Misc. code improvements.

### Removed

* Removed custom support for Luck skill and Love of Cooking.
* Removed alost all third-party mod integrations since I don't know which of them were/will be updated. I might re-add Automate integration later, or I might not.


[🔼 Back to top](#profs-changelog)

[View the 1.5 Changelog](resources/CHANGELOG_old.md)