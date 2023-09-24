# TOLS Changelog

## 3.0.1

### Added

* Re-added the tool-specific enchant command.

### Fixed

* Fixed Master enchantment on tools other than Fishing Rod still increasing Fishing level by 1, and also not showing up as green in the skills page. Idk why ConcernedApe didn't just make it an `addedFishingLevel` to begin with.

<sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

## 3.0.0

### Fixed

* Fixed a vanilla bug that occurs when attempting to harvest fiber that is not ready for harvest, with a Haymaker scythe, and caused an infinite fiber exploit.

<sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

## 2.5.5

### Fixed

* Added missing GMCM options for Watering Can.

<sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

## 2.5.4

### Fixed

* Fixed a possible Null-Reference exception when using Flexible Sprinklers.

<sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

## 2.5.3

### Fixed

* Fixed an issue with Hoe and Watering Can settings validation.

<sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

## 2.5.0

### Added

* Added Radioactive tool upgrades and Volcano Forge Upgrading.
    * Once the Volcano Forge is unlocked, you can use it to complete your tool upgrades. You will need the same amount of metal bars, and a handful of Cinder Shards to ignite the Forge. In exchange, there is no labor cost, and most importantly, the result is instantaneous (no waiting around for 2 days).
    * This is the only way to obtain the Radioactive upgrade level, since Clint is not a fool to mess around with radioactive substances.
    * If Moon Misadventures is intalled, the Mythicite upgrade level also can only be obtained by this method.
    * Includes Radioactive and Mythicite textures compatible with [Grandpa's Tools](https://www.nexusmods.com/stardewvalley/mods/8835).
* Added option to reward experience for using the Watering Can.
* Added option to prevent refilling the Watering Can with salt water (enabled by default).

### Removed

* Removed OverrideAffectedTiles config. Affected tiles now are always overriden.

### Fixed

* ColorCodedForYourConvenience now works without WPNZ module.

<sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

## 2.4.0

### Fixed

* Fixed broken translations in GMCM page links.

<sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

## 2.2.3

### Fixed

* Revised enable condition for ButtonPressedEvent, which should fix issues with FaceMouseCursor, SlickMoves and AutoSelection working if any is disabled.

<sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

## 2.0.0

### Changed

* The stamina cost of the charged Axe and Pickaxe can now be configured separately.

### Fixed

* Added missing base stamina cost mulitpliers for each tool to GMCM.

<sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

## 1.3.3

### Fixed

* Fixed object harvesting not limited to forage (woops).
* Fixed out of bounds experience gain.

### Remved

* Removed `HarvestSpringOnions` option. This is now considered forage.

<sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

## 1.3.2

### Added

* Added the ability to harvest forage with scythe.

### Changed

* Harvest with scythe functionality will no-longer apply while Yet Another Harvest With Scythe mod is installed.

<sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

## 1.3.1

### Added

* By request, added the option to limit crop harvesting to Golden Scythe.

### Fixed

* Fixed scythe tooltip patcher not applying due to bad namespace.

<sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

## 1.3.0

## Added

* Added crop harvesting with Scythe.

<sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

## 1.2.3

## Added

* Added auto-select compatibility for Dr. Birb's Upgradeable Ranching Tools and Upgradeable Pan.
* Added the ability to customize the auto-selection border color.

## Changed

* Auto-selectable cache now uses Dictionary instead of Hash Set for much better performance.

## Fixed

* Fixed a possible memory leak in tool auto-selection logic.

<sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

## 1.2.0

### Added

* Added tool auto-selection.

<sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

## 1.0.4

### Fixed

* The AllowMasterEnchantment config should now work correctly.

<sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

## 1.0.1

### Changed

* Affected tile settings for Hoe and Watering Can now use named tuple array instead of jagged array. This is more efficient and more legible.

### Fixed

* Added a failsafe for an Index Out Of Range exception that may occur with Moon Misadventures installed.

<sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

## 0.9.9

### Fixed

* No longer changes the stats of scythes (which means they no longer need to be revalidated).

<sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

## 0.9.7

### Fixed

* Fixed a bug causing player Stamina to get stuck at 1 and not continue below 0.

<sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

## 0.9.4

### Fixed

* Fixed a bug preventing weapons from destroying bushes and other location objects.
* Fixed a bug with Scythe ClearTreeSaplings setting.
* Scythe can now receive the Haymaker enchantment as intended.

<sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

## 0.9.3

### Fixed

* Control settings now apply only to weapons, as they should.

<sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

## 0.9.0 (Initial release)

### Added

* Added Scythe settings.
* Added stamina multiplier setting to each tool.
* Added Face Mouse Cursor setting to match Arsenal.

[🔼 Back to top](#tols-changelog)