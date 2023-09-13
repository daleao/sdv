# TOLS Changelog

## 3.0.1 <sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

### Added

* Re-added the tool-specific enchant command.

### Fixed

* Fixed Master enchantment on tools other than Fishing Rod still increasing Fishing level by 1, and also not showing up as green in the skills page. Idk why ConcernedApe didn't just make it an `addedFishingLevel` to begin with.

## 3.0.0 <sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

### Fixed

* Fixed a vanilla bug that occurs when attempting to harvest fiber that is not ready for harvest, with a Haymaker scythe, and caused an infinite fiber exploit.

## 2.5.5 <sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

### Fixed

* Added missing GMCM options for Watering Can.

## 2.5.4 <sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

### Fixed

* Fixed a possible Null-Reference exception when using Flexible Sprinklers.

## 2.5.3 <sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

### Fixed

* Fixed an issue with Hoe and Watering Can settings validation.

## 2.5.0 <sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

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

## 2.4.0 <sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

### Fixed

* Fixed broken translations in GMCM page links.

## 2.2.3 <sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

### Fixed

* Revised enable condition for ButtonPressedEvent, which should fix issues with FaceMouseCursor, SlickMoves and AutoSelection working if any is disabled.

## 2.0.0 <sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

### Changed

* The stamina cost of the charged Axe and Pickaxe can now be configured separately.

### Fixed

* Added missing base stamina cost mulitpliers for each tool to GMCM.

## 1.3.3 <sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

### Fixed

* Fixed object harvesting not limited to forage (woops).
* Fixed out of bounds experience gain.

### Remved

* Removed `HarvestSpringOnions` option. This is now considered forage.

## 1.3.2 <sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

### Added

* Added the ability to harvest forage with scythe.

### Changed

* Harvest with scythe functionality will no-longer apply while Yet Another Harvest With Scythe mod is installed.

## 1.3.1 <sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

### Added

* By request, added the option to limit crop harvesting to Golden Scythe.

### Fixed

* Fixed scythe tooltip patcher not applying due to bad namespace.

## 1.3.0 <sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

## Added

* Added crop harvesting with Scythe.

## 1.2.3 <sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

## Added

* Added auto-select compatibility for Dr. Birb's Upgradeable Ranching Tools and Upgradeable Pan.
* Added the ability to customize the auto-selection border color.

## Changed

* Auto-selectable cache now uses Dictionary instead of Hash Set for much better performance.

## Fixed

* Fixed a possible memory leak in tool auto-selection logic.

## 1.2.0 <sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

### Added

* Added tool auto-selection.

## 1.0.4 <sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

### Fixed

* The AllowMasterEnchantment config should now work correctly.

## 1.0.1 <sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

### Changed

* Affected tile settings for Hoe and Watering Can now use named tuple array instead of jagged array. This is more efficient and more legible.

### Fixed

* Added a failsafe for an Index Out Of Range exception that may occur with Moon Misadventures installed.

## 0.9.9 <sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

### Fixed

* No longer changes the stats of scythes (which means they no longer need to be revalidated).

## 0.9.7 <sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

### Fixed

* Fixed a bug causing player Stamina to get stuck at 1 and not continue below 0.

## 0.9.4 <sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

### Fixed

* Fixed a bug preventing weapons from destroying bushes and other location objects.
* Fixed a bug with Scythe ClearTreeSaplings setting.
* Scythe can now receive the Haymaker enchantment as intended.

## 0.9.3 <sup><sup>[🔼 Back to top](#tols-changelog)</sup></sup>

### Fixed

* Control settings now apply only to weapons, as they should.

## 0.9.0 (Initial release)

### Added

* Added Scythe settings.
* Added stamina multiplier setting to each tool.
* Added Face Mouse Cursor setting to match Arsenal.

[🔼 Back to top](#tols-changelog)