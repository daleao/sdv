namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using Common;
using Common.Extensions;
using Common.Extensions.Collections;
using Common.Extensions.Reflection;
using Common.Harmony;
using Common.ModData;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using Netcode;
using StardewValley;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondDoActionPatch : Common.Harmony.HarmonyPatch
{
    private delegate void ShowObjectThrownIntoPondAnimationDelegate(FishPond instance, Farmer who, SObject whichObject,
        DelayedAction.delayedBehavior? callback = null);

    private static ShowObjectThrownIntoPondAnimationDelegate? _ShowObjectThrownIntoPondAnimation;

    /// <summary>Construct an instance.</summary>
    internal FishPondDoActionPatch()
    {
        Target = RequireMethod<FishPond>(nameof(FishPond.doAction));
    }

    #region harmony patches

    /// <summary>Inject ItemGrabMenu + allow legendary fish to share a pond with their extended families + secretly enrich metals in radioactive ponds.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FishPondDoActionTranspiler(IEnumerable<CodeInstruction> instructions,
        ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: if (output.Value != null) {...} return true;
        /// To: if (output.Value != null)
        /// {
        ///     this.RewardExp(who);
        ///     return this.OpenChumBucketMenu();
        /// }

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(FishPond).RequireField(nameof(FishPond.output))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(NetRef<Item>).RequirePropertyGetter(nameof(NetRef<Item>.Value))),
                    new CodeInstruction(OpCodes.Stloc_1)
                )
                .Retreat()
                .SetOpCode(OpCodes.Brfalse_S)
                .Advance()
                .RemoveUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Ret)
                )
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(OpCodes.Call,
                        typeof(FishPondExtensions).RequireMethod(nameof(FishPondExtensions.RewardExp))),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(OpCodes.Call,
                        typeof(FishPondExtensions).RequireMethod(nameof(FishPondExtensions.OpenChumBucketMenu)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding chum bucket menu.\nHelper returned {ex}");
            return null;
        }

        /// From: if (who.ActiveObject.ParentSheetIndex != (int) fishType)
        /// To: if (who.ActiveObject.ParentSheetIndex != (int) fishType && !IsExtendedFamily(who.ActiveObject.ParentSheetIndex, (int) fishType)

        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Ldfld, typeof(FishPond).RequireField(nameof(FishPond.fishType))),
                    new CodeInstruction(OpCodes.Call, typeof(NetFieldBase<int, NetInt>).RequireMethod("op_Implicit")),
                    new CodeInstruction(OpCodes.Beq)
                )
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Ldloc_0)
                )
                .GetInstructionsUntil(out var got, true, true,
                    new CodeInstruction(OpCodes.Beq)
                )
                .Insert(got)
                .Retreat()
                .Insert(
                    new CodeInstruction(OpCodes.Call,
                        typeof(Framework.Utils).RequireMethod(nameof(Framework.Utils.IsExtendedFamilyMember)))
                )
                .SetOpCode(OpCodes.Brtrue_S);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding family ties to legendary fish in ponds.\nHelper returned {ex}");
            return null;
        }

        /// Injected: TryThrowMetalIntoPond(this, who)
        /// Before: if (fishType >= 0) open PondQueryMenu ...

        var resumeExecution = generator.DefineLabel();
        try
        {
            helper
                .FindLast(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(FishPond).RequireField(nameof(FishPond.fishType)))
                )
                .StripLabels(out var labels)
                .AddLabels(resumeExecution)
                .InsertWithLabels(
                    labels,
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(OpCodes.Call,
                        typeof(FishPondDoActionPatch).RequireMethod(nameof(TryThrowMetalIntoPond))),
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Ret)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding metal enrichment to radioactive ponds.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static bool TryThrowMetalIntoPond(FishPond pond, Farmer who)
    {
        if (who.ActiveObject is not { Category: SObject.metalResources } metallic ||
            !(metallic.IsNonRadioactiveOre() || metallic.IsNonRadioactiveIngot()) ||
            !pond.HasRadioactiveFish()) return false;

        var heldMinerals =
            ModDataIO.Read(pond, "MetalsHeld")
                .ParseList<string>(";")?
                .Select(li => li.ParseTuple<int, int>())
                .WhereNotNull()
                .ToList()
            ?? new List<(int, int)>();
        var count = heldMinerals.Sum(m => new SObject(m.Item1, 1).IsNonRadioactiveIngot() ? 5 : 1);
        if (count >= 20)
        {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Buildings:PondFull"));
            return true;
        }

        var days = pond.GetEnrichmentDuration(metallic);
        if (days == 0) return false;

        heldMinerals.Add((metallic.ParentSheetIndex, days));
        ModDataIO.Write(pond, "MetalsHeld",
            string.Join(';', heldMinerals.Select(m => string.Join(',', m.Item1, m.Item2))));
        _ShowObjectThrownIntoPondAnimation ??= typeof(FishPond).RequireMethod("showObjectThrownIntoPondAnimation")
            .CompileUnboundDelegate<ShowObjectThrownIntoPondAnimationDelegate>();
        _ShowObjectThrownIntoPondAnimation(pond, who, who.ActiveObject);
        who.reduceActiveItemByOne();
        return true;
    }

    #endregion injected subroutines
}