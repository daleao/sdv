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
                { new MeleeWeapon(Constants.WoodenBladeIndex), new[] { 200, int.MaxValue } },
            };

            if (MineShaft.lowestLevelReached >= 20)
            {
                stock.Add(new MeleeWeapon(Constants.SteelSmallswordIndex), new[] { 500, int.MaxValue });
                stock.Add(new MeleeWeapon(Constants.SilverSaberIndex), new[] { 800, int.MaxValue });
                stock.Add(new MeleeWeapon(Constants.CarvingKnife), new[] { 350, int.MaxValue });
                stock.Add(new MeleeWeapon(Constants.WoodClubIndex), new[] { 250, int.MaxValue });
            }

            if (MineShaft.lowestLevelReached >= 40)
            {
                stock.Add(new MeleeWeapon(Constants.CutlassIndex), new[] { 1200, int.MaxValue });
                stock.Add(new MeleeWeapon(Constants.IronEdgeIndex), new[] { 2000, int.MaxValue });
                stock.Add(new MeleeWeapon(Constants.BurglarsShankIndex), new[] { 400, int.MaxValue });
                stock.Add(new MeleeWeapon(Constants.WoodMalletIndex), new[] { 500, int.MaxValue });
            }

            if (MineShaft.lowestLevelReached >= 80)
            {
                stock.Add(new MeleeWeapon(Constants.RapierIndex), new[] { 5000, int.MaxValue });
                stock.Add(new MeleeWeapon(Constants.ClaymoreIndex), new[] { 10000, int.MaxValue });
                stock.Add(new MeleeWeapon(Constants.WindSpireIndex), new[] { 1200, int.MaxValue });
                stock.Add(new MeleeWeapon(Constants.LeadRodIndex), new[] { 8000, int.MaxValue });
            }

            if (MineShaft.lowestLevelReached >= 120)
            {
                stock.Add(new MeleeWeapon(Constants.SteelFalchionIndex), new[] { 15000, int.MaxValue });
                stock.Add(new MeleeWeapon(Constants.TemperedBroadswordIndex), new[] { 20000, int.MaxValue });
                stock.Add(new MeleeWeapon(Constants.IronDirkIndex), new[] { 8000, int.MaxValue });
                stock.Add(new MeleeWeapon(Constants.KudgelIndex), new[] { 12000, int.MaxValue });
            }

            if (MineShaft.lowestLevelReached >= 220)
            {
                stock.Add(new MeleeWeapon(Constants.TemplarsBladeIndex), new[] { 50000, int.MaxValue });
                stock.Add(new MeleeWeapon(Constants.WickedKrisIndex), new[] { 25000, int.MaxValue });
                stock.Add(new MeleeWeapon(Constants.TheSlammerIndex), new[] { 80000, int.MaxValue });
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
                stock.Add(new Ring(Constants.AmethystRingIndex), new[] { 1000, int.MaxValue });
                stock.Add(new Ring(Constants.TopazRingIndex), new[] { 1000, int.MaxValue });
                if (MineShaft.lowestLevelReached >= 40)
                {
                    stock.Add(new Ring(Constants.AquamarineRingIndex), new[] { 2500, int.MaxValue });
                    stock.Add(new Ring(Constants.JadeRingIndex), new[] { 2500, int.MaxValue });
                }

                if (MineShaft.lowestLevelReached >= 80)
                {
                    stock.Add(new Ring(Constants.EmeraldRingIndex), new[] { 5000, int.MaxValue });
                    stock.Add(new Ring(Constants.RubyRingIndex), new[] { 5000, int.MaxValue });
                }
            }

            stock.Add(new Slingshot(), new[] { 500, int.MaxValue });
            if (MineShaft.lowestLevelReached >= 50)
            {
                stock.Add(new Slingshot(Constants.MasterSlingshotIndex), new[] { 1000, int.MaxValue });
            }

            if (Game1.player.craftingRecipes.ContainsKey("Explosive Ammo"))
            {
                stock.Add(new SObject(Constants.ExplosiveAmmoIndex, 1), new[] { 300, int.MaxValue });
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
