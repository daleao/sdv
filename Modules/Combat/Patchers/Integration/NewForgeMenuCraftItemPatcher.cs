namespace DaLion.Overhaul.Modules.Combat.Patchers.Integration;

#region using directives

using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Shared.Attributes;
using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using DaLion.Shared.Networking;
using HarmonyLib;
using SpaceCore.Interface;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
[ModRequirement("spacechase0.SpaceCore")]
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
        if (!CombatModule.Config.EnableInfinityBand || !JsonAssetsIntegration.InfinityBandIndex.HasValue ||
            left_item is not Ring { ParentSheetIndex: ObjectIds.IridiumBand } ||
            right_item?.ParentSheetIndex != ObjectIds.GalaxySoul)
        {
            return;
        }

        __result = new Ring(JsonAssetsIntegration.InfinityBandIndex!.Value);
        if (!forReal)
        {
            return;
        }

        DelayedAction.playSoundAfterDelay("discoverMineral", 400);
        if (Context.IsMultiplayer)
        {
            Broadcaster.SendPublicChat(I18n.Global_Infinitycraft(Game1.player.Name));
        }
    }

    #endregion harmony patches
}
