namespace DaLion.Core.Framework.Patchers;

using DaLion.Shared.Constants;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectMinutesElapsedPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectMinutesElapsedPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal ObjectMinutesElapsedPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.minutesElapsed));
    }

    #region harmony patches

    /// <summary>Patch to make Hopper actually useful.</summary>
    [HarmonyPostfix]
    private static void ObjectMinutesElapsedPostfix(SObject __instance)
    {
        if (__instance.QualifiedItemId != QualifiedBigCraftableIds.CheesePress)
        {
            return;
        }

        var machineData = __instance.GetMachineData();
        if (machineData is null)
        {
            return;
        }

        var shouldTimePass = __instance.ShouldTimePassForMachine();
    }

    #endregion harmony patches
}
