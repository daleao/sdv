namespace DaLion.Professions.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Characters;
using StardewValley.Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class CropHarvestPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CropHarvestPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal CropHarvestPatcher(Harmonizer harmonizer)
        : base(harmonizer)
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
        var helper = new ILHelper(original, instructions);

        // From: obj.Quality = 4
        // To: obj.Quality = GetEcologistForageQuality()
        try
        {
            helper
                .MatchProfessionCheck(Farmer.botanist) // find index of botanist check
                .PatternMatch([new CodeInstruction(OpCodes.Ldc_I4_4)]) // start of obj.Quality = 4
                .ReplaceWith(
                    // replace with custom quality
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.GetEcologistForageQuality))))
                .Insert([
                    // set edibility
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                    new CodeInstruction(OpCodes.Dup), // prepare to set quality
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(OpCodes.Call, typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.ApplyEcologistEdibility))),
                    // append to items foraged
                    new CodeInstruction(OpCodes.Call, typeof(ProfessionsMod).RequirePropertyGetter(nameof(Data))),
                    new CodeInstruction(OpCodes.Ldstr, "399"),
                    new CodeInstruction(OpCodes.Ldnull),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ModDataManagerExtensions).RequireMethod(
                            nameof(ModDataManagerExtensions
                            .AppendToEcologistItemsForaged))),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching modded Ecologist spring onion quality.\nHelper returned {ex}");
            return null;
        }

        // Injected: or (Game1.player.professions.Contains(<agriculturist_id>) && random2.NextDouble() < chanceForGoldQuality / 3.0)
        // After: if (fertilizerQualityLevel >= 3 && random2.NextDouble() < chanceForGoldQuality / 2.0)
        try
        {
            var checkForAgriculturist = generator.DefineLabel();
            var setIridiumQuality = generator.DefineLabel();
            helper
                .PatternMatch([
                    // find index of Crop.fertilizerQualityLevel >= 3
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[10]),
                    new CodeInstruction(OpCodes.Ldc_I4_3),
                    new CodeInstruction(OpCodes.Blt_S),
                ])
                .Move(2)
                .GetOperand(out var checkForGoldQuality) // this is the label for the failed iridium check
                .SetOperand(checkForAgriculturist) // if failed, try the OR
                .PatternMatch([new CodeInstruction(OpCodes.Bge_Un_S)]) // advance until the end of random2.NextDouble() < chanceForGoldQuality / 2.0
                .ReplaceWith(new CodeInstruction(OpCodes.Blt_S, setIridiumQuality)) // replace AND with OR
                .Move()
                .AddLabels(setIridiumQuality) // this is the destination for a successful iridium check
                .InsertProfessionCheck(Farmer.agriculturist, [checkForAgriculturist])
                .Insert([
                    new CodeInstruction(OpCodes.Brfalse_S, checkForGoldQuality),
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[9]),
                    new CodeInstruction(OpCodes.Callvirt, typeof(Random).RequireMethod(nameof(Random.NextDouble))),
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[11]),
                    new CodeInstruction(OpCodes.Ldc_R8, 3d),
                    new CodeInstruction(OpCodes.Div),
                    new CodeInstruction(OpCodes.Bge_Un_S, checkForGoldQuality),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding modded Agriculturist crop harvest quality.\nHelper returned {ex}");
            return null;
        }

        // Injected: if (ShouldIncreaseHarvestYield(junimoHarvester, random2) numToHarvest++;
        // After: ExtraHarvestChance loop;
        try
        {
            var numToHarvest = helper.Locals[14];
            var dontIncreaseNumToHarvest = generator.DefineLabel();
            helper
                .PatternMatch([
                    // find index of numToHarvest++
                    new CodeInstruction(OpCodes.Ldloc_S, numToHarvest),
                ])
                .PatternMatch([new CodeInstruction(OpCodes.Ldarg_0)]) // beginning of the next segment
                .StripLabels(out var labels) // copy existing labels
                .AddLabels(dontIncreaseNumToHarvest) // branch here if shouldn't apply Harvester bonus
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)4), // arg 4 = JunimoHarvester junimoHarvester
                        new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[9]), // local 9 = Random r
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(CropHarvestPatcher).RequireMethod(nameof(ShouldIncreaseHarvestYield))),
                        new CodeInstruction(OpCodes.Brfalse_S, dontIncreaseNumToHarvest),
                        new CodeInstruction(OpCodes.Ldloc_S, numToHarvest),
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Add),
                        new CodeInstruction(OpCodes.Stloc_S, numToHarvest),
                    ],
                    labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding modded Harvester extra crop yield.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injections

    private static bool ShouldIncreaseHarvestYield(JunimoHarvester? junimoHarvester, Random r)
    {
        var harvester = junimoHarvester is null ? Game1.player :
            Config.ShouldJunimosInheritProfessions ? junimoHarvester.GetOwner() : null;
        if (harvester?.HasProfession(Profession.Harvester) != true)
        {
            return false;
        }

        var chance = harvester.HasProfession(Profession.Harvester, true) ? 0.2 : 0.1;
        return r.NextBool(chance);
    }

    #endregion injections
}
