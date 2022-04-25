namespace DaLion.Stardew.Professions.Framework.Patches.Common;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal class ObjectCheckForActionPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal ObjectCheckForActionPatch()
    {
        Original = RequireMethod<SObject>(nameof(SObject.checkForAction));
    }

    #region harmony patches

    /// <summary>Patch to remember object state.</summary>
    [HarmonyPrefix]
    private static bool ObjectCheckForActionPrefix(SObject __instance, ref bool __state)
    {
        __state = __instance.heldObject.Value is not null;
        return true; // run original logic
    }

    /// <summary>Patch to increment Ecologist counter for Mushroom Box.</summary>
    [HarmonyPostfix]
    private static void ObjectCheckForActionPostfix(SObject __instance, bool __state, Farmer who)
    {
        if (__state && __instance.heldObject.Value is null && __instance.IsMushroomBox() &&
            who.HasProfession(Profession.Ecologist))
            Game1.player.IncrementData<uint>(DataField.EcologistItemsForaged);
    }

    /// <summary>Patch to increment Gemologist counter for gems collected from Crystalarium + increase Honey quality with age + increase production frequency of Producer Bee House.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> ObjectCheckForActionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: if (who.professions.Contains(<gemologist_id>) && name.Equals("Crystalarium"))
        ///		Data.IncrementField<uint>("GemologistMineralsCollected")
        ///	Before: switch (name)

        var dontIncreaseGemologistCounter = generator.DefineLabel();
        try
        {
            helper
                .FindLast(
                    new CodeInstruction(OpCodes.Ldstr, "coin")
                )
                .Advance(2)
                .Insert(
                    // prepare profession check
                    new CodeInstruction(OpCodes.Ldarg_1) // arg 1 = Farmer who
                )
                .InsertProfessionCheck((int) Profession.Gemologist, forLocalPlayer: false)
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, dontIncreaseGemologistCounter),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(SObject).RequirePropertyGetter(nameof(SObject.name))),
                    new CodeInstruction(OpCodes.Ldstr, "Crystalarium"),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(string).RequireMethod(nameof(string.Equals), new[] {typeof(string)})),
                    new CodeInstruction(OpCodes.Brfalse_S, dontIncreaseGemologistCounter),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Ldstr, DataField.GemologistMineralsCollected.ToString()),
                    new CodeInstruction(OpCodes.Call,
                        typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.IncrementData), new[] {typeof(Farmer), typeof(DataField)})
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

        /// From: minutesUntilReady.Value = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, 4);
        /// To: minutesUntilReady.Value = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, this.DoesOwnerHaveProfession(<producer_id>)
        ///     ? this.DoesOwnerHaveProfession(100 + <producer_id>
        ///         ? 1
        ///         : 2
        ///     : 4);

        var isNotProducer = generator.DefineLabel();
        var isNotPrestiged = generator.DefineLabel();
        var resumeExecution = generator.DefineLabel();
        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Ldc_I4_4),
                    new CodeInstruction(OpCodes.Call,
                        typeof(Utility).RequireMethod(nameof(Utility.CalculateMinutesUntilMorning),
                            new[] { typeof(int), typeof(int) }))
                )
                .AddLabels(isNotProducer)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldc_I4_3), // 3 = Profession.Producer
                    new CodeInstruction(OpCodes.Ldc_I4_0), // false for not prestiged
                    new CodeInstruction(OpCodes.Call,
                        typeof(SObjectExtensions).RequireMethod(nameof(SObjectExtensions.DoesOwnerHaveProfession))),
                    new CodeInstruction(OpCodes.Brfalse_S, isNotProducer),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldc_I4_3),
                    new CodeInstruction(OpCodes.Ldc_I4_1), // true for prestiged
                    new CodeInstruction(OpCodes.Call,
                        typeof(SObjectExtensions).RequireMethod(nameof(SObjectExtensions.DoesOwnerHaveProfession))),
                    new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    new CodeInstruction(OpCodes.Ldc_I4_2),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                )
                .Retreat(2)
                .AddLabels(isNotPrestiged)
                .Return()
                .Advance()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching bee house production speed for Producers.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}