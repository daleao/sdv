namespace DaLion.Overhaul.Modules.Professions.Patchers.Combat;

#region using directives

using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class GreenSlimeDoJumpPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GreenSlimeDoJumpPatcher"/> class.</summary>
    internal GreenSlimeDoJumpPatcher()
    {
        this.Target = this.RequireMethod<GreenSlime>("doJump");
    }

    #region harmony patches

    /// <summary>Patch to detect jumping Slimes.</summary>
    [HarmonyPrefix]
    private static void GreenSlimeDoJumpPrefix(GreenSlime __instance)
    {
        __instance.Set_JumpTimer(200);
    }

    #endregion harmony patches
}
