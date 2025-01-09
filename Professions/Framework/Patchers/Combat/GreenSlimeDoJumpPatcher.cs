namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class GreenSlimeDoJumpPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GreenSlimeDoJumpPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal GreenSlimeDoJumpPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<GreenSlime>("doJump");
    }

    #region harmony patches

    /// <summary>Patch to detect jumping Slimes.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static void GreenSlimeDoJumpPrefix(GreenSlime __instance)
    {
        __instance.Set_JumpTimer(200);
    }

    #endregion harmony patches
}
