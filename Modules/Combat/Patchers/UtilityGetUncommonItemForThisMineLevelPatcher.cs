namespace DaLion.Overhaul.Modules.Combat.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Combat.Extensions;
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
        if (CombatModule.Config.EnableWeaponOverhaul && __result is MeleeWeapon weapon)
        {
            weapon.RandomizeDamage();
        }
    }

    #endregion harmony patches
}
