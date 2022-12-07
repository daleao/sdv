namespace DaLion.Ligo.Modules.Arsenal.Patchers.Weapons;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class UtilityGetUncommonItemForThisMineLevelPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="UtilityGetUncommonItemForThisMineLevelPatcher"/> class.</summary>
    internal UtilityGetUncommonItemForThisMineLevelPatcher()
    {
        this.Target = this.RequireMethod<Utility>(nameof(Utility.getUncommonItemForThisMineLevel));
    }

    #region harmony patches

    /// <summary>Randomize Mine drops.</summary>
    [HarmonyPostfix]
    private static void UtilityGetUncommonItemForThisMineLevelPostfix(Item __result)
    {
        if (ModEntry.Config.Arsenal.Weapons.RebalancedWeapons && __result is MeleeWeapon weapon)
        {
            weapon.RefreshStats();
        }
    }

    #endregion harmony patches
}
