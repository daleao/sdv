namespace DaLion.Professions.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectMaximumStackSizePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectMaximumStackSizePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ObjectMaximumStackSizePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.maximumStackSize));
    }

    #region harmony patches

    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool ObjectMaximumStackSizePrefix(SObject __instance, ref int __result)
    {
        if (__instance.ItemId == SurveyFlagId)
        {
            __result = 1;
            return false; // don't run original logic
        }

        return true; // run original logic
    }

    #endregion harmony patches
}
