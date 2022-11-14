namespace DaLion.Ligo.Modules.Professions.Patches.Common;

#region using directives

using System.Reflection;
using HarmonyLib;
using StardewValley.Menus;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class LevelUpMenuGetProfessionNamePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="LevelUpMenuGetProfessionNamePatch"/> class.</summary>
    internal LevelUpMenuGetProfessionNamePatch()
    {
        this.Target = this.RequireMethod<LevelUpMenu>("getProfessionName");
    }

    #region harmony patches

    /// <summary>Patch to apply modded profession names.</summary>
    [HarmonyPrefix]
    private static bool LevelUpMenuGetProfessionNamePrefix(ref string __result, int whichProfession)
    {
        try
        {
            if (!Profession.TryFromValue(whichProfession, out var profession))
            {
                return true; // run original logic
            }

            __result = profession.Name;
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
