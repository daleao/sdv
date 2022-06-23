namespace DaLion.Stardew.Professions.Framework.Patches.Common;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;

using DaLion.Common;
using DaLion.Common.Data;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class CropHarvestPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal CropHarvestPatch()
    {
        Target = RequireMethod<Crop>(nameof(Crop.harvest));
    }

    #region harmony patches

    /// <summary>
    ///     Patch to nerf Ecologist spring onion quality and increment forage counter + always allow iridium-quality crops
    ///     for Agriculturist + Harvester bonus crop yield.
    /// </summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> CropHarvestTranspiler(IEnumerable<CodeInstruction> instructions,
        ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

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
                        typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.GetEcologistForageQuality)))
                )
                .Insert(
                    new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching modded Ecologist spring onion quality.\nHelper returned {ex}");
            return null;
        }

        /// Injected: if (Game1.player.professions.Contains(<ecologist_id>))
        ///		Game1.player.IncrementField("EcologistItemsForaged", amount: @object.Stack)
        ///	After: Game1.stats.ItemsForaged += @object.Stack;

        // this particular method is too edgy for Harmony's AccessTools, so we use some old-fashioned reflection trickery to find this particular overload of FarmerExtensions.IncrementData<T>
        var mi = typeof(ModDataIO)
                     .GetMethods()
                     .FirstOrDefault(mi => mi.Name.Contains("IncrementData") && mi.GetParameters().Length == 3)?
                     .MakeGenericMethod(typeof(uint)) ?? throw new MissingMethodException("Increment method not found.");

        var dontIncreaseEcologistCounter = generator.DefineLabel();
        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(Stats).RequirePropertySetter(nameof(Stats.ItemsForaged)))
                )
                .Advance()
                .AddLabels(dontIncreaseEcologistCounter)
                .InsertProfessionCheck(Profession.Ecologist.Value)
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, dontIncreaseEcologistCounter),
                    new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                    new CodeInstruction(OpCodes.Ldc_I4_0), // DataField.EcologistItemsForaged
                    new CodeInstruction(OpCodes.Ldloc_1), // loc 1 = @object
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(Item).RequirePropertyGetter(nameof(Item.Stack))),
                    new CodeInstruction(OpCodes.Call, mi)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding Ecologist counter increment.\nHelper returned {ex}");
            return null;
        }

        /// From: if (fertilizerQualityLevel >= 3 && random2.NextDouble() < chanceForGoldQuality / 2.0)
        /// To: if (Game1.player.professions.Contains(<agriculturist_id>) || fertilizerQualityLevel >= 3) && random2.NextDouble() < chanceForGoldQuality / 2.0)

        var fertilizerQualityLevel = helper.Locals[8];
        var random2 = helper.Locals[9];
        var isAgriculturist = generator.DefineLabel();
        try
        {
            helper.AdvanceUntil( // find index of Crop.fertilizerQualityLevel >= 3
                    new CodeInstruction(OpCodes.Ldloc_S, fertilizerQualityLevel),
                    new CodeInstruction(OpCodes.Ldc_I4_3),
                    new CodeInstruction(OpCodes.Blt_S)
                )
                .InsertProfessionCheck(Profession.Agriculturist.Value)
                .Insert(
                    new CodeInstruction(OpCodes.Brtrue_S, isAgriculturist)
                )
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

        /// Injected: if ((junimoHarvester is null || ModEntry.ModHelper.ModRegistry.IsLoaded("BetterJunimos")) && Game1.player.professions.Contains(<harvester_id>) &&
        ///     r.NextDouble() < 0.1 + (Game1.player.professions.Contains(100 + <harverster_id>) ? 0.1 : 0))
        ///		numToHarvest++

        var numToHarvest = helper.Locals[6];
        var continueToHarvesterCheck = generator.DefineLabel();
        var dontIncreaseNumToHarvest = generator.DefineLabel();
        var isNotPrestiged = generator.DefineLabel();
        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Ldloc_S, numToHarvest) // find index of numToHarvest++
                )
                .GetInstructionsUntil( // copy this segment
                    out var got,
                    true,
                    false,
                    new CodeInstruction(OpCodes.Stloc_S, numToHarvest)
                )
                .AdvanceUntil( // find end of chanceForExtraCrops while loop
                    new CodeInstruction(OpCodes.Ldfld,
                        typeof(Crop).RequireField(nameof(Crop.chanceForExtraCrops)))
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldarg_0) // beginning of the next segment
                )
                .StripLabels(out var labels) // copy existing labels
                .AddLabels(dontIncreaseNumToHarvest) // branch here if shouldn't apply Harvester bonus
                .InsertWithLabels( // insert check if junimoHarvester is null
                    labels,
                    new CodeInstruction(OpCodes.Ldarg_S, (byte) 4), // arg 4 = bool junimoHarvester
                    new CodeInstruction(OpCodes.Brfalse_S, continueToHarvesterCheck),
                    new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.ModHelper))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(IModHelper).RequirePropertyGetter(nameof(IModHelper.ModRegistry))),
                    new CodeInstruction(OpCodes.Ldstr, "hawkfalcon.BetterJunimos"),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(IModRegistry).RequireMethod(nameof(IModRegistry.IsLoaded))),
                    new CodeInstruction(OpCodes.Brfalse_S, dontIncreaseNumToHarvest)
                )
                .InsertProfessionCheck(Profession.Harvester.Value, new[] {continueToHarvesterCheck})
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, dontIncreaseNumToHarvest),
                    new CodeInstruction(OpCodes.Ldloc_S, random2)
                )
                .InsertDiceRoll(0.1, forStaticRandom: false)
                .InsertProfessionCheck(Profession.Harvester.Value + 100)
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged),
                    // double chance if prestiged
                    new CodeInstruction(OpCodes.Ldc_R8, 0.1),
                    new CodeInstruction(OpCodes.Add)
                )
                .InsertWithLabels(
                    new[] {isNotPrestiged},
                    new CodeInstruction(OpCodes.Bge_Un_S, dontIncreaseNumToHarvest)
                )
                .Insert(got); // insert numToHarvest++
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