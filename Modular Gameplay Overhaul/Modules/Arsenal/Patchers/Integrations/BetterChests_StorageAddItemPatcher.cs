namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[RequiresMod("furyx639.BetterChests")]
internal sealed class StorageAddItemPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="StorageAddItemPatcher"/> class.</summary>
    internal StorageAddItemPatcher()
    {
        this.Target = "StardewMods.BetterChests.StorageHandlers.BaseStorage"
            .ToType()
            .RequireMethod("AddItem");
    }

    #region harmony patches

    /// <summary>Prevent depositing Dark Sword.</summary>
    [HarmonyPrefix]
    private static bool StorageAddItemPrefix(ref Item __result, Item item)
    {
        if (item is not MeleeWeapon { InitialParentTileIndex: Constants.DarkSwordIndex })
        {
            return true; // run original logic
        }

        __result = item;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
