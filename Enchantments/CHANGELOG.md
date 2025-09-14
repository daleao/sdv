# ENCHANTMENTS Changelog

## 2.2.2

### Changed

* Changed the damage of echo projectiles. The first echo now deals 70% damage (10% increase). The second echo now deals 49% damage (11% decrease).
* Greedy enchantment HP threshold scaling increased from 1% to 2.5% per kill.

<sup><sup>[🔼 Back to top](#enchantments-changelog)</sup></sup>

## 2.2.1

Rebuilt with updated [Core](../Core) version.

### Fixed

* Fixed Vampiric Ring removing the overheal from Sanguine Enchantment.

<sup><sup>[🔼 Back to top](#enchantments-changelog)</sup></sup>

## 2.2.0

### Added

* Added Sharp enchantment to Iridium Scythe and Pickaxe.

### Fixed

* Fixed Master enchantment not working as advertised.
* Fixed some enchantment names in the documentation.

### Removed

* Master enchantment can no longer be applied to Scythe.

<sup><sup>[🔼 Back to top](#enchantments-changelog)</sup></sup>

## 2.1.5

### Fixed

* Fixed an error thrown when [Harmonics](../Harmonics) is not installed.

<sup><sup>[🔼 Back to top](#enchantments-changelog)</sup></sup>

## 2.1.4

### Fixed

* Shrunk Slimes no longer display misplaced eyes and antenna.
* Fixed Garnet gemstone forge slot not displaying correctly in weapon tooltips.

<sup><sup>[🔼 Back to top](#enchantments-changelog)</sup></sup>

## 2.1.3

### Fixed

* Players can no longer open the Animal Query Menu on farm animals transfigured by Wabbajack enchantment. This prevents an exploit where these animals could be sold repeatedly for infinite gold. 

<sup><sup>[🔼 Back to top](#enchantments-changelog)</sup></sup>

## 2.1.2

### Changed

* Changed the implementation of the Reaching enchantment for Scythe to avoid hitting tile behind the player. It now neatly extends the range by exactly 1 tile in the player's facing direction. Also optimizes the vanilla hitbox calculation without changing the area.

<sup><sup>[🔼 Back to top](#enchantments-changelog)</sup></sup>

## 2.1.1

### Added

* Added extended Tool enchantments:
  * Master enchantment can now be applied to all major tools, and effect was buffed from a flat +1 to a proportional +20% to the corresponding skill level.
  * Swift enchantment can now be applied to Watering Can.
  * Reaching enchantment can now be applied to Scythe.

### Fixed

* Fixed Haymaker Scythe application.

<sup><sup>[🔼 Back to top](#enchantments-changelog)</sup></sup>

## 2.1.0

### Added

* Haymaker is now exclusively a scythe enchantment and can only be applied to the Iridium Scythe.

### Changed

* Reduced the delay before firing echo projectiles. 
* Renamed some enchantments, again.
    * Sunburst -> Radiant
    * Vampiric -> Sanguine
    * Volatile -> Explosive

### Fixed

* Fixed an error loop when firing a slingshot.
* Re-added missing tool enchantments.

### Removed

* Removed unused Steadfast enchantment.

<sup><sup>[🔼 Back to top](#enchantments-changelog)</sup></sup>

## 2.0.2

### Changed

* Renamed enchantments. Again.
* Piercing (renamed to Reckless) enchant now grants invincibility frames during the animation and allows the dash strike to be used twice in succession. If hovering over an enemies, the attack will automatically home in on the target's location.

<sup><sup>[🔼 Back to top](#enchantments-changelog)</sup></sup>

## 2.0.1

### Added

* Added to mod description the missing fact that this mod rebalances the Jade enchantment.

### Changed

* Renamed some enchantments.

<sup><sup>[🔼 Back to top](#enchantments-changelog)</sup></sup>

## 2.0.0

Updated for game version 1.6.14.

### Added

* Added non-optional Jade Enchantment rebalance. The vanilla version is completely useless at 10% crit power because it's the same amount given by Ruby Enchantment, but while the Ruby applies to every hit, the Jade only applies to critical hits.
    * Before: crit power +10%
    * After: crit power +50%

<sup><sup>[🔼 Back to top](#enchantments-changelog)</sup></sup>

## 1.1.0

### Added

* Added Sunburst enchantment.
* Added French translations by [CaranudLapin](https://github.com/CaranudLapin).
* Added FakeFarmer.

### Changed

* Wabbajack cheese conversion now creates individual cheese items instead of a single stack of cheese. I think it's funnnier this way.
* Tweaked some of the odds of different Wabbajack effects.

### Fixed

* Prevent showing gemstone forges in Scythe tooltips.
* Fixed GMCM menu not loading.

<sup><sup>[🔼 Back to top](#enchantments-changelog)</sup></sup>

## 1.0.0 - Initial 1.6 release

### Added

* Stabbing Sword is now a sword-exclusive enchantment.


[🔼 Back to top](#enchantments-changelog)
