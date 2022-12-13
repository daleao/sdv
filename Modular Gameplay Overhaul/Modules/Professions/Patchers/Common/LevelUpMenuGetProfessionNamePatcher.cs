namespace DaLion.Overhaul.Modules.Professions.Patchers.Common;

#region using directives

using System.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class LevelUpMenuGetProfessionNamePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="LevelUpMenuGetProfessionNamePatcher"/> class.</summary>
    internal LevelUpMenuGetProfessionNamePatcher()
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
