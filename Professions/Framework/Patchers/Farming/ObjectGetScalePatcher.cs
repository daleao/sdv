namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectGetScalePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectGetScalePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages __instance patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ObjectGetScalePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.getScale));
    }

    #region harmony patches

    /// <summary>Patch for Machinist overclock treatment visual.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? ObjectGetScaleTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .ForEach(
                    [
                        new CodeInstruction(OpCodes.Ldc_R4, 0.1f),
                    ],
                    _ => helper
                        .Insert([
                            new CodeInstruction(OpCodes.Ldarg_0),
                        ])
                        .ReplaceWith(new CodeInstruction(OpCodes.Call, typeof(ObjectGetScalePatcher).RequireMethod(nameof(GetWobbleSpeed))))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed settings overclocked machine wobble speed.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected

    private static float GetWobbleSpeed(SObject machine)
    {
        return Data.ReadAppliedMachineTreatments(machine).OverclockCycles > 0 ? 0.2f : 0.1f;
    }

    #endregion injected
}
