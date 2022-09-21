namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using DaLion.Stardew.Professions.Extensions;
using HarmonyLib;
using StardewValley.Monsters;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationIsMonsterDamageApplicablePatch : HarmonyPatch
{
    private const int SlimeIndex = 766;

    /// <summary>Initializes a new instance of the <see cref="GameLocationIsMonsterDamageApplicablePatch"/> class.</summary>
    internal GameLocationIsMonsterDamageApplicablePatch()
    {
        this.Target = this.RequireMethod<GameLocation>("isMonsterDamageApplicable");
    }

    #region harmony patches

    /// <summary>Club smash aoe ignores gliders.</summary>
    [HarmonyPrefix]
    private static bool GameLocationIsMonsterDamageApplicablePrefix(
        GameLocation __instance, ref bool __result, Farmer who, Monster monster)
    {
        if (!monster.IsSlime() || who.CurrentTool is not Slingshot slingshot ||
            slingshot.attachments[0].ParentSheetIndex != SlimeIndex)
        {
            return true; // run original logic
        }

        __result = false;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
