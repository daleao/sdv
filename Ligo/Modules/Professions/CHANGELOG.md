# Ligo Professions Change Log

## 1.0.0 (Unreleased)

### Added

* When a Gemologist reaches a new quality threshold, all currently processing Crystalariums owned by that player will likewise receive a quality upgrade to reflect that.
* Added golden versions of profession icons for Prestiged professions.

### Removed

* Removed the SeaweedIsTrash config setting.
* Removed configs from the [IProfessions interface](../../../Shared/Integrations/Ligo/ILigo.cs).

### Changed

* **Rascal** - ~~Slingshot damage +25%. 60% chance to recover spent ammo.~~ Gain one additional ammo slot. 35% chance to recover spent ammo.
    * The damage perk is gone. I always felt like Slingshot damage was overpowered anyway. In its place comes a second ammo slot; Rascals can now equip 2 different types of ammo (or the same ammo twice). The secondary ammo is fired with the Action button, usually reserved for Special Moves. Secondary ammo can equally be overcharged and will gain any effects from applied Slingshot Enchantments (see the [Slingshots](../Arsenal/Slingshots) module).
    * The ammo recovery perk has been nerfed to account for new enchantments provided by the [Slingshots](../Arsenal/Slingshots) module. In exchange, the Prestige perk now doubles this value to a whopping 70%---a higher value than the original.
    * Ability to equip Slime ammo moved from Slimed Piper to Rascal. It will still cause a slow debuff, but will not heal ally Slimes unless the player *also* has the Piper profession.
* **Slimed Piper** - Summoned Slimes are now friendly (will not attack players, but will still cause damage if touched).
* **Desperado (Prestiged)** - ~~Overcharged shots become spreadshots.~~ Overcharged shots can pierce enemies.
    * The Spreadshot perk is removed to make room for new enchantments provided by the [Slingshots](../Arsenal/Slingshots) module. The ability to pierce enemies with overcharged shots is now the new prestige perk.
* The [API](../../../Shared/Integrations/Ligo/ILigoApi.cs) has been slightly changed. Some mods may need to update the corresponding interfaces.

### Fixed

* Fixed (hopefully) a bug with Automate pushing inputs from Junimo Chests.
* Fixed a bug where Conservationist Trash Collected would not reset if no trash was collected during the season.
* Fixed a bug where Tapper perk would incorrectly apply to tapped Mushroom Trees in Winter, causing multiple progressively shorted harvests.
* Fixed an issue with Automated Junimo Chests being unable to decide their owner.
* Crab Pots now correctly trap Seaweed instead of Green Algae in the Beach farm map.
* Fixed Special Abiltiy charge still accumulating with Enable Specials config set to false.
* Fixed a possible NullReferenceException when shooting at Slimes.
* Added a setter to the CustomArtisanMachines config, preventing it from being reset on game load.
* The print_fishdex console command now takes into account the value of AnglerMultiplierCap config.
* Fixed some translation errors.