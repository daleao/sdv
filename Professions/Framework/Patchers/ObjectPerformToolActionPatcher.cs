namespace DaLion.Professions.Framework.Patchers;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectPerformToolActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectPerformToolActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ObjectPerformToolActionPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.performToolAction));
    }

    #region harmony patches

    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool ObjectPerformToolActionPrefix(SObject __instance, ref bool __state, Tool t)
    {
        if (__instance.ItemId != SurveyFlagId || State.SpelunkerFlag is null)
        {
            return true; // run original logic
        }

        if (t.getLastFarmerToUse() is not Farmer lastUser || !__instance.IsOwnedBy(lastUser))
        {
            return false; // don't run original logic
        }

        if (__instance.Location.Objects.ContainsKey(__instance.TileLocation))
        {
            __state = true;
        }

        return true; // run original logic
    }

    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ObjectPerformToolActionPostfix(SObject __instance, bool __state)
    {
        if (__instance.ItemId == SurveyFlagId && State.SpelunkerFlag is not null && __state &&
            !__instance.Location.Objects.ContainsKey(__instance.TileLocation))
        {
            State.SpelunkerFlag = null;
            State.SpelunkerFlagLevel = 0;
        }
    }

    #endregion harmony patches
}
