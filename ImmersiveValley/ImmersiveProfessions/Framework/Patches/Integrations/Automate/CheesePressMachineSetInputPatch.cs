namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.Automate;

#region using directives

using DaLion.Common;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class CheesePressMachineSetInput : DaLion.Common.Harmony.HarmonyPatch
{
    private static Func<object, Item>? _GetSample;

    /// <summary>Construct an instance.</summary>
    internal CheesePressMachineSetInput()
    {
        try
        {
            Target = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.CheesePressMachine".ToType()
                .RequireMethod("SetInput");
        }
        catch
        {
            // ignored
        }

        Transpiler!.priority = Priority.LowerThanNormal;
    }

    #region harmony patches

    /// <summary>Patch to apply Artisan effects to automated Cheese Press.</summary>
    [HarmonyTranspiler]
    [HarmonyPriority(Priority.LowerThanNormal)]
    private static IEnumerable<CodeInstruction>? GenericObjectMachineGenericPullRecipeTranspiler(
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
                .GetInstructions(out var got, 2)
                .FindNext(
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Ret)
                )
                .Insert(got)
                .Insert(
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(CheesePressMachineSetInput).RequireMethod(nameof(SetInputSubroutine)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching modded Artisan behavior for automated Cheese Press.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region private methods

    private static void SetInputSubroutine(SObject machine, object consumable)
    {
        _GetSample ??= consumable.GetType().RequirePropertyGetter("Sample")
            .CompileUnboundDelegate<Func<object, Item>>();
        if (_GetSample(consumable) is not SObject input) return;

        var owner = Game1.getFarmerMaybeOffline(machine.owner.Value) ?? Game1.MasterPlayer;
        if (!owner.HasProfession(Profession.Artisan)) return;

        var output = machine.heldObject.Value;
        output.Quality = input.Quality;
        if (output.Quality < SObject.bestQuality &&
            new Random(Guid.NewGuid().GetHashCode()).NextDouble() < 0.05)
            output.Quality += output.Quality == SObject.highQuality ? 2 : 1;

        if (owner.HasProfession(Profession.Artisan, true))
            machine.MinutesUntilReady -= machine.MinutesUntilReady / 4;
        else
            machine.MinutesUntilReady -= machine.MinutesUntilReady / 10;
    }

    #endregion private methods
}