namespace DaLion.Ligo.Modules.Professions.Patches.Combat;

#region using directives

using DaLion.Ligo.Modules.Professions.VirtualProperties;
using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
[Deprecated]
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
    private static bool GreenSlimeDoJumpPrefix(GreenSlime __instance)
    {
        __instance.Set_JumpTimer(200);
        return true; // run original logic
    }

    #endregion harmony patches
}
