namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Common;
using DaLion.Common.Attributes;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using StardewValley.Objects;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;
using Utility = StardewValley.Utility;

#endregion using directives

[UsedImplicitly]
[RequiresMod("spacechase0.SpaceCore")]
internal sealed class NewForgeMenuUpdatePatch : HarmonyPatch
{
    private static readonly Lazy<Func<object, ClickableTextureComponent>> GetLeftIngredientSpot = new(() =>
        "SpaceCore.Interface.NewForgeMenu"
            .ToType()
            .RequireField("leftIngredientSpot")
            .CompileUnboundFieldGetterDelegate<object, ClickableTextureComponent>());

    /// <summary>Initializes a new instance of the <see cref="NewForgeMenuUpdatePatch"/> class.</summary>
    internal NewForgeMenuUpdatePatch()
    {
        this.Target = "SpaceCore.Interface.NewForgeMenu"
            .ToType()
            .RequireMethod("update", new[] { typeof(GameTime) });
    }

    #region harmony patches

    /// <summary>Modify unforge behavior of combined Infinity Band.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ForgeMenuUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // Injected: if (ModEntry.Config.TheOneIridiumBand && ring.ParentSheetIndex == Constants.IRIDIUM_BAND_INDEX_I)
        //               UnforgeIridiumBand(ring);
        //           else ...
        // After: if (leftIngredientSpot.item is CombinedRing ring)
        var vanillaUnforge = generator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Stloc_S, helper.Locals[15])) // local 15 = CombinedRing ring
                .AdvanceUntil(new CodeInstruction(OpCodes.Brfalse_S))
                .GetOperand(out var resumeExecution)
                .Advance()
                .AddLabels(vanillaUnforge)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.TheOneIridiumBand))),
                    new CodeInstruction(OpCodes.Brfalse_S, vanillaUnforge),
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[15]),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Item).RequirePropertyGetter(nameof(Item.ParentSheetIndex))),
                    new CodeInstruction(OpCodes.Ldc_I4, Constants.IridiumBandIndex),
                    new CodeInstruction(OpCodes.Bne_Un_S, vanillaUnforge),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[15]),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(NewForgeMenuUpdatePatch).RequireMethod(nameof(UnforgeIridiumBand))),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution));
        }
        catch (Exception ex)
        {
            Log.E("Immersive Rings failed modifying unforge behavior of combined iridium band." +
                  "\n—-- Do NOT report this to SpaceCore's author. ---" +
                  $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void UnforgeIridiumBand(IClickableMenu menu, CombinedRing iridium)
    {
        var combinedRings = new List<Ring>(iridium.combinedRings);
        iridium.combinedRings.Clear();
        foreach (var gemstone in combinedRings.Select(ring => Gemstone.FromRing(ring.ParentSheetIndex)))
        {
            Utility.CollectOrDrop(new SObject(gemstone, 1));
            Utility.CollectOrDrop(new SObject(848, 5));
        }

        Utility.CollectOrDrop(new Ring(Constants.IridiumBandIndex));
        GetLeftIngredientSpot.Value(menu).item = null;
        Game1.playSound("coin");
    }

    #endregion injected subroutines
}
