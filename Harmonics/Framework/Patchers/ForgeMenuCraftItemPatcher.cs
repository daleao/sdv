namespace DaLion.Overhaul.Modules.Combat.Patchers.Rings;

#region using directives

using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using DaLion.Shared.Networking;
using HarmonyLib;
using StardewValley.Menus;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class ForgeMenuCraftItemPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ForgeMenuCraftItemPatcher"/> class.</summary>
    internal ForgeMenuCraftItemPatcher()
    {
        this.Target = this.RequireMethod<ForgeMenu>(nameof(ForgeMenu.CraftItem));
    }

    #region harmony patches

    /// <summary>Allow forging Infinity Band.</summary>
    [HarmonyPrefix]
    private static bool ForgeMenuCraftItemPrefix(ref Item? __result, Item? left_item, Item? right_item, bool forReal)
    {
        if (!CombatModule.Config.RingsEnchantments.EnableInfinityBand || !JsonAssetsIntegration.InfinityBandIndex.HasValue ||
            left_item is not Ring { ParentSheetIndex: ObjectIds.IridiumBand } ||
            right_item?.ParentSheetIndex != ObjectIds.GalaxySoul)
        {
            return true; // run original logic
        }

        __result = new Ring(JsonAssetsIntegration.InfinityBandIndex.Value);
        if (!forReal)
        {
            return false; // don't run original logic
        }

        DelayedAction.playSoundAfterDelay("discoverMineral", 400);
        if (Context.IsMultiplayer)
        {
            Broadcaster.SendPublicChat(I18n.Global_Infinitycraft(Game1.player.Name));
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}
