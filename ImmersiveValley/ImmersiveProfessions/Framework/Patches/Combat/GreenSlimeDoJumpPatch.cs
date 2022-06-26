namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using DaLion.Common.Data;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class GreenSlimeDoJumpPatch : DaLion.Common.Harmony.HarmonyPatch
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