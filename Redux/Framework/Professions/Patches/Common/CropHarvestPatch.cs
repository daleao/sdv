namespace DaLion.Redux.Framework.Professions.Patches.Common;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Redux.Framework.Professions.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Characters;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class CropHarvestPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="CropHarvestPatch"/> class.</summary>
    internal CropHarvestPatch()
    {
        this.Target = this.RequireMethod<Crop>(nameof(Crop.harvest));
    }

    #region harmony patches

    /// <summary>
    ///     Patch to nerf Ecologist spring onion quality and increment forage counter + always allow iridium-quality crops
    ///     for Agriculturist + Harvester bonus crop yield.
    /// </summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? CropHarvestTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // From: obj.Quality = 4
        // To: obj.Quality = GetEcologistForageQuality()
        try
        {
            helper
                .FindProfessionCheck(Farmer.botanist) // find index of botanist check
                .AdvanceUntil(new CodeInstruction(OpCodes.Ldc_I4_4)) // start of obj.Quality = 4
                .ReplaceInstructionWith(
                    // replace with custom quality
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.GetEcologistForageQuality))))
                .InsertInstructions(
                    new CodeInstruction(
                    OpCodes.Call,
                    typeof(Game1).RequirePropertyGetter(nameof(Game1.player))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching modded Ecologist spring onion quality.\nHelper returned {ex}");
            return null;
        }

        // Injected: if (Game1.player.professions.Contains(<ecologist_id>))
        //     Game1.player.Increment(DataFields.EcologistItemsForaged, amount: obj.Stack)
        // After: Game1.stats.ItemsForaged += obj.Stack;
        // Note: this particular method is too edgy for Harmony's AccessTools, so we use some old-fashioned reflection trickery to find this particular overload of FarmerExtensions.IncrementData<T>
        try
        {
            var incrementMethod = typeof(Shared.Extensions.Stardew.FarmerExtensions)
                                      .GetMethods()
                                      .FirstOrDefault(mi =>
                                          mi.Name.Contains(nameof(Shared.Extensions.Stardew.FarmerExtensions.Increment)) && mi.GetGenericArguments().Length > 0)?
                                      .MakeGenericMethod(typeof(uint)) ??
                                  ThrowHelper.ThrowMissingMethodException<MethodInfo>("Increment method not found.");

            var dontIncreaseEcologistCounter = generator.DefineLabel();
            helper
                .FindNext(
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Stats).RequirePropertySetter(nameof(Stats.ItemsForaged))))
                .Advance()
                .AddLabels(dontIncreaseEcologistCounter)
                .InsertProfessionCheck(Profession.Ecologist.Value)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Brfalse_S, dontIncreaseEcologistCounter),
                    new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                    new CodeInstruction(OpCodes.Ldstr, DataFields.EcologistItemsForaged),
                    new CodeInstruction(OpCodes.Ldloc_1), // loc 1 = obj
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Item).RequirePropertyGetter(nameof(Item.Stack))),
                    new CodeInstruction(OpCodes.Call, incrementMethod));
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding Ecologist counter increment.\nHelper returned {ex}");
            return null;
        }

        // From: if (fertilizerQualityLevel >= 3 && random2.NextDouble() < chanceForGoldQuality / 2.0)
        // To: if (Game1.player.professions.Contains(<agriculturist_id>) || fertilizerQualityLevel >= 3) && random2.NextDouble() < chanceForGoldQuality / 2.0)
        var random2 = helper.Locals[9];
        try
        {
            var fertilizerQualityLevel = helper.Locals[8];
            var isAgriculturist = generator.DefineLabel();
            helper.AdvanceUntil(
                    // find index of Crop.fertilizerQualityLevel >= 3
                    new CodeInstruction(OpCodes.Ldloc_S, fertilizerQualityLevel),
                    new CodeInstruction(OpCodes.Ldc_I4_3),
                    new CodeInstruction(OpCodes.Blt_S))
                .InsertProfessionCheck(Profession.Agriculturist.Value)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Brtrue_S, isAgriculturist))
                .AdvanceUntil(// find start of dice roll
                    new CodeInstruction(OpCodes.Ldloc_S, random2))
                .AddLabels(isAgriculturist); // branch here if player is agriculturist
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding modded Agriculturist crop harvest quality.\nHelper returned {ex}");
            return null;
        }

        // Injected: if (ShouldIncreaseHarvestYield(junimoHarvester, random2) numToHarvest++;
        // After: numToHarvest++;
        try
        {
            var numToHarvest = helper.Locals[6];
            var dontIncreaseNumToHarvest = generator.DefineLabel();
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Ldloc_S, numToHarvest)) // find index of numToHarvest++
                .GetInstructionsUntil(
                    out var got,
                    true,
                    false,
                    new CodeInstruction(OpCodes.Stloc_S, numToHarvest))
                .AdvanceUntil(// find end of chanceForExtraCrops while loop
                    new CodeInstruction(
                        OpCodes.Ldfld,
                        typeof(Crop).RequireField(nameof(Crop.chanceForExtraCrops))))
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldarg_0)) // beginning of the next segment
                .StripLabels(out var labels) // copy existing labels
                .AddLabels(dontIncreaseNumToHarvest) // branch here if shouldn't apply Harvester bonus
                .InsertWithLabels(
                    labels,
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)4), // arg 4 = JunimoHarvester junimoHarvester
                    new CodeInstruction(OpCodes.Ldloc_S, random2),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(CropHarvestPatch).RequireMethod(nameof(ShouldIncreaseHarvestYield))),
                    new CodeInstruction(OpCodes.Brfalse_S, dontIncreaseNumToHarvest))
                .InsertInstructions(got); // insert numToHarvest++
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding modded Harvester extra crop yield.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static bool ShouldIncreaseHarvestYield(JunimoHarvester? junimoHarvester, Random r)
    {
        var harvester = junimoHarvester is null ? Game1.player :
            ModEntry.Config.Professions.ShouldJunimosInheritProfessions ? junimoHarvester.GetOwner() : null;
        return harvester?.HasProfession(Profession.Harvester) == true &&
               r.NextDouble() < (harvester.HasProfession(Profession.Harvester, true) ? 0.2 : 0.1);
    }

    #endregion injected subroutines
}
