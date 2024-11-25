namespace DaLion.Harmonics.Framework.Patchers;

#region using directives

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
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal ForgeMenuCraftItemPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<ForgeMenu>(nameof(ForgeMenu.CraftItem));
    }

    #region harmony patches

    /// <summary>Allow forging Infinity Band.</summary>
    [HarmonyPrefix]
    private static bool ForgeMenuCraftItemPrefix(ref Item? __result, Item? left_item, Item? right_item, bool forReal)
    {
        if (left_item?.QualifiedItemId != QualifiedObjectIds.IridiumBand ||
            right_item?.QualifiedItemId != QualifiedObjectIds.GalaxySoul)
        {
            return true; // run original logic
        }

        __result = ItemRegistry.Create<Ring>(InfinityBandId);
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
