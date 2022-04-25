namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Monsters;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal class GreenSlimeDoJumpPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal GreenSlimeDoJumpPatch()
    {
        //Original = RequireMethod<GreenSlime>("doJump");
    }

    #region harmony patches

    /// <summary>Patch to detect jumping Slimes.</summary>
    [HarmonyPrefix]
    private static bool GreenSlimeDoJumpPrefix(GreenSlime __instance)
    {
        __instance.WriteData("Jumping", 200.ToString());
        return true; // run original logic
    }

    #endregion harmony patches
}