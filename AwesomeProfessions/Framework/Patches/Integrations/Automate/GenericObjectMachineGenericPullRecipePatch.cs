using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using DaLion.Stardew.Common.Extensions;
using DaLion.Stardew.Common.Harmony;
using DaLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace DaLion.Stardew.Professions.Framework.Patches.Integrations;

internal class GenericObjectMachineGenericPullRecipePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal GenericObjectMachineGenericPullRecipePatch()
    {
        try
        {
            Original = "Pathoschild.Stardew.Automate.Framework.GenericObjectMachine`1".ToType()
                .MakeGenericType(typeof(SObject))
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .FirstOrDefault(m => m.Name == "GenericPullRecipe" && m.GetParameters().Length == 3);
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch to apply Artisan effects to automated generic machines.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> GenericObjectMachineGenericPullRecipeTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: GenericPullRecipeSubroutine(this, consumable)
        /// Before: return true;

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call)
                )
                .ToBuffer(2)
                .FindNext(
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Ret)
                )
                .InsertBuffer()
                .Insert(
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(GenericObjectMachineGenericPullRecipePatch).MethodNamed(
                            nameof(GenericPullRecipeSubroutine)))
                );
        }
        catch (Exception ex)
        {
            ModEntry.Log(
                $"Failed while patching modded Artisan behavior to generic Automate machines.\nHelper returned {ex}",
                LogLevel.Error);
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region private methods

    private static void GenericPullRecipeSubroutine(SObject machine, object consumable)
    {
        if (!machine.IsArtisanMachine() || !machine.heldObject.Value.IsArtisanGood()) return;

        if (consumable.GetType().GetProperty("Sample")?.GetValue(consumable) is not SObject input) return;

        var output = machine.heldObject.Value;
        if (machine.name == "Mayonnaise Machine")
        {
            // large eggs give double output at normal quality
            if (input.Name.ContainsAnyOf("Large", "L."))
            {
                output.Stack = 2;
                output.Quality = SObject.lowQuality;
            }
            else
            {
                switch (input.ParentSheetIndex)
                {
                    // ostrich mayonnaise keeps giving x10 output but doesn't respect input quality without Artisan
                    case 289 when !ModEntry.ModHelper.ModRegistry.IsLoaded(
                        "ughitsmegan.ostrichmayoForProducerFrameworkMod"):
                        output.Quality = SObject.lowQuality;
                        break;
                    // golden mayonnaise keeps giving gives single output but keeps golden quality
                    case 928 when !ModEntry.ModHelper.ModRegistry.IsLoaded(
                        "ughitsmegan.goldenmayoForProducerFrameworkMod"):
                        output.Stack = 1;
                        break;
                }
            }
        }

        var who = Game1.getFarmerMaybeOffline(machine.owner.Value) ?? Game1.MasterPlayer;
        if (!who.HasProfession("Artisan")) return;

        output.Quality = input.Quality;
        if (output.Quality < SObject.bestQuality &&
            new Random(Guid.NewGuid().GetHashCode()).NextDouble() < 0.05)
            output.Quality += output.Quality == SObject.highQuality ? 2 : 1;

        if (who.HasPrestigedProfession("Artisan"))
            machine.MinutesUntilReady -= machine.MinutesUntilReady / 4;
        else
            machine.MinutesUntilReady -= machine.MinutesUntilReady / 10;

        switch (machine.name)
        {
            // golden mayonnaise is always iridium quality
            case "Mayonnaise Machine" when input.ParentSheetIndex == 928 &&
                                           !ModEntry.ModHelper.ModRegistry.IsLoaded(
                                               "ughitsmegan.goldenmayoForProducerFrameworkMod"):
                output.Quality = SObject.bestQuality;
                break;
            // mead cares about input honey flower type
            case "Keg" when input.ParentSheetIndex == 340 && input.preservedParentSheetIndex.Value > 0:
                output.name = input.name.Split(" Honey")[0] + " Mead";
                output.preservedParentSheetIndex.Value = input.preservedParentSheetIndex.Value;
                output.Price = input.Price * 2;
                break;
        }
    }

    #endregion private methods
}