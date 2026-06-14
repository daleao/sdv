namespace DaLion.Professions.Framework.Patchers.Mining;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectShouldWobblePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectShouldWobblePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ObjectShouldWobblePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.ShouldWobble));
    }

    #region harmony patches

    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ObjectShouldWobblePostfix(SObject __instance, ref bool __result)
    {
        if (State.ProspectorHunt is null || !State.ProspectorHunt.IsActive ||
            !ReferenceEquals(State.ProspectorHunt.Location, __instance.Location) ||
            !ReferenceEquals(State.ProspectorHunt.TreasureStone, __instance))
        {
            return;
        }

        __result = true;
    }

    #endregion harmony patches
}
