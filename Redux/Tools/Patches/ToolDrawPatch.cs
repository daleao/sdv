namespace DaLion.Redux.Tools.Patches;

#region using directives

using HarmonyLib;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolDrawPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="ToolDrawPatch"/> class.</summary>
    internal ToolDrawPatch()
    {
        this.Target = this.RequireMethod<Tool>("draw");
    }

    #region harmony patches

    /// <summary>Hide affected tiles overlay.</summary>
    [HarmonyPrefix]
    private static bool ToolDrawPrefix()
    {
        return !ModEntry.Config.Tools.HideAffectedTiles;
    }

    #endregion harmony patches
}
