namespace DaLion.Professions.Framework.Patchers.Foraging;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class UtilityPickFarmEventPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="UtilityPickFarmEventPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal UtilityPickFarmEventPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = typeof(Utility).GetInnerMethodsContaining("<pickFarmEvent>b__213_0").Single();
    }

    #region harmony patches

    /// <summary>Patch to increase fairy chance for Silviculturist.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? UtilityPickFarmEventTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var resumeExecution = generator.DefineLabel();
            helper
                .PatternMatch([new CodeInstruction(OpCodes.Stloc_1)])
                .StripLabels(out var labels)
                .AddLabels(resumeExecution)
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Ldsfld, typeof(Game1).RequireField(nameof(Game1.game1))),
                        new CodeInstruction(OpCodes.Ldsfld, typeof(Profession).RequireField(nameof(Profession.Arborist))),
                        new CodeInstruction(OpCodes.Ldc_I4_0), // includeOffline = false
                        new CodeInstruction(OpCodes.Ldc_I4_1), // prestiged = true
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Game1Extensions).RequireMethod(
                                nameof(Game1Extensions.DoesAnyPlayerHaveProfession),
                                [typeof(Game1), typeof(IProfession), typeof(bool), typeof(bool)])),
                        new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                        new CodeInstruction(OpCodes.Ldc_R8, 0.01),
                        new CodeInstruction(OpCodes.Add),
                    ],
                    labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed doubling fairy chance.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
