namespace DaLion.Ligo.Modules.Arsenal.Patchers;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerAddItemToInventoryPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerAddItemToInventoryPatcher"/> class.</summary>
    internal FarmerAddItemToInventoryPatcher()
    {
        this.Target =
            this.RequireMethod<Farmer>(nameof(Farmer.addItemToInventory), new[] { typeof(Item), typeof(int) });
    }

    /// <inheritdoc />
    protected override void ApplyImpl(Harmony harmony)
    {
        base.ApplyImpl(harmony);

        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.addItemToInventoryBool));
        base.ApplyImpl(harmony);
    }

    #region harmony patches

    /// <summary>Record unique weapons obtained.</summary>
    [HarmonyPostfix]
    private static void FarmerAddItemToInventoryPostfix(Farmer __instance, Item item)
    {
        if (item is MeleeWeapon { specialItem: true } weapon)
        {
            __instance.Append(DataFields.UniqueWeapons, weapon.InitialParentTileIndex.ToString());
        }
    }

    #endregion harmony patches
}
