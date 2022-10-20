namespace DaLion.Stardew.Professions.Framework.Patches.Common;

#region using directives

using System.Reflection;
using HarmonyLib;
using StardewValley.Menus;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class LevelUpMenuGetProfessionTitleFromNumberPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="LevelUpMenuGetProfessionTitleFromNumberPatch"/> class.</summary>
    internal LevelUpMenuGetProfessionTitleFromNumberPatch()
    {
        this.Target = this.RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.getProfessionTitleFromNumber));
    }

    #region harmony patches

    /// <summary>Patch to apply modded profession names.</summary>
    [HarmonyPrefix]
    private static bool LevelUpMenuGetProfessionTitleFromNumberPrefix(ref string __result, int whichProfession)
    {
        try
        {
            if (!Profession.TryFromValue(whichProfession, out var profession) ||
                (Skill)profession.Skill == Farmer.luckSkill)
            {
                return true; // run original logic
            }

            __result = profession.DisplayName;
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
