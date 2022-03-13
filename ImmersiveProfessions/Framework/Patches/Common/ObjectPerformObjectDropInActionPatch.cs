namespace DaLion.Stardew.Professions.Framework.Patches.Common;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;

using Stardew.Common.Extensions;
using Stardew.Common.Harmony;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal class ObjectPerformObjectDropInActionPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal ObjectPerformObjectDropInActionPatch()
    {
        Original = RequireMethod<SObject>(nameof(SObject.performObjectDropInAction));
    }

    #region harmony patches

    /// <summary>Patch to remember initial machine state.</summary>
    [HarmonyPrefix]
    // ReSharper disable once RedundantAssignment
    private static bool ObjectPerformObjectDropInActionPrefix(SObject __instance, ref bool __state)
    {
        __state = __instance.heldObject.Value !=
                  null; // remember whether this machine was already holding an object
        return true; // run original logic
    }

    /// <summary>
    ///     Patch to increase Gemologist mineral quality from Geode Crusher + speed up Artisan production
    ///     speed + integrate Quality Artisan Products.
    /// </summary>
    [HarmonyPostfix]
    private static void ObjectPerformObjectDropInActionPostfix(SObject __instance, bool __state, Item dropInItem,
        bool probe, Farmer who)
    {
        // if there was an object inside before running the original method, or if the machine is still empty after running the original method, or the machine doesn't belong to the farmer, then do nothing
        if (__state || __instance.heldObject.Value is null || probe || Context.IsMultiplayer && __instance.owner.Value != who.UniqueMultiplayerID) return;

        if (__instance.name == "Geode Crusher" && who.HasProfession(Profession.Gemologist) &&
            (__instance.heldObject.Value.IsForagedMineral() || __instance.heldObject.Value.IsGemOrMineral()))
        {
            __instance.heldObject.Value.Quality = who.GetGemologistMineralQuality();
        }
        else if (__instance.IsArtisanMachine() && who.HasProfession(Profession.Artisan) && dropInItem is SObject dropIn)
        {
            // produce cares about input quality with low chance for upgrade
            __instance.heldObject.Value.Quality = dropIn.Quality;
            if (dropIn.Quality < SObject.bestQuality &&
                new Random(Guid.NewGuid().GetHashCode()).NextDouble() < 0.05)
                __instance.heldObject.Value.Quality +=
                    dropIn.Quality == SObject.highQuality ? 2 : 1;

            if (who.HasProfession(Profession.Artisan, true))
                __instance.MinutesUntilReady -= __instance.MinutesUntilReady / 4;
            else
                __instance.MinutesUntilReady -= __instance.MinutesUntilReady / 10;

            switch (__instance.name)
            {
                // golden mayonnaise is always iridium quality
                case "Mayonnaise Machine" when dropIn.ParentSheetIndex == 928 &&
                                               !ModEntry.ModHelper.ModRegistry.IsLoaded(
                                                   "ughitsmegan.goldenmayoForProducerFrameworkMod"):
                    __instance.heldObject.Value.Quality = SObject.bestQuality;
                    break;
            }
        }
    }

    /// <summary>
    ///     Patch to increment Gemologist counter for geodes cracked by Geode Crusher +  + reduce prestiged Breeder
    ///     incubation time.
    /// </summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> ObjectPerformObjectDropInActionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: if (Game1.player.professions.Contains(<gemologist_id>))
        ///		Data.IncrementField<uint>("GemologistMineralsCollected")
        ///	After: Game1.stats.GeodesCracked++;

        var dontIncreaseGemologistCounter = generator.DefineLabel();
        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(Stats).PropertySetter(nameof(Stats.GeodesCracked)))
                )
                .Advance()
                .InsertProfessionCheck((int) Profession.Gemologist)
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, dontIncreaseGemologistCounter),
                    new CodeInstruction(OpCodes.Call, typeof(Game1).PropertyGetter(nameof(Game1.player))),
                    new CodeInstruction(OpCodes.Ldstr, DataField.GemologistMineralsCollected.ToString()),
                    new CodeInstruction(OpCodes.Call,
                        typeof(FarmerExtensions)
                            .MethodNamed(nameof(FarmerExtensions.IncrementData), new[] {typeof(Farmer), typeof(DataField)})
                            .MakeGenericMethod(typeof(uint)))
                )
                .AddLabels(dontIncreaseGemologistCounter);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding Gemologist counter increment.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        /// From: minutesUntilReady.Value /= 2
        /// To: minutesUntilReady.Value /= who.professions.Contains(100 + <breeder_id>) ? 3 : 2

        helper.GoTo(0);
        var i = 0;
        repeat:
        try
        {
            var notPrestigedBreeder = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .FindProfessionCheck((int) Profession.Breeder, true)
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Ldloc_0)
                )
                .GetInstructionsUntil(out var got, true, true,
                    new CodeInstruction(OpCodes.Brfalse_S)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_2)
                )
                .AddLabels(notPrestigedBreeder)
                .Insert(got)
                .Retreat()
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_2)
                )
                .ReplaceWith(
                    new(OpCodes.Ldc_I4_S, (int) Profession.Breeder + 100)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brfalse_S)
                )
                .SetOperand(notPrestigedBreeder)
                .Advance()
                .Insert(
                    new CodeInstruction(OpCodes.Ldc_I4_3),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                )
                .Advance()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding prestiged Breeder incubation bonus.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        // repeat injection three times
        if (++i < 3) goto repeat;

        return helper.Flush();
    }

    #endregion harmony patches
}