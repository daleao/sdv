# SLNGS Changelog

## 2.5.6 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Fixed

* Fixed a small tooltip overflow issue with the Basic Slingshot.
* Fixed EnableSpecialMove config not doing anything.
* Removed some leftover debugging Alerts.

## 2.5.5 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Fixed

* Explosive ammo now slays Mummies as expected.

## 2.5.3 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Fixed

* Fixed another possible Null-Reference, this time in Slingshot tooltip draw.

## 2.5.1 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Fixed

* Fixed possible Null-Reference in Slingshot stat creation.

## 2.5.0 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Added

* Topaz Enchantment now actual does something if CMBT module is disabled (previously required CMBT module to do anything).
* Added option to display the current primary ammo in the item's sprite (enabled by default).

### Changed

* Archery Integration:
    * Bows can no longer receive gemstone enchantments. Instead, the focus shifts to Arrow Enchantments, as intended by PeacefulEnd.
    * Effectively, Bow ammo slots are equivalent to gemstone sockets, which means they can receive up to 2 gemstone effects (with the Rascal profession) based on the arrows that are equipped. The downside is that the player is required to craft enchanted arrows. The upside is that arrows can be swapped freely, unlike enchantments.
    * Arrow Enchantments will in turn resonate with Infinity Bands ([CMBT](../Rings)), as would other gemstone enchantments. Resonance bonuses apply even while the resonating arrow is in secondary ammo slot. If the resonating arrow is in the primary ammo slot, however, it's damage is also amplified by the resonance.
    * Diamond-tipped arrows give 2 random enchantment effects, but do not resonate.
    * If Rebalance option is enbled, arrow base damage also increases the bow's minimum damage, instead of only the maximum.
    * Rebalanced the Archery Starter Pack (new Misc. download). This adds the remaining gemstone arrow varities, making it required for all of the above. **I will not support any other content packs.**
* Changes to make regular slingshots more similar to Archery bows, and distinct from Melee Weapons:
    * Slingshots can now equip gemstones, including Diamonds and Prismatic Shard. Diamonds are particularly strong. Prismatic Shards are nukes.
    * Slingshots can no longer receive gemstone enchantments. As with bows, they now receive bonuses based on equipped gemstones. Clearly this is a huge nerf for Slingshots in the end-game, but also a huge buff in the early game, which I think is worth the trade.
    * Gemstone bonuses are now additive instead of multiplicative, which is in most cases an overall buff.
    * Again, resonances from Infinity Bands still apply as expected, and buff the gemstonee's raw damage when equipped as primary.
    * Diamonds give 2 random enchantment effects, but do not resonate.
    * Prismatic Shards give all enchantment effects simultaneously.

### Fixed

* ColorCodedForYourConvenience now works without WPNZ module. Also works for weapons from Archery Starter Pack.

## 2.4.0 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Added

* Added compatibility for [Archery](http://www.nexusmods.com/stardewvalley/mods/16767).
    * Bows can receive gemstone enchantments. The number of allowed gemstones must be configured manually using the SocketsPerBow setting.
    * Due to the nature of the Archery framework, Prismatic Shard enchantments are not allowed. This is not a technical limitation, but a design choice, given that bows can already be configured with customizable special attacks, and that arrows can receive custom enchantments. It simply would not make sense to add Prismatic Shard enchantments on top of that.
    * Bows are not affected by the following features:
        - They do not have ammo modifiers like slingshots do, but rather actual damage stats. So the EnableRebalance option does nothing. I may rebalance specific content packs, such as the Starter Pack. But obviously, expect this to receive the same treatment as the Weapon Rebalance from WPNZ.
        - Bows can naturally critical hit, so the EnableCriticalHits option, likewise, does nothing.
    * Anything else not listed here should be compatible, including Auto-Selection and Slick Moves.

### Fixed

* Fixed a typo that made fire speed bonuses from rings barely noticeable. They now work as advertised.
* Improved the special move animation (again). Hopefully the last time.
* Infinity Slingshot swipe effect is now hot pink, like the other Infinity weapons if WPNZ is enabled with Infinity+1 option.
* Can no longer stun enemies from afar by using the special move as a projectile is about to hit an enemy.
* Fixed Archery bows incorrectly triggering the Slingshot special move.

## 2.2.8 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Changed

* Removed 2 frames from Special Move windup, so it should feel slightly better.

### Fixed

* Fixed a bug allowing slingshots to receiving infinite forges.
* Improved Slick Moves stop condtion.
* Fixed FaceMouseCursor GMCM setting incorrectly mapped to the one from WPNZ module.

## 2.2.6 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Changed

* When FaceMouseCursor is enabled, pressing the Action button will no longer cause the player to accidentally use a special move in another direction when trying to interact with something.

## 2.2.4 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Added

* Moved Infinity Slingshot transformation over from WPNZ module, so the Infinity Slingshot can now be created even if WPNZ is disabled.

## 2.2.3 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Fixed

* Revised enable condition for ButtonPressedEvent, which should fix issues with FaceMouseCursor, SlickMoves and AutoSelection working if any is disabled.
* Fixed a bug preventing Galaxy Slingshot from receing Galaxy Souls.

## 2.2.0 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Added

* Added FaceMouseCursor option.
* Added Slingshot swipe animation for Artful enchantment.

### Changed

* Small improvements to the Slingshot smack special animation.
* Increased the hitbox of Slingshot's overhead smack special attack.

## 2.1.0 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Fixed

* Can no longer dash in festivals.

## 2.0.6 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Fixed

* Tool now read "Knockback" instead of "Weight", like melee weapons.
* Renamed internal stat virtual properties for better clarity.

## 2.0.5 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Fixed

* Fixed an issue preventing slingshots from receiving enchantments from ENCH module.

## 2.0.0 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

* Initial release of Slingshots module.

---

*Changes below this point refer to the legacy Arsenal module.*

## 1.3.5 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Fixed

* Fixed a bad Null-Reference Exception introduced in 1.3.4.

## 1.3.4 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Fixed

* Fixed missing scythe swipe sound.

## 1.3.2 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Fixed

* Enemies defeated by knockback damage now properly count for game stats, quests and drop items. 

## 1.3.1 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Fixed

* Fixed start-up error with Neptune's Glaive patcher.

## 1.3.0 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Changed

* Drop chance of Lava Katana has been doubled.
* Drop chance of Neptune's Glaive has been halved.
* Added apostrophe to "Neptune's Glaive", to make it sound more unique.

### Fixed

* Fixed a bug preventing explosive ammo from exploding without the Desperado profession.
* Fixed a bug with Obsidian Edge and Lava Katana drops creating error daggers instead.

## 1.2.3 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Added

* Added some emote bubbles to spice up Blade of the Ruined Hero and Clint's Forge events.
* Added the ability to customize the auto-selection border color.

## Changed

* Tribute enchantment threshold now inreases with each kill, and resets on taking damage.

### Fixed

* Blade of the Ruined Hero introduction event is no longer skippable.

## 1.2.1 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Fixed

* Fixed auto-selection preventing other LeftShift actions in inventory menu.
* Fixed inverted swipe effect when swinging scythe.

## 1.2.0 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Added

* Added arsenal auto-selection.
* You can now specify individual weapon with the console command for getting blueprints (`ars get bp <weapon1> <weapon2> etc...`).

### Fixed

* Fixed a bug in the console command for getting blueprints (`ars get bp`).

## 1.1.0 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Fixed

* Fixed out-of-bounds tooltip elements for slingshots higher than basic.
* Fixed bullseye position while aiming.

## 1.0.4 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Added

* Added SwipeHold config. You can now hold the tool button to perform a continuous combo instead of spam-clicking.
* Added CustomStabbingSwords config. Users can now register swords from their modlist to be treated as Stabbing swords. **Do not enter vanilla swords here, or non-sword weapons.**
* Lava Katana swing effect is now orange-ish.

### Fixed

* Fixed shipping bin logic, which was also messed up by the Dark Sword.

## 1.0.3-Hotfix <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

Hotfix for Null-Reference Exception when opening Clint's shop menu.

## 1.0.2 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Added

* Defense overhaul setting now also reduces Jinxed debuff from -8 to -5 defense.

### Fixed

* Fixed the overlaping ammo stacks displaying on slingshots with double ammo.
* Fixed the shop selling logic which was messed up in 1.0.1.
* Fixed the tooltip of weapons enchanted with Diamond.
* Dark Sword should now be immune from accidental deposit by Better Chests' stack feature.
* Clint's menu should now support mod geodes like Kaya's.

## 1.0.1 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Added

* Added an option to change the display style of weapon tooltips between relative and absolute values.
* Added colorful swipe effect for Blade of Ruin and Blade of Dawn, as well as Infinity Gavel smash hit.
* Legendary weapons can no longer be sold.
* Enchanted weapons now sell for a premium (+1000g).

### Changed

* Minimal edits to Blade of Dawn description.

### Fixed

* Galaxy and Infinity weapons, Blade of Ruin and Blade of Dawn are now marked as Special and cannot be trashed.
* Fixed a bug with Garnet resonance.
* Improved the response of FaceMouseCursor feature in-between combo hits.
* Fixed the direction of the swipe effect on backhand sword swipes.
* No longer conflicts with Love Of Cooking's Frying Pan upgrades or drbirbdev's Panning and Ranching Tool Upgrades.
* No longer prevents players from receiving the Galaxy Sword mail flag.

## 1.0.0 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Added

* Added flavor text for each virtue at the Yoba altar.

### Changed

* Removed Gil requirement for advancing the BOTRK quest-line. Now, simply exhausting the Yoba altar's dialogue options will advance the quest.
* Improved the textures for Blade of Ruin and Blade of Dawn.

### Fixed

* Garnet Enchantment can now be applied to weapons correctly.
* Garnet Enchantment's effect will now apply correctly.
* Fixed an issue where holding anything besides a weapon or slingshot would cause the player to take 0 damage.
* Fixed a possible Null-Reference Exception when receiving a Galaxy weapon.
* Infinity weapons now have 4 forge slots as advertised under the rebalance.
* Fixed a bug with Ruby Enchantment applying only a single point of damage.
* Weapon and Slingshot Forges now unapply correctly.
* Hoe is not longer classified as Rare (color-coded Blue).

## 0.9.9 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Added

* Ruined Blade quest-line is now fully implemented.
* Added option to draw bulls-eye instead of mouse cursor while firing a slingshot.

### Changed

* Few translations changed.
* Reduced the aura on the Blade of Ruin texture.

### Fixed

* Small fix for Slick Moves to prevent the player from sliding during combo cooldown.
* Scythes can no longer perform the stabby special move.
* The item hold up animation now plays correctly when obtaining the Blade of Ruin.
* The Blade of Ruin is now a Stabbing Sword, as it should.
* No longer triggers stabbing special move when obtaining the Blade of Dawn.
* Fixed possible bug with Bloodthirsty enchantment.
* Fixed some incorrect translation keys.

## 0.9.8 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Added

* The natural damage and knockback modifiers of advanced slingshots now display in the tooltip.

### Changed

* Improvements to Slick Moves. Can now drift in the parallel direction (i.e., when swinging directly in front or behind you). Before, drifting only worked for perpendicular directions (i.e., when swinging to the sides). Also increased the drift velocity slightly. Finally, the drift now comes to a halt at the end of the animation, removing the unimmersive impression of buttery floor.
* Clint's follow-up quest now begins correctly.

## 0.9.7 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Changed

* SVE's Treasure Cave now rewards an Obsidian Edge instead of Galaxy Slingshot.
* Clint's forging quest is no longer random, and now takes less time the higher your friendship with him.
* Changed the knockback of some weapons:
    * Wooden Blade: +0.25
    * Rapier: -0.05
    * Steel Falchion: -0.05
    * Katana: -0.2
* The Dark Sword's curse has been changed. No longer affects stamina consumption. Now, as the curse strengthens the sword will gain a progressively higher chance to auto-equip itself, preventing you from using other weapons.
* Community Center is no longer a requirement for obtaining the Dark Sword.

### Fixed

* Fixed an issue which caused critical hits to deal absurdly low damage.
* Fixed an error that could be thrown when playing the hold-up-item animation.
* Clint's forging quest was still bugged. Now tested.
* Fixed a Null-Reference Exception when attempting to grab the Dwarvish Blueprint from a Scavenger Hunt chest.
* Fixed a Null-Reference Exception in pirate treasure menu when a Neptune's Glaive is produced.
* Fixed missing patch targets for allowing crits to ignore monster defense.
* Added config checks to logic that was missing it.

## 0.9.6 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Fixed

* The event which counts the days left until Clint finishes translating the blueprints should now trigger correctly.
* Fixed a compatibility issue with any modded recipes containing Dragon Tooth.
* Fixed a null-reference exception when farmer takes damage from bombs and maybe other sources too.

## 0.9.5 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Added

* Added Initialize command to apply all necessary configurations to weapons and farmers on existing saves.

### Fixed

* Club second combo hit now actually does damage.

### Removed

* Removed automatic initialization script from SaveLoaded event. This was inefficient and unreliable. Replaced with manual console command.

## 0.9.4 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Added

* If SVE is installed and Infinity +1 feature is enabled, Tempered Galaxy weapons will now be automatically removed from Alesia and Isaac's shops, so users no longer have to manually remove them from SVE's shop.json file. 

### Changed

* Slingshot special move cooldown is now a property of Farmer and not Slingshot, as it should be.
* Default values for difficulty sliders are now set to 1.
* The base damage of some starter weapons has been slightly increased.

### Fixed

* Fixed Slick Moves applying to Scythe.
* Fixed Brute prestige perk not applying with Arsenal module enabled.
* Fixed Galaxy Slingshot being instantiated as Melee Weapon.
* Fixed special item hold up message for first Galaxy weapon not being displayed.
* Fixed Mine chests changes not being applied as intended with the Weapon Rebalance setting.
* Players on existing save files that have already obtained the Galaxy Sword should now be able to obtain the remaining Galaxy weapons.
* Fixed a bug in the monster stat randomization logic, which was generating monsters with current HP higher than max HP.

## 0.9.3 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Fixed

* Fixed null-reference exception when opening Marlon's shop (forgot to pass `__result` by `ref`).
* Control settings now apply only to weapons, as they should.

## 0.9.2 <sup><sup>[🔼 Back to top](#slngs-changelog)</sup></sup>

### Changed

* Weapon tooltips now revert to vanilla when `RebalancedStats` option is disabled.
* The category of Dwarven Blueprint has been changed from Artifact to Junk. This avoids the error caused by the game attempting to spawn a blueprint when the player digs an artifact spot.

### Fixed

* Added even more robust null-checking for custom JA items to avoid issues.
* Fixed SlickMoves config setting in GMCM which was incorrectly mapped to FaceMouseCursor.
* Fixed player's facing direction changing during active menu.

## 0.9.0 (Initial release)

* Initial Version

[🔼 Back to top](#slngs-changelog)