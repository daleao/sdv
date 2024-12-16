namespace DaLion.Arsenal.Framework.Patchers.Infinity;

#region using directives

using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ChestAddItemPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ChestAddItemPatcher"/> class.</summary>
    internal ChestAddItemPatcher()
    {
        this.Target = this.RequireMethod<Chest>(nameof(Chest.addItem));
    }

    #region harmony patches

    /// <summary>Prevent depositing Dark Sword.</summary>
    [HarmonyPrefix]
    private static bool ChestAddItemPrefix(ref Item __result, Item item)
    {
        if (item is not MeleeWeapon { InitialParentTileIndex: WeaponIds.DarkSword } ||
            CombatModule.Config.Quests.CanStoreRuinBlade)
        {
            return true; // run original logic
        }

        __result = item;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
