namespace DaLion.Overhaul.Modules.Core.Debug;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class DebugPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="DebugPatcher"/> class.</summary>
    internal DebugPatcher()
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.placementAction));
    }

    #region harmony patches

    [HarmonyPrefix]
    private static void DebugPrefix()
    {
        Log.A("Debug prefix called!");
    }

    [HarmonyPostfix]
    private static void DebugPostfix()
    {
        Log.A("Debug postfix called!");
    }

    #endregion harmony patches
}
