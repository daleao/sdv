namespace DaLion.Stardew.Professions.Framework.Patches.Common;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using Stardew.Common.Harmony;
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
            who.HasProfession("Ecologist"))
            ModData.Increment<uint>(DataField.EcologistItemsForaged);
    }

    /// <summary>Patch to increment Gemologist counter for gems collected from Crystalarium.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> ObjectCheckForActionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: if (who.professions.Contains(<gemologist_id>) && name.Equals("Crystalarium"))
        ///		Data.IncrementField<uint>("GemologistMineralsCollected")
        ///	Before: switch (name)

        var dontIncreaseGemologistCounter = iLGenerator.DefineLabel();
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
                .InsertProfessionCheckForPlayerOnStack((int) Profession.Gemologist,
                    dontIncreaseGemologistCounter)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(SObject).PropertyGetter(nameof(SObject.name))),
                    new CodeInstruction(OpCodes.Ldstr, "Crystalarium"),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(string).MethodNamed(nameof(string.Equals), new[] {typeof(string)})),
                    new CodeInstruction(OpCodes.Brfalse_S, dontIncreaseGemologistCounter),
                    new CodeInstruction(OpCodes.Ldstr, DataField.GemologistMineralsCollected.ToString()),
                    new CodeInstruction(OpCodes.Ldnull),
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModData)
                            .MethodNamed(nameof(ModData.Increment), new[] {typeof(DataField), typeof(Farmer)})
                            .MakeGenericMethod(typeof(uint)))
                )
                .AddLabels(dontIncreaseGemologistCounter);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding Gemologist counter increment.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}