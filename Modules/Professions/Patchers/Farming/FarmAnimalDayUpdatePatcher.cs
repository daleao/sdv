namespace DaLion.Overhaul.Modules.Professions.Patchers.Farming;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Netcode;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmAnimalDayUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmAnimalDayUpdatePatcher"/> class.</summary>
    internal FarmAnimalDayUpdatePatcher()
    {
        this.Target = this.RequireMethod<FarmAnimal>(nameof(FarmAnimal.dayUpdate));
    }

    #region harmony patches

    /// <summary>
    ///     Patch for Producer to double produce frequency at max animal happiness + remove Shepherd and Coopmaster hidden
    ///     produce quality boosts.
    /// </summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FarmAnimalDayUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: FarmAnimal.daysToLay -= (FarmAnimal.type.Value.Equals("Sheep") && Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(Farmer.shepherd)) ? 1 : 0
        // To: FarmAnimal.daysToLay /= (FarmAnimal.happiness.Value >= 200 && Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(<producer_id>))
        //		? Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(100 + <producer_id>)) ? 3 : 2
        //		: 1
        try
        {
            var notPrestigedProducer = generator.DefineLabel();
            var resumeExecution1 = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        // find index of FarmAnimal.type.Value.Equals("Sheep")
                        new CodeInstruction(OpCodes.Ldstr, "Sheep"),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(string).RequireMethod(nameof(string.Equals), new[] { typeof(string) })),
                    })
                .Match(new[] { new CodeInstruction(OpCodes.Ldarg_0) }, ILHelper.SearchOption.Previous)
                .Insert(new[] { new CodeInstruction(OpCodes.Conv_R8) })
                .Match(new[]
                {
                    new CodeInstruction(OpCodes.Ldfld, typeof(FarmAnimal)
                        .RequireField(nameof(FarmAnimal.type))),
                })
                .SetOperand(typeof(FarmAnimal).RequireField(nameof(FarmAnimal.happiness))) // was FarmAnimal.type
                .Move()
                .SetOperand(typeof(NetFieldBase<byte, NetByte>)
                    .RequirePropertyGetter(nameof(NetFieldBase<byte, NetByte>.Value))) // was <string, NetString>
                .Move()
                .ReplaceWith(new CodeInstruction(OpCodes.Ldc_I4_S, 200)) // was Ldstr "Sheep"
                .Move()
                .Remove()
                .SetOpCode(OpCodes.Blt_S) // was Brfalse_S
                .Move()
                .CountUntil(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(NetList<int, NetInt>).RequireMethod(nameof(NetList<int, NetInt>.Contains))),
                    },
                    out var steps)
                .Copy(
                    out var copy,
                    steps,
                    false,
                    true)
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4_0) })
                .ReplaceWith(new CodeInstruction(OpCodes.Ldc_R8, 1.0), true)
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4_1) })
                .ReplaceWith(new CodeInstruction(OpCodes.Ldc_R8, 1.75), true)
                .AddLabels(notPrestigedProducer)
                .Insert(copy)
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4_3) }, ILHelper.SearchOption.Previous)
                .ReplaceWith(new CodeInstruction(OpCodes.Ldc_I4_S, VanillaProfession.Producer.Value + 100))
                .Return()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Brfalse_S, notPrestigedProducer),
                        new CodeInstruction(OpCodes.Ldc_R8, 3.0),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution1),
                    })
                .Move()
                .SetOpCode(OpCodes.Div) // was Sub
                .AddLabels(resumeExecution1)
                .Move()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Math).RequireMethod(nameof(Math.Round), new[] { typeof(double) })),
                        new CodeInstruction(OpCodes.Conv_U1),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching modded Producer produce frequency.\nHelper returned {ex}");
            return null;
        }

        // Skipped: if ((!isCoopDweller() && Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(<shepherd_id>)) || (isCoopDweller() && Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(<coopmaster_id>))) chanceForQuality += 0.33
        try
        {
            helper
                .Match(
                    new[]
                    {
                        // find index of first FarmAnimal.isCoopDweller check
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(FarmAnimal).RequireMethod(nameof(FarmAnimal.isCoopDweller))),
                    })
                .Match(new[] { new CodeInstruction(OpCodes.Brfalse_S) }) // the all cases false branch
                .GetOperand(out var resumeExecution2) // copy destination
                .Return()
                .Move(-1)
                .Insert(
                    new[]
                    {
                        // insert unconditional branch to skip this whole section
                        new CodeInstruction(OpCodes.Br_S, (Label)resumeExecution2),
                    });
        }
        catch (Exception ex)
        {
            Log.E(
                $"Failed removing vanilla Coopmaster + Shepherd produce quality bonuses.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
