namespace DaLion.Ligo.Modules.Rings.Patches;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using StardewValley.Objects;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class ForgeMenuUpdatePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="ForgeMenuUpdatePatch"/> class.</summary>
    internal ForgeMenuUpdatePatch()
    {
        this.Target = this.RequireMethod<ForgeMenu>(nameof(ForgeMenu.update), new[] { typeof(GameTime) });
    }

    #region harmony patches

    /// <summary>Modify unforge behavior of combined Infinity Band.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ForgeMenuUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // Injected: if (ModEntry.Config.Arsenal.Slingshots.TheOneInfinityBand && ring.ParentSheetIndex == Globals.InfinityBandIndex)
        //               UnforgeInfinityBand(ring);
        //           else ...
        // After: if (leftIngredientSpot.item is CombinedRing ring)
        try
        {
            var vanillaUnforge = generator.DefineLabel();
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Stloc_S, helper.Locals[14])) // local 14 = CombinedRing ring
                .AdvanceUntil(new CodeInstruction(OpCodes.Brfalse_S))
                .GetOperand(out var resumeExecution)
                .Advance()
                .AddLabels(vanillaUnforge)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Rings))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Config).RequirePropertyGetter(nameof(Config.TheOneInfinityBand))),
                    new CodeInstruction(OpCodes.Brfalse_S, vanillaUnforge),
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[14]),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Item).RequirePropertyGetter(nameof(Item.ParentSheetIndex))),
                    new CodeInstruction(OpCodes.Call, typeof(Globals).RequirePropertyGetter(nameof(Globals.InfinityBandIndex))),
                    new CodeInstruction(OpCodes.Bne_Un_S, vanillaUnforge),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[14]),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ForgeMenuUpdatePatch).RequireMethod(nameof(UnforgeInfinityBand))),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution));
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
        var combinedRings = new List<Ring>(infinity.combinedRings);
        infinity.combinedRings.Clear();
        foreach (var gemstone in combinedRings.Select(ring => Gemstone.FromRing(ring.ParentSheetIndex)))
        {
            Utility.CollectOrDrop(new SObject(gemstone, 1));
            Utility.CollectOrDrop(new SObject(848, 5));
        }

        Utility.CollectOrDrop(new Ring(Globals.InfinityBandIndex));
        menu.leftIngredientSpot.item = null;
        Game1.playSound("coin");
    }

    #endregion injected subroutines
}
