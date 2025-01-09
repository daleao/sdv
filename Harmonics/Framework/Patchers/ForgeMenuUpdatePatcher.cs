namespace DaLion.Harmonics.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Harmonics.Framework;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class ForgeMenuUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ForgeMenuUpdatePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal ForgeMenuUpdatePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<ForgeMenu>(nameof(ForgeMenu.update), new[] { typeof(GameTime) });
    }

    #region harmony patches

    /// <summary>Modify unforge behavior of combined Infinity Band.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? ForgeMenuUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: if (ring.QualifiedItemId == InfinityBandIndex)
        //               UnforgeInfinityBand(leftRing);
        //           else ...
        // After: if (leftIngredientSpot.item is CombinedRing leftRing)
        try
        {
            var vanillaUnforge = generator.DefineLabel();
            helper
                .PatternMatch([
                    new CodeInstruction(OpCodes.Stloc_S, helper.Locals[11]), // local 11 = CombinedRing leftRing
                ])
                .PatternMatch([new CodeInstruction(OpCodes.Brfalse_S)])
                .GetOperand(out var resumeExecution)
                .Move()
                .AddLabels(vanillaUnforge)
                .Insert([
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[11]),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Item).RequirePropertyGetter(nameof(Item.QualifiedItemId))),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(HarmonicsMod).RequirePropertyGetter(nameof(InfinityBandId))),
                    new CodeInstruction(OpCodes.Bne_Un_S, vanillaUnforge),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[11]),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ForgeMenuUpdatePatcher).RequireMethod(nameof(UnforgeInfinityBand))),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed modifying unforge behavior of combined Infinity Band.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void UnforgeInfinityBand(ForgeMenu menu, CombinedRing infinity)
    {
        foreach (var ring in infinity.combinedRings)
        {
            var gemstone = Gemstone.FromRing(ring.QualifiedItemId);
            Utility.CollectOrDrop(ItemRegistry.Create<SObject>(gemstone.ObjectId));
            Utility.CollectOrDrop(ItemRegistry.Create<SObject>(QualifiedObjectIds.CinderShard, 5));
        }

        infinity.combinedRings.Clear();
        Utility.CollectOrDrop(ItemRegistry.Create<Ring>(InfinityBandId));
        menu.leftIngredientSpot.item = null;
        Game1.playSound("coin");
    }

    #endregion injected subroutines
}
