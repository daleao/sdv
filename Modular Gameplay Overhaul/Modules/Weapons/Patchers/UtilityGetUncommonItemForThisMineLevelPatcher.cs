namespace DaLion.Overhaul.Modules.Weapons.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Weapons.Extensions;
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
        if (WeaponsModule.Config.EnableRebalance && __result is MeleeWeapon weapon)
        {
            weapon.RefreshStats();
        }
    }

    #endregion harmony patches
}
