namespace DaLion.Harmonics.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CombinedRingLoadDisplayFieldsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CombinedRingLoadDisplayFieldsPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal CombinedRingLoadDisplayFieldsPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<CombinedRing>("loadDisplayFields");
        this.Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Adjust Infinity Band description.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool CombinedRingsLoadDisplayFieldsPrefix(CombinedRing __instance, ref bool __result)
    {
        if (__instance.QualifiedItemId != $"(O){InfinityBandId}")
        {
            return true; // don't run original logic
        }

        var data = Game1.objectData[InfinityBandId];
        __instance.displayName = data.DisplayName;
        __instance.description = data.Description;
        __result = true;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
