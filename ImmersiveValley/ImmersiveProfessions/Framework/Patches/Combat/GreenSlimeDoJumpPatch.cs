namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using DaLion.Common.Attributes;
using DaLion.Common.Extensions.Stardew;
using HarmonyLib;
using StardewValley.Monsters;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[Deprecated]
internal sealed class GreenSlimeDoJumpPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="GreenSlimeDoJumpPatch"/> class.</summary>
    internal GreenSlimeDoJumpPatch()
    {
        this.Target = this.RequireMethod<GreenSlime>("doJump");
    }

    #region harmony patches

    /// <summary>Patch to detect jumping Slimes.</summary>
    [HarmonyPrefix]
    private static bool GreenSlimeDoJumpPrefix(GreenSlime __instance)
    {
        __instance.Write(DataFields.Jumping, 200.ToString());
        return true; // run original logic
    }

    #endregion harmony patches
}
