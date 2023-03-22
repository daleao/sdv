namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Weapons;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        if (!ArsenalModule.Config.Weapons.EnableRebalance)
        {
            return true; // run original logic
        }

        try
        {
            var stock = new Dictionary<ISalable, int[]>
            {
                { new MeleeWeapon(ItemIDs.WoodenBlade), new[] { 200, int.MaxValue } },
            };

            stock.Add(new MeleeWeapon(ItemIDs.SteelSmallsword), new[] { 600, int.MaxValue });
            stock.Add(new MeleeWeapon(ItemIDs.SilverSaber), new[] { 800, int.MaxValue });
            stock.Add(new MeleeWeapon(ItemIDs.CarvingKnife), new[] { 350, int.MaxValue });
            stock.Add(new MeleeWeapon(ItemIDs.WoodClub), new[] { 250, int.MaxValue });

            if (MineShaft.lowestLevelReached >= 15)
            {
                stock.Add(new MeleeWeapon(ItemIDs.Cutlass), new[] { 1200, int.MaxValue });
                stock.Add(new MeleeWeapon(ItemIDs.IronEdge), new[] { 2000, int.MaxValue });
                stock.Add(new MeleeWeapon(ItemIDs.BurglarsShank), new[] { 600, int.MaxValue });
                stock.Add(new MeleeWeapon(ItemIDs.WoodMallet), new[] { 750, int.MaxValue });
            }

            if (MineShaft.lowestLevelReached >= 35)
            {
                stock.Add(new MeleeWeapon(ItemIDs.Rapier), new[] { 5000, int.MaxValue });
                stock.Add(new MeleeWeapon(ItemIDs.Claymore), new[] { 6500, int.MaxValue });
                stock.Add(new MeleeWeapon(ItemIDs.WindSpire), new[] { 1200, int.MaxValue });
                stock.Add(new MeleeWeapon(ItemIDs.LeadRod), new[] { 6000, int.MaxValue });
            }

            if (MineShaft.lowestLevelReached >= 75)
            {
                stock.Add(new MeleeWeapon(ItemIDs.SteelFalchion), new[] { 12500, int.MaxValue });
                stock.Add(new MeleeWeapon(ItemIDs.TemperedBroadsword), new[] { 15000, int.MaxValue });
                stock.Add(new MeleeWeapon(ItemIDs.IronDirk), new[] { 6000, int.MaxValue });
                stock.Add(new MeleeWeapon(ItemIDs.Kudgel), new[] { 10000, int.MaxValue });
            }

            if (MineShaft.lowestLevelReached >= 115)
            {
                stock.Add(new MeleeWeapon(ItemIDs.TemplarsBlade), new[] { 50000, int.MaxValue });
                stock.Add(new MeleeWeapon(ItemIDs.WickedKris), new[] { 24000, int.MaxValue });
                stock.Add(new MeleeWeapon(ItemIDs.TheSlammer), new[] { 65000, int.MaxValue });
            }

            stock.Add(new Boots(504), new[] { 500, int.MaxValue }); // sneakers
            if (MineShaft.lowestLevelReached >= 20)
            {
                stock.Add(new Boots(506), new[] { 800, int.MaxValue }); // leather boots
            }

            if (MineShaft.lowestLevelReached >= 40)
            {
                stock.Add(new Boots(509), new[] { 1000, int.MaxValue }); // tundra boots
            }

            if (MineShaft.lowestLevelReached >= 60)
            {
                stock.Add(new Boots(508), new[] { 1500, int.MaxValue }); // combat boots
            }

            if (MineShaft.lowestLevelReached >= 80)
            {
                stock.Add(new Boots(512), new[] { 5000, int.MaxValue }); // firewalker boots
            }

            if (MineShaft.lowestLevelReached >= 120)
            {
                stock.Add(new Boots(514), new[] { 20000, int.MaxValue }); // space boots
            }

            if (!RingsModule.IsEnabled || !RingsModule.Config.CraftableGemRings)
            {
                stock.Add(new Ring(ItemIDs.AmethystRing), new[] { 1000, int.MaxValue });
                stock.Add(new Ring(ItemIDs.TopazRing), new[] { 1000, int.MaxValue });
                if (MineShaft.lowestLevelReached >= 40)
                {
                    stock.Add(new Ring(ItemIDs.AquamarineRing), new[] { 2500, int.MaxValue });
                    stock.Add(new Ring(ItemIDs.JadeRing), new[] { 2500, int.MaxValue });
                }

                if (MineShaft.lowestLevelReached >= 80)
                {
                    stock.Add(new Ring(ItemIDs.EmeraldRing), new[] { 5000, int.MaxValue });
                    stock.Add(new Ring(ItemIDs.RubyRing), new[] { 5000, int.MaxValue });
                }
            }

            stock.Add(new Slingshot(), new[] { 500, int.MaxValue });
            if (MineShaft.lowestLevelReached >= 50)
            {
                stock.Add(new Slingshot(ItemIDs.MasterSlingshot), new[] { 1000, int.MaxValue });
            }

            if (Game1.player.craftingRecipes.ContainsKey("Explosive Ammo"))
            {
                stock.Add(new SObject(ItemIDs.ExplosiveAmmo, 1), new[] { 300, int.MaxValue });
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
        if (!ArsenalModule.Config.InfinityPlusOne)
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
