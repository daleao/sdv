namespace DaLion.Professions.Framework.Patchers;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectCheckForActionOnMachinePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectCheckForActionOnMachinePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ObjectCheckForActionOnMachinePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<SObject>("CheckForActionOnMachine");
    }

    #region harmony patches

    /// <summary>Implements Machinist's leftover material recovery.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? ObjectCheckForActionOnMachineTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var dontReturn = generator.DefineLabel();
            helper
                .PatternMatch([new CodeInstruction(OpCodes.Call, typeof(MachineDataUtility).RequireMethod(nameof(MachineDataUtility.UpdateStats)))])
                .Move()
                .AddLabels(dontReturn)
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(OpCodes.Ldloc_2),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ObjectCheckForActionOnMachinePatcher).RequireMethod(nameof(CheckForExtraMaterial))),
                        new CodeInstruction(OpCodes.Brfalse_S, dontReturn),
                        new CodeInstruction(OpCodes.Ldc_I4_1), // early return true
                        new CodeInstruction(OpCodes.Ret),
                    ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting Machinist's leftover material pickup.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected

    private static bool CheckForExtraMaterial(SObject machine, Farmer who, SObject objectThatWasHeld)
    {
        if (!machine.IsArtisanMachine() || !who.HasProfession(Profession.Artisan))
        {
            return false;
        }

        var repeatedCycles = Data.ReadAs<int>(machine, DataKeys.RepeatedInputCycles);
        var lastLeftoverCycle = Data.ReadAs<int>(machine, DataKeys.LastLeftoverCycle);
        const int maxCalibration = 25;
        if (repeatedCycles <= maxCalibration || (repeatedCycles - maxCalibration) % 5 != 0 || lastLeftoverCycle == repeatedCycles)
        {
            return false;
        }

        var extraOutput = objectThatWasHeld.getOne();
        extraOutput.Quality = SObject.lowQuality;
        machine.heldObject.Value = (SObject)extraOutput;
        Data.Write(machine, DataKeys.LastLeftoverCycle, repeatedCycles.ToString());
        return true;
    }

    #endregion injected
}
