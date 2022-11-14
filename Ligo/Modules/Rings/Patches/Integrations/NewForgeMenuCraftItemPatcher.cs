namespace DaLion.Ligo.Modules.Rings.Patches;

#region using directives

using Shared.Networking;
using HarmonyLib;
using Shared.Harmony;
using SpaceCore.Interface;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class NewForgeMenuCraftItemPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NewForgeMenuCraftItemPatcher"/> class.</summary>
    internal NewForgeMenuCraftItemPatcher()
    {
        this.Target = this.RequireMethod<NewForgeMenu>(nameof(NewForgeMenu.CraftItem));
    }

    #region harmony patches

    /// <summary>Allow forging Infinity Band.</summary>
    [HarmonyPostfix]
    private static void NewForgeMenuCraftItemPostfix(ref Item? __result, Item? left_item, Item? right_item, bool forReal)
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
