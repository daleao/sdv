namespace DaLion.Overhaul.Modules.Slingshots.Patchers;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Slingshots.VirtualProperties;
using DaLion.Overhaul.Modules.Weapons.Enchantments;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolForgePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolForgePatcher"/> class.</summary>
    internal ToolForgePatcher()
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.Forge));
    }

    #region harmony patches


    /// <summary>Transform Galaxy Slingshot into Infinity Slingshot.</summary>
    [HarmonyPrefix]
    private static bool ToolForgePrefix(Tool __instance, ref bool __result, Item item, bool count_towards_stats)
    {
        if (__instance is not Slingshot { InitialParentTileIndex: ItemIDs.GalaxySlingshot } slingshot ||
            !SlingshotsModule.Config.EnableInfinitySlingshot)
        {
            return true; // run original logic
        }

        try
        {
            var enchantment = BaseEnchantment.GetEnchantmentFromItem(__instance, item);
            if (WeaponsModule.ShouldEnable && WeaponsModule.Config.InfinityPlusOne)
            {
                if (enchantment is not InfinityEnchantment)
                {
                    return true; // run original logic
                }
            }
            else
            {
                if (enchantment is not GalaxySoulEnchantment)
                {
                    return true; // run original logic
                }
            }

            if (!slingshot.AddEnchantment(enchantment) || slingshot.GetEnchantmentLevel<GalaxySoulEnchantment>() < 3)
            {
                return true; // run original logic
            }

            slingshot.CurrentParentTileIndex = ItemIDs.InfinitySlingshot;
            slingshot.InitialParentTileIndex = ItemIDs.InfinitySlingshot;
            slingshot.IndexOfMenuItemView = ItemIDs.InfinitySlingshot;
            slingshot.BaseName = "Infinity Slingshot";
            slingshot.DisplayName = I18n.Get("slingshots.infinity.name");
            slingshot.description = I18n.Get("slingshots.infinity.desc");
            if (count_towards_stats)
            {
                DelayedAction.playSoundAfterDelay("discoverMineral", 400);
                Reflector.GetStaticFieldGetter<Multiplayer>(typeof(Game1), "multiplayer").Invoke()
                    .globalChatInfoMessage("InfinityWeapon", Game1.player.Name, slingshot.DisplayName);

                slingshot.previousEnchantments.Insert(0, enchantment.GetName());
                while (slingshot.previousEnchantments.Count > 2)
                {
                    slingshot.previousEnchantments.RemoveAt(slingshot.previousEnchantments.Count - 1);
                }

                Game1.stats.incrementStat("timesEnchanted", 1);
            }

            var galaxyEnchantment = __instance.GetEnchantmentOfType<GalaxySoulEnchantment>();
            if (galaxyEnchantment is not null)
            {
                __instance.RemoveEnchantment(galaxyEnchantment);
            }

            __result = true;
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    /// <summary>Invalidate stats on forge.</summary>
    [HarmonyPostfix]
    private static void ToolForgePostfix(Tool __instance, bool __result)
    {
        if (__instance is Slingshot slingshot && __result)
        {
            slingshot.Invalidate();
        }
    }

    #endregion harmony patches
}
