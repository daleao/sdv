namespace DaLion.Stardew.Professions.Framework.Patches.Common;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Netcode;
using StardewValley;

using Stardew.Common.Harmony;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal class CropHarvestPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal CropHarvestPatch()
    {
        Original = RequireMethod<Crop>(nameof(Crop.harvest));
    }

    #region harmony patches

    /// <summary>
    ///     Patch to nerf Ecologist spring onion quality and increment forage counter + always allow iridium-quality crops
    ///     for Agriculturist + Harvester bonus crop yield.
    /// </summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> CropHarvestTranspiler(IEnumerable<CodeInstruction> instructions,
        ILGenerator iLGenerator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        var mb = original.GetMethodBody() ??
                 throw new ArgumentNullException($"{original.Name} method body returned null.");

        /// From: @object.Quality = 4
        /// To: @object.Quality = GetEcologistForageQuality()

        try
        {
            helper
                .FindProfessionCheck(Farmer.botanist) // find index of botanist check
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_4) // start of @object.Quality = 4
                )
                .ReplaceWith( // replace with custom quality
                    new(OpCodes.Call,
                        typeof(FarmerExtensions).MethodNamed(
                            nameof(FarmerExtensions.GetEcologistForageQuality)))
                )
                .Insert(
                    new CodeInstruction(OpCodes.Call, typeof(Game1).PropertyGetter(nameof(Game1.player)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching modded Ecologist spring onion quality.\nHelper returned {ex}");
            return null;
        }

        /// Injected: if (Game1.player.professions.Contains(<ecologist_id>))
        ///		Data.IncrementField("EcologistItemsForaged", amount: @object.Stack)
        ///	After: Game1.stats.ItemsForaged += @object.Stack;

        // this particular method is too edgy for Harmony Access Tool, so we use some old-fashioned reflection trickery to find this particular overload of ModData.Increment<T>
        var mi = typeof(ModData)
                     .GetMember("Increment*", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static)
                     .Cast<MethodInfo>()
                     .FirstOrDefault(mi => mi.GetParameters().Length == 3) ??
                 throw new MissingMethodException("Increment method not found.");
        mi = mi.MakeGenericMethod(typeof(uint));

        var dontIncreaseEcologistCounter = iLGenerator.DefineLabel();
        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(Stats).PropertySetter(nameof(Stats.ItemsForaged)))
                )
                .Advance()
                .InsertProfessionCheckForLocalPlayer("Ecologist".ToProfessionIndex(),
                    dontIncreaseEcologistCounter)
                .Insert(
                    new CodeInstruction(OpCodes.Ldstr, DataField.EcologistItemsForaged.ToString()),
                    new CodeInstruction(OpCodes.Ldloc_1), // loc 1 = @object
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(Item).PropertyGetter(nameof(Item.Stack))),
                    new CodeInstruction(OpCodes.Ldnull),
                    new CodeInstruction(OpCodes.Call, mi)
                )
                .AddLabels(dontIncreaseEcologistCounter);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding Ecologist counter increment.\nHelper returned {ex}");
            return null;
        }

        /// From: if (fertilizerQualityLevel >= 3 && random2.NextDouble() < chanceForGoldQuality / 2.0)
        /// To: if (Game1.player.professions.Contains(<agriculturist_id>) || fertilizerQualityLevel >= 3) && random2.NextDouble() < chanceForGoldQuality / 2.0)

        var fertilizerQualityLevel = mb.LocalVariables[8];
        var random2 = mb.LocalVariables[9];
        var isAgriculturist = iLGenerator.DefineLabel();
        try
        {
            helper.AdvanceUntil( // find index of Crop.fertilizerQualityLevel >= 3
                    new CodeInstruction(OpCodes.Ldloc_S, fertilizerQualityLevel),
                    new CodeInstruction(OpCodes.Ldc_I4_3),
                    new CodeInstruction(OpCodes.Blt_S)
                )
                .InsertProfessionCheckForLocalPlayer("Agriculturist".ToProfessionIndex(), isAgriculturist,
                    true)
                .AdvanceUntil( // find start of dice roll
                    new CodeInstruction(OpCodes.Ldloc_S, random2)
                )
                .AddLabels(isAgriculturist); // branch here if player is agriculturist
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding modded Agriculturist crop harvest quality.\nHelper returned {ex}");
            return null;
        }

        /// Injected: if (junimoHarvester is null && Game1.player.professions.Contains(<harvester_id>) && r.NextDouble() <
        ///		Game1.player.professions.Contains(100 + <harverster_id>) ? 0.2 : 0.1)
        ///		numToHarvest++

        var numToHarvest = mb.LocalVariables[6];
        var dontIncreaseNumToHarvest = iLGenerator.DefineLabel();
        var dontDuplicateChance = iLGenerator.DefineLabel();
        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Ldloc_S, numToHarvest) // find index of numToHarvest++
                )
                .ToBufferUntil( // copy this segment
                    true,
                    false,
                    new CodeInstruction(OpCodes.Stloc_S, numToHarvest)
                )
                .FindNext(
                    new CodeInstruction(OpCodes.Ldloc_S, random2) // find an instance of accessing the rng
                )
                .GetOperand(out var r2) // copy operand object
                .FindLast( // find end of chanceForExtraCrops while loop
                    new CodeInstruction(OpCodes.Ldfld,
                        typeof(Crop).Field(nameof(Crop.chanceForExtraCrops)))
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldarg_0) // beginning of the next segment
                )
                .GetLabels(out var labels) // copy existing labels
                .SetLabels(dontIncreaseNumToHarvest) // branch here if shouldn't apply Harvester bonus
                .Insert( // insert check if junimoHarvester is null
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)4),
                    new CodeInstruction(OpCodes.Brtrue_S, dontIncreaseNumToHarvest)
                )
                .InsertProfessionCheckForLocalPlayer("Harvester".ToProfessionIndex(),
                    dontIncreaseNumToHarvest)
                .Insert( // insert dice roll
                    new CodeInstruction(OpCodes.Ldloc_S, r2),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(Random).MethodNamed(nameof(Random.NextDouble))),
                    new CodeInstruction(OpCodes.Ldc_R8, 0.1),
                    // double chance if prestiged
                    new CodeInstruction(OpCodes.Call, typeof(Game1).PropertyGetter(nameof(Game1.player))),
                    new CodeInstruction(OpCodes.Ldfld, typeof(Farmer).Field(nameof(Farmer.professions))),
                    new CodeInstruction(OpCodes.Ldc_I4_S, "Harvester".ToProfessionIndex() + 100),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(NetList<int, NetInt>).MethodNamed(nameof(NetList<int, NetInt>.Contains))),
                    new CodeInstruction(OpCodes.Brfalse_S, dontDuplicateChance),
                    new CodeInstruction(OpCodes.Ldc_R8, 0.1),
                    new CodeInstruction(OpCodes.Add)
                )
                .Insert(
                    new[] { dontDuplicateChance },
                    new CodeInstruction(OpCodes.Bge_Un_S, dontIncreaseNumToHarvest)
                )
                .InsertBuffer() // insert numToHarvest++
                .Return(3) // return to first inserted instruction
                .SetLabels(labels); // restore original labels to this segment
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding modded Harvester extra crop yield.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}