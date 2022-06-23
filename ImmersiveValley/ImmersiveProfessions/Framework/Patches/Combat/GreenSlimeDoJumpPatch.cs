namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Monsters;

using DaLion.Common.Data;
using DaLion.Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal sealed class GreenSlimeDoJumpPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal GreenSlimeDoJumpPatch()
    {
        //Target = RequireMethod<GreenSlime>("doJump");
    }

    #region harmony patches

    /// <summary>Patch to detect jumping Slimes.</summary>
    [HarmonyPrefix]
    private static bool GreenSlimeDoJumpPrefix(GreenSlime __instance)
    {
        ModDataIO.WriteData(__instance, "Jumping", 200.ToString());
        return true; // run original logic
    }

    #endregion harmony patches
}