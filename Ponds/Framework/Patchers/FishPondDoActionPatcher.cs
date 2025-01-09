namespace DaLion.Ponds.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using DaLion.Shared.Reflection;
using HarmonyLib;
using Netcode;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondDoActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondDoActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FishPondDoActionPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<FishPond>(nameof(FishPond.doAction));
    }

    private delegate void ShowObjectThrownIntoPondAnimationDelegate(
        FishPond instance, Farmer who, SObject whichObject, Action? callback = null);

    #region harmony patches

    /// <summary>
    ///     Inject ItemGrabMenu + allow legendary fish to share a pond with their extended families + secretly enrich
    ///     metals in radioactive ponds.
    /// </summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
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
                .PatternMatch([
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(FishPond).RequireField(nameof(FishPond.output))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(NetRef<Item>).RequirePropertyGetter(nameof(NetRef<Item>.Value))),
                    new CodeInstruction(OpCodes.Stloc_1),
                ])
                .Move(-1)
                .SetOpCode(OpCodes.Brfalse_S)
                .Move()
                .RemoveUntil([
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Ret),
                ])
                .Insert([
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(FishPondExtensions).RequireMethod(nameof(FishPondExtensions.RewardExp))),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(FishPondExtensions).RequireMethod(nameof(FishPondExtensions.OpenChumBucketMenu))),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding chum bucket menu.\nHelper returned {ex}");
            return null;
        }

        // Injected: TryThrowMetalIntoPond(this, who)
        // Before: if (fishType >= 0) open PondQueryMenu ...
        try
        {
            var resumeExecution = generator.DefineLabel();
            helper
                .PatternMatch(
                    [
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(FishPond).RequireField(nameof(FishPond.fishType))),
                    ],
                    ILHelper.SearchOption.Last)
                .StripLabels(out var labels)
                .AddLabels(resumeExecution)
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldarg_2),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(FishPondDoActionPatcher).RequireMethod(nameof(TryThrowMetalIntoPond))),
                        new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Ret),
                    ],
                    labels);
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
            !metallic.CanBeEnriched() || !pond.IsRadioactive())
        {
            return false;
        }

        var heldMinerals =
            Data.Read(pond, DataKeys.MetalsHeld)
                .ParseList<string>(';')
                .Select(li => li?.ParseTuple<string, int>())
                .WhereNotNull()
                .Where(li => !string.IsNullOrEmpty(li.Item1))
                .ToList();
        var count = heldMinerals.Sum(_ => 1);
        if (count >= 10)
        {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Buildings:PondFull"));
            return true;
        }

        var days = pond.GetEnrichmentTime(metallic);
        if (days == 0)
        {
            return false;
        }

        heldMinerals.Add((metallic.QualifiedItemId, days));
        Data.Write(
            pond,
            DataKeys.MetalsHeld,
            string.Join(';', heldMinerals
                .Select(m => string.Join(',', m.Item1, m.Item2))));

        Reflector
            .GetUnboundMethodDelegate<ShowObjectThrownIntoPondAnimationDelegate>(
                pond,
                "showObjectThrownIntoPondAnimation")
            .Invoke(pond, who, who.ActiveObject);
        who.reduceActiveItemByOne();
        return true;
    }

    #endregion injected subroutines
}
