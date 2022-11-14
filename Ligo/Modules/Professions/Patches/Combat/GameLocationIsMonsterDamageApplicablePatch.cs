namespace DaLion.Ligo.Modules.Professions.Patches.Combat;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using HarmonyLib;
using StardewValley.Monsters;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationIsMonsterDamageApplicablePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationIsMonsterDamageApplicablePatch"/> class.</summary>
    internal GameLocationIsMonsterDamageApplicablePatch()
    {
        this.Target = this.RequireMethod<GameLocation>("isMonsterDamageApplicable");
    }

    #region harmony patches

    /// <summary>Patch to make Slimes immune to slime ammo.</summary>
    [HarmonyPrefix]
    private static bool GameLocationIsMonsterDamageApplicablePrefix(
        ref bool __result, Farmer who, Monster monster)
    {
        if (!monster.IsSlime() || who.CurrentTool is not Slingshot slingshot ||
            slingshot.attachments[0]?.ParentSheetIndex != Constants.SlimeIndex)
        {
            return true; // run original logic
        }

        __result = false;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
