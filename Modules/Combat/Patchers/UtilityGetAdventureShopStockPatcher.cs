namespace DaLion.Overhaul.Modules.Combat.Patchers;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class UtilityGetAdventureShopStockPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="UtilityGetAdventureShopStockPatcher"/> class.</summary>
    internal UtilityGetAdventureShopStockPatcher()
    {
        this.Target = this.RequireMethod<Utility>(nameof(Utility.getAdventureShopStock));
    }

    #region harmony patches

    /// <summary>More consistent Adventurer Guild shop.</summary>
    [HarmonyPrefix]
    private static bool UtilityGetAdventureShopStockPrefix(ref Dictionary<ISalable, int[]> __result)
    {
        if (!CombatModule.Config.EnableWeaponOverhaul)
        {
            return true; // run original logic
        }

        try
        {
            var stock = new Dictionary<ISalable, int[]>
            {
                { new MeleeWeapon(WeaponIds.WoodenBlade), new[] { 200, int.MaxValue } },
                { new MeleeWeapon(WeaponIds.SteelSmallsword), new[] { 600, int.MaxValue } },
                { new MeleeWeapon(WeaponIds.SilverSaber), new[] { 800, int.MaxValue } },
                { new MeleeWeapon(WeaponIds.CarvingKnife), new[] { 350, int.MaxValue } },
                { new MeleeWeapon(WeaponIds.WoodClub), new[] { 250, int.MaxValue } },
            };

            if (MineShaft.lowestLevelReached >= 15)
            {
                stock.Add(new MeleeWeapon(WeaponIds.Cutlass), new[] { 1200, int.MaxValue });
                stock.Add(new MeleeWeapon(WeaponIds.IronEdge), new[] { 2000, int.MaxValue });
                stock.Add(new MeleeWeapon(WeaponIds.BurglarsShank), new[] { 600, int.MaxValue });
                stock.Add(new MeleeWeapon(WeaponIds.WoodMallet), new[] { 750, int.MaxValue });
            }

            if (MineShaft.lowestLevelReached >= 35)
            {
                stock.Add(new MeleeWeapon(WeaponIds.Rapier), new[] { 5000, int.MaxValue });
                stock.Add(new MeleeWeapon(WeaponIds.Claymore), new[] { 6500, int.MaxValue });
                stock.Add(new MeleeWeapon(WeaponIds.WindSpire), new[] { 1200, int.MaxValue });
                stock.Add(new MeleeWeapon(WeaponIds.LeadRod), new[] { 6000, int.MaxValue });
            }

            if (MineShaft.lowestLevelReached >= 75)
            {
                stock.Add(new MeleeWeapon(WeaponIds.SteelFalchion), new[] { 12500, int.MaxValue });
                stock.Add(new MeleeWeapon(WeaponIds.TemperedBroadsword), new[] { 15000, int.MaxValue });
                stock.Add(new MeleeWeapon(WeaponIds.IronDirk), new[] { 6000, int.MaxValue });
                stock.Add(new MeleeWeapon(WeaponIds.Kudgel), new[] { 10000, int.MaxValue });
            }

            if (MineShaft.lowestLevelReached >= 115)
            {
                stock.Add(new MeleeWeapon(WeaponIds.TemplarsBlade), new[] { 50000, int.MaxValue });
                stock.Add(new MeleeWeapon(WeaponIds.WickedKris), new[] { 24000, int.MaxValue });
                stock.Add(new MeleeWeapon(WeaponIds.TheSlammer), new[] { 65000, int.MaxValue });
            }

            stock.Add(new Boots(BootIds.Sneakers), new[] { 500, int.MaxValue });
            if (MineShaft.lowestLevelReached >= 20)
            {
                stock.Add(new Boots(BootIds.LeatherBoots), new[] { 800, int.MaxValue });
            }

            if (MineShaft.lowestLevelReached >= 40)
            {
                stock.Add(new Boots(BootIds.TundraBoots), new[] { 1000, int.MaxValue });
            }

            if (MineShaft.lowestLevelReached >= 60)
            {
                stock.Add(new Boots(BootIds.CombatBoots), new[] { 1500, int.MaxValue });
            }

            if (MineShaft.lowestLevelReached >= 80)
            {
                stock.Add(new Boots(BootIds.FirewalkerBoots), new[] { 5000, int.MaxValue });
            }

            if (MineShaft.lowestLevelReached >= 120)
            {
                stock.Add(new Boots(BootIds.SpaceBoots), new[] { 20000, int.MaxValue });
            }

            if (!CombatModule.Config.CraftableGemstoneRings)
            {
                stock.Add(new Ring(ObjectIds.AmethystRing), new[] { 1000, int.MaxValue });
                stock.Add(new Ring(ObjectIds.TopazRing), new[] { 1000, int.MaxValue });
                if (MineShaft.lowestLevelReached >= 40)
                {
                    stock.Add(new Ring(ObjectIds.AquamarineRing), new[] { 2500, int.MaxValue });
                    stock.Add(new Ring(ObjectIds.JadeRing), new[] { 2500, int.MaxValue });
                }

                if (MineShaft.lowestLevelReached >= 80)
                {
                    stock.Add(new Ring(ObjectIds.EmeraldRing), new[] { 5000, int.MaxValue });
                    stock.Add(new Ring(ObjectIds.RubyRing), new[] { 5000, int.MaxValue });
                }
            }

            stock.Add(new Slingshot(), new[] { 500, int.MaxValue });
            if (MineShaft.lowestLevelReached >= 50)
            {
                stock.Add(new Slingshot(WeaponIds.MasterSlingshot), new[] { 1000, int.MaxValue });
            }

            if (Game1.player.craftingRecipes.ContainsKey("Explosive Ammo"))
            {
                stock.Add(new SObject(ObjectIds.ExplosiveAmmo, 1), new[] { 300, int.MaxValue });
            }

            var rotatingStock = new Dictionary<ISalable, int[]>();
            if (Game1.player.mailReceived.Contains("Gil_Slime Charmer Ring"))
            {
                rotatingStock.Add(new Ring(520), new[] { 25000, int.MaxValue });
            }

            if (Game1.player.mailReceived.Contains("Gil_Savage Ring"))
            {
                rotatingStock.Add(new Ring(523), new[] { 25000, int.MaxValue });
            }

            if (Game1.player.mailReceived.Contains("Gil_Burglar's Ring"))
            {
                rotatingStock.Add(new Ring(526), new[] { 20000, int.MaxValue });
            }

            if (Game1.player.mailReceived.Contains("Gil_Vampire Ring"))
            {
                rotatingStock.Add(new Ring(522), new[] { 15000, int.MaxValue });
            }

            if (Game1.player.mailReceived.Contains("Gil_Crabshell Ring"))
            {
                rotatingStock.Add(new Ring(810), new[] { 15000, int.MaxValue });
            }

            if (Game1.player.mailReceived.Contains("Gil_Napalm Ring"))
            {
                rotatingStock.Add(new Ring(811), new[] { 30000, int.MaxValue });
            }

            if (Game1.player.mailReceived.Contains("Gil_Skeleton Mask"))
            {
                rotatingStock.Add(new Hat(8), new[] { 20000, int.MaxValue });
            }

            if (Game1.player.mailReceived.Contains("Gil_Hard Hat"))
            {
                rotatingStock.Add(new Hat(27), new[] { 20000, int.MaxValue });
            }

            if (Game1.player.mailReceived.Contains("Gil_Arcane Hat"))
            {
                rotatingStock.Add(new Hat(60), new[] { 20000, int.MaxValue });
            }

            if (Game1.player.mailReceived.Contains("Gil_Knight's Helmet"))
            {
                rotatingStock.Add(new Hat(50), new[] { 20000, int.MaxValue });
            }

            if (Game1.player.mailReceived.Contains("Gil_Insect Head"))
            {
                rotatingStock.Add(new MeleeWeapon(13), new[] { 10000, int.MaxValue });
            }

            if (rotatingStock.Count > 0)
            {
                var selected = rotatingStock.ElementAt(Game1.dayOfMonth % rotatingStock.Count);
                stock.Add(selected.Key, selected.Value);
            }

            __result = stock;
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    /// <summary>Remove Galaxy weapons from shop, if the weapon rebalance is disabled.</summary>
    [HarmonyPostfix]
    private static void UtilityGetAdventureShopStockPostfix(Dictionary<ISalable, int[]> __result)
    {
        if (CombatModule.Config.EnableWeaponOverhaul || !CombatModule.Config.EnableHeroQuest)
        {
            return;
        }

        for (var i = __result.Count - 1; i >= 0; i--)
        {
            var salable = __result.ElementAt(i).Key;
            if (salable is MeleeWeapon weapon && weapon.isGalaxyWeapon())
            {
                __result.Remove(salable);
            }
        }
    }

    #endregion harmony patches
}
