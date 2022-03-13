namespace DaLion.Stardew.Professions.Framework.Patches.Common;

#region using directives

using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Menus;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal class LevelUpMenuGetProfessionTitleFromNumberPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal LevelUpMenuGetProfessionTitleFromNumberPatch()
    {
        Original = RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.getProfessionTitleFromNumber));
    }

    #region harmony patches

    /// <summary>Patch to apply modded profession names.</summary>
    [HarmonyPrefix]
    private static bool LevelUpMenuGetProfessionTitleFromNumberPrefix(ref string __result, int whichProfession)
    {
        try
        {
            if (!Enum.IsDefined(typeof(Profession), whichProfession)) return true; // run original logic

            __result = ModEntry.ModHelper.Translation.Get(whichProfession.ToProfessionName() + ".name." +
                                                          (Game1.player.IsMale ? "male" : "female"));
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