# Immersive Professions Change Log

## 5.2.0 (Unreleased)

### Changed

* **Rascal** - ~~Slingshot damage +25%. 60% chance to recover spent ammo.~~ Gain one additional ammo slot. 35% chance to recover spent ammo.
    * The damage perk is gone. I always felt like Slingshot damage was overpowered anyway. In its place comes a second ammo slot; Rascals can now equip 2 different types of ammo (or the same ammo twice). The secondary ammo is fired with the Action button, usually reserved for Special Moves. Secondary ammo can equally be overcharged and will gain any effects from applied Slingshot Enchantments (see [Immersive Slingshots](../ImmersiveSlingshots)).
    * The ammo recovery perk has been nerfed to account for new enchantments [Immersive Slingshots](../ImmersiveSlingshots). In exchange, the Prestige perk now doubles this value to a whopping 70%---a higher value than the original.
* **Desperado (Prestiged)** - ~~Overcharged shots become spreadshots.~~ Overcharge twice as fast.
    * The Spreadshot perk is removed to make room for new enchantments [Immersive Slingshots](../ImmersiveSlingshots). In exchange, the Desperado can now overcharge twice as fast, which makes it much more useful when kiting enemies.
* The [API](../Common/Integrations/ImmersiveProfessions/IImmersiveProfessionsApi.cs) has been slightly changed. Some mods may need to update the corresponding interfaces.

### Fixed

* Fixed a bug where Conservationist Trash Collected would not reset if no trash was collected during the season.
* Fixed a bug where Tapper perk would incorrectly apply to tapped Mushroom Trees in Winter, causing multiple progressively shorted harvests.
* Fixed an issue with Automated Junimo Chests being unable to decide their owner.
* Fixed some translation errors.

## 1.0.0 (March 24, 2021)

* Initial Version