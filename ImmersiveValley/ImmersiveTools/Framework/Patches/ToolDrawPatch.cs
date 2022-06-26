namespace DaLion.Stardew.Tools.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolDrawPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal ToolDrawPatch()
    {
        Target = RequireMethod<Tool>("draw");
    }

    #region harmony patches

    /// <summary>Hide affected tiles overlay.</summary>
    [HarmonyPrefix]
    private static bool ToolDrawPrefix()
    {
        return !ModEntry.Config.HideAffectedTiles;
    }

    #endregion harmony patches
}