// ReSharper disable PossibleLossOfFraction
namespace DaLion.Overhaul.Modules.Combat.Patchers.Melee;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponGetItemLevelPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponGetItemLevelPatcher"/> class.</summary>
    internal MeleeWeaponGetItemLevelPatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.getItemLevel));
    }

    #region harmony patches

    /// <summary>Adjust weapon level.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponGetItemLevelPrefix(MeleeWeapon __instance, ref int __result)
    {
        if (!CombatModule.Config.EnableWeaponOverhaul)
        {
            return true; // run original logic
        }

        try
        {
            __result = __instance.Get_Level();
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
