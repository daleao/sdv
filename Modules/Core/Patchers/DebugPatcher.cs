namespace DaLion.Overhaul.Modules.Core.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class DebugPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="DebugPatcher"/> class.</summary>
    internal DebugPatcher()
    {
    }

    /// <inheritdoc />
    protected override bool ApplyImpl(Harmony harmony)
    {
        return this.Target is null || base.ApplyImpl(harmony);
    }

    /// <inheritdoc />
    protected override bool UnapplyImpl(Harmony harmony)
    {
        return this.Target is null || base.UnapplyImpl(harmony);
    }

    #region harmony patches

    [HarmonyPrefix]
    private static void DebugPrefix(object __instance, object __result)
    {
        Log.D("Debug prefix called!");
    }

    [HarmonyPostfix]
    private static void DebugPostfix(object __instance, object __result)
    {
        Log.D("Debug postfix called!");
    }

    #endregion harmony patches
}
