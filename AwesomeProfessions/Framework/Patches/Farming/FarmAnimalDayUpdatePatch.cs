using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches;

[UsedImplicitly]
internal class FarmAnimalDayUpdatePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FarmAnimalDayUpdatePatch()
    {
        Original = RequireMethod<FarmAnimal>(nameof(FarmAnimal.dayUpdate));
    }

    #region harmony patches

    /// <summary>
    ///     Patch for Producer to double produce frequency at max animal happiness + remove Shepherd and Coopmaster hidden
    ///     produce quality boosts.
    /// </summary>
    [HarmonyTranspiler]
    protected static IEnumerable<CodeInstruction> FarmAnimalDayUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: FarmeAnimal.daysToLay -= (FarmAnimal.type.Value.Equals("Sheep") && Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(Farmer.shepherd)) ? 1 : 0
        /// To: FarmAnimal.daysToLay /= (FarmAnimal.happiness.Value >= 200 && Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(<producer_id>))
        ///		? Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(100 + <producer_id>)) ? 3 : 2
        ///		: 1

        var notPrestigedProducer = iLGenerator.DefineLabel();
        var resumeExecution1 = iLGenerator.DefineLabel();
        try
        {
            helper
                .FindFirst( // find index of FarmAnimal.type.Value.Equals("Sheep")
                    new CodeInstruction(OpCodes.Ldstr, "Sheep"),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(string).MethodNamed(nameof(string.Equals), new[] {typeof(string)}))
                )
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Ldarg_0)
                )
                .Insert(
                    new CodeInstruction(OpCodes.Conv_R8)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldfld, typeof(FarmAnimal).Field(nameof(FarmAnimal.type)))
                )
                .SetOperand(typeof(FarmAnimal).Field(nameof(FarmAnimal.happiness))) // was FarmAnimal.type
                .Advance()
                .SetOperand(typeof(NetFieldBase<byte, NetByte>)
                    .PropertyGetter(nameof(NetFieldBase<byte, NetByte>.Value))) // was <string, NetString>
                .Advance()
                .ReplaceWith(
                    new(OpCodes.Ldc_I4_S, 200) // was Ldstr "Sheep"
                )
                .Advance()
                .Remove()
                .SetOpCode(OpCodes.Blt_S) // was Brfalse_S
                .Advance()
                .ToBufferUntil(
                    stripLabels: false,
                    advance: true,
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(NetList<int, NetInt>).MethodNamed(nameof(NetList<int, NetInt>.Contains)))
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_0)
                )
                .ReplaceWith(
                    new CodeInstruction(OpCodes.Ldc_R8, 1.0),
                    preserveLabels: true
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_1)
                )
                .ReplaceWith(
                    new CodeInstruction(OpCodes.Ldc_R8, 2.0),
                    preserveLabels: true
                )
                .AddLabels(notPrestigedProducer)
                .InsertBuffer()
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_3)
                )
                .ReplaceWith(
                    new CodeInstruction(OpCodes.Ldc_I4_S, 100 + Utility.Professions.IndexOf("Producer"))
                )
                .Return()
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, notPrestigedProducer),
                    new CodeInstruction(OpCodes.Ldc_R8, 3.0),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution1)
                )
                .Advance()
                .SetOpCode(OpCodes.Div) // was Sub
                .AddLabels(resumeExecution1)
                .Advance()
                .Insert(
                    new CodeInstruction(OpCodes.Call,
                        typeof(Math).MethodNamed(nameof(Math.Round), new[] {typeof(double)})),
                    new CodeInstruction(OpCodes.Conv_U1)
                );
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed while patching modded Producer produce frequency.\nHelper returned {ex}",
                LogLevel.Error);
            return null;
        }

        /// Skipped: if ((!isCoopDweller() && Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(<shepherd_id>)) || (isCoopDweller() && Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(<coopmaster_id>))) chanceForQuality += 0.33

        try
        {
            helper
                .FindNext( // find index of first FarmAnimal.isCoopDweller check
                    new CodeInstruction(OpCodes.Call,
                        typeof(FarmAnimal).MethodNamed(nameof(FarmAnimal.isCoopDweller)))
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brfalse_S) // the all cases false branch
                )
                .GetOperand(out var resumeExecution2) // copy destination
                .Return()
                .Retreat()
                .Insert( // insert unconditional branch to skip this whole section
                    new CodeInstruction(OpCodes.Br_S, (Label) resumeExecution2)
                );
        }
        catch (Exception ex)
        {
            ModEntry.Log(
                $"Failed while removing vanilla Coopmaster + Shepherd produce quality bonuses.\nHelper returned {ex}",
                LogLevel.Error);
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}