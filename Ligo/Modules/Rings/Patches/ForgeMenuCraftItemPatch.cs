namespace DaLion.Ligo.Modules.Rings.Patches;

#region using directives

using DaLion.Shared.Multiplayer;
using HarmonyLib;
using StardewValley.Menus;
using StardewValley.Objects;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class ForgeMenuCraftItemPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="ForgeMenuCraftItemPatch"/> class.</summary>
    internal ForgeMenuCraftItemPatch()
    {
        this.Target = this.RequireMethod<ForgeMenu>(nameof(ForgeMenu.CraftItem));
    }

    #region harmony patches

    /// <summary>Allow forging Infinity Band.</summary>
    [HarmonyPostfix]
    private static void ForgeMenuCraftItemPostfix(ref Item? __result, Item? left_item, Item? right_item, bool forReal)
    {
        if (left_item is not Ring { ParentSheetIndex: Constants.IridiumBandIndex } ||
            right_item?.ParentSheetIndex != Constants.GalaxySoulIndex)
        {
            return;
        }

        __result = new Ring(Globals.InfinityBandIndex);
        if (!forReal)
        {
            return;
        }

        DelayedAction.playSoundAfterDelay("discoverMineral", 400);
        if (Context.IsMultiplayer)
        {
            Broadcaster.SendPublicChat(ModEntry.i18n.Get("global.infinitycraft", new { who = Game1.player.Name }));
        }
    }

    #endregion harmony patches
}
