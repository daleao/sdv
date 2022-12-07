namespace DaLion.Ligo.Modules.Ponds.Patchers;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Ponds.Extensions;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Netcode;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondDoActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondDoActionPatcher"/> class.</summary>
    internal FishPondDoActionPatcher()
    {
        this.Target = this.RequireMethod<FishPond>(nameof(FishPond.doAction));
    }

    private delegate void ShowObjectThrownIntoPondAnimationDelegate(
        FishPond instance, Farmer who, SObject whichObject, DelayedAction.delayedBehavior? callback = null);

    #region harmony patches

    /// <summary>
    ///     Inject ItemGrabMenu + allow legendary fish to share a pond with their extended families + secretly enrich
    ///     metals in radioactive ponds.
    /// </summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FishPondDoActionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: if (output.Value != null) {...} return true;
        // To: if (output.Value != null)
        // {
        //     this.RewardExp(who);
        //     return this.OpenChumBucketMenu();
        // }
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(FishPond).RequireField(nameof(FishPond.output))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(NetRef<Item>).RequirePropertyGetter(nameof(NetRef<Item>.Value))),
                    new CodeInstruction(OpCodes.Stloc_1))
                .Retreat()
                .SetOpCode(OpCodes.Brfalse_S)
                .Advance()
                .RemoveInstructionsUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Ret))
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(FishPondExtensions).RequireMethod(nameof(FishPondExtensions.RewardExp))),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(FishPondExtensions).RequireMethod(nameof(FishPondExtensions.OpenChumBucketMenu))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding chum bucket menu.\nHelper returned {ex}");
            return null;
        }

        // From: if (who.ActiveObject.ParentSheetIndex != (int) fishType)
        // To: if (who.ActiveObject.ParentSheetIndex != (int) fishType && !IsExtendedFamily(who.ActiveObject.ParentSheetIndex, (int) fishType)
        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Ldfld, typeof(FishPond).RequireField(nameof(FishPond.fishType))),
                    new CodeInstruction(OpCodes.Call, typeof(NetFieldBase<int, NetInt>).RequireMethod("op_Implicit")),
                    new CodeInstruction(OpCodes.Beq))
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Ldloc_0))
                .GetInstructionsUntil(
                    out var got,
                    true,
                    true,
                    new CodeInstruction(OpCodes.Beq))
                .InsertInstructions(got)
                .Retreat()
                .InsertInstructions(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(FishPondDoActionPatcher).RequireMethod(nameof(IsExtendedFamilyMember))))
                .SetOpCode(OpCodes.Brtrue_S);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding family ties to legendary fish in ponds.\nHelper returned {ex}");
            return null;
        }

        // Injected: TryThrowMetalIntoPond(this, who)
        // Before: if (fishType >= 0) open PondQueryMenu ...
        try
        {
            var resumeExecution = generator.DefineLabel();
            helper
                .FindLast(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(FishPond).RequireField(nameof(FishPond.fishType))))
                .StripLabels(out var labels)
                .AddLabels(resumeExecution)
                .InsertWithLabels(
                    labels,
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(FishPondDoActionPatcher).RequireMethod(nameof(TryThrowMetalIntoPond))),
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Ret));
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding metal enrichment to radioactive ponds.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static bool TryThrowMetalIntoPond(FishPond pond, Farmer who)
    {
        if (who.ActiveObject is not { Category: SObject.metalResources } metallic ||
            !metallic.CanEnrich() || !pond.IsRadioactive())
        {
            return false;
        }

        var heldMinerals =
            pond.Read(DataFields.MetalsHeld)
                .ParseList<string>(";")
                .Select(li => li?.ParseTuple<int, int>())
                .WhereNotNull()
                .ToList();
        var count = heldMinerals.Sum(m => new SObject(m.Item1, 1).Name.Contains("Bar") ? 5 : 1);
        if (count >= 20)
        {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Buildings:PondFull"));
            return true;
        }

        var days = pond.GetEnrichmentDuration(metallic);
        if (days == 0)
        {
            return false;
        }

        heldMinerals.Add((metallic.ParentSheetIndex, days));
        pond.Write(DataFields.MetalsHeld, string.Join(';', heldMinerals.Select(m => string.Join(',', m.Item1, m.Item2))));

        ModEntry.Reflector
            .GetUnboundMethodDelegate<ShowObjectThrownIntoPondAnimationDelegate>(
                pond,
                "showObjectThrownIntoPondAnimation")
            .Invoke(pond, who, who.ActiveObject);
        who.reduceActiveItemByOne();
        return true;
    }

    private static bool IsExtendedFamilyMember(int held, int other)
    {
        return Collections.ExtendedFamilyPairs.TryGetValue(other, out var pair) && pair == held;
    }

    #endregion injected subroutines
}
