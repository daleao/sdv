 Chargeable Resource Tools  


This mod is inspired by the tools progression system of old Harvest Moon: FoMT, where the ultimate tool upgrades would let you instantly destroy all debris on the current screen.




With this mod, you will be able to charge your Axe and Pickaxe to unleash an area of effect shockwave.



You can configure the maximum radius of affected tiles for each of the four upgrade levels (in order: copper, steel, gold and iridium). These values must be positive (obviously). Set to zero to disable at any particular upgrade level. The circle shape itself is uses the same algorithm as bomb explosions.

By default, the maximum radius is equal to the tool's upgrade level. The shockwave is set to only clear debris (like stones and twigs), weeds, dead crops and resource clumps (like stumps, logs and boulders), as well as mining nodes. Trees, live crops and placed objects will be unaffected. You are free to change these settings and choose which terrain features and objects you would like to be affected by the shockwave (though you can't do anything the Axe or Pickaxe can't ordinarily do).

Should be compatible with all tool enchants.




'RequiredUpgradeLevelForCharging': This is the minimum upgrade level your tool must be at in order to enable charging. Accepts integer values (0 -> Base; 1 -> Copper; 2 -> Steel; 3 -> Gold; 4 -> Iridium; 5 -> Prismatic or Radioactive, if you have either of those mods installed).
'RadiusAtEachLevel': Allows you to specify a custom radius for the shockwave at each charging level. Note that your charging level is separate from your upgrade level. For instance, if 'RequiredUpgradeLevelForCharging' set to 4, and 'RadiusAtEachLevel' set to [ 1, 2, 3, 4 ], then you will not be able to charge until the tool is Iridium level, but once it is, then your charging progression will be similar to the gif above (starting at 1, and increase by 1 until 4). If you wanted to skip charging up and instantly get the max radius, you could set all four values to the same number (and set 'ShowAffectedTiles' to false to avoid the overlay instantly appearing). If you have Prismatic or Radioactive Tools mod installed, a fifth value will be added automatically to the list. Only natural numbers (non-negative integers) are accepted.
'RequireModKey': Set to false if you want charging behavior to be the default when holding down the tool button. Set to true if you prefer the default tool spamming behavior.
'ModKey': If 'RequireModKey' is true, you must hold this key in order to charge (default LeftShift). If you play with a gamepad controller you can set this to LeftTrigger or LeftShoulder. Check here for a list of available keybinds. You can set multiple comma-separated keys.
'StaminaCostMultiplier': By default, charging multiplies your tool's base stamina cost by the charging level. Use this multiplier to adjust the cost of the shockwave only. Set to zero to make it free (you will still lose stamina equal to the base tool cost). Accepts any real number greater than zero.
Other settings are self explanatory. Use Generic Mod Config Menu if you need verbatim explanations.




This mod uses Harmony to patch the behavior of Axe and Pickaxe. Any mods that also directly patch Tool behavior might be incompatible.
Compatible with Prismatic Tools and Radioactive Tools.
Compatible with Harvest Moon FoMT-like Watering Can And Hoe Area.
Compatible with Generic Mod Config Menu.




Install like any other mod, by extracting the content of the downloaded zip file to your mods folder and starting the game via SMAPI.
To update, first delete the old version and then install the new one. You can optionally keep your configs.json in case you have personalized settings.
To uninstall simply delete the mod from your mods folder. This mod is safe to uninstall at any point.




Pathoschild for their TractorMod, from which this mod steals borrows much of its code.
ConcernedApe for StardewValley.




﻿GitLab


Check out my other mod Walk Of Life﻿ if you want your professions to be awesome too!
