namespace DaLion.Ligo.Modules.Rings.Patchers;

using System;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using SpaceCore.Interface;
using StardewValley.Menus;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
[RequiresMod("spacechase0.SpaceCore")]
internal sealed class NewForgeMenuUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NewForgeMenuUpdatePatcher"/> class.</summary>
    internal NewForgeMenuUpdatePatcher()
    {
        this.Target = this.RequireMethod<NewForgeMenu>("update", new[] { typeof(GameTime) });
    }

    #region harmony patches

    /// <summary>Modify unforge behavior of combined Infinity Band.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? NewForgeMenuUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: if (ArsenalModule.Config.Slingshots.TheOneInfinityBand && ring.ParentSheetIndex == Globals.InfinityBandIndex.Value)
        //               UnforgeInfinityBand(ring);
        //           else ...
        // After: if (leftIngredientSpot.item is CombinedRing ring)
        try
        {
            var vanillaUnforge = generator.DefineLabel();
            helper
                .Match(
                    new[] { new CodeInstruction(OpCodes.Stloc_S, helper.Locals[15]) }) // local 15 = CombinedRing ring
                .Match(new[] { new CodeInstruction(OpCodes.Brfalse_S) })
                .GetOperand(out var resumeExecution)
                .Move()
                .AddLabels(vanillaUnforge)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Rings))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(RingsConfig).RequirePropertyGetter(nameof(RingsConfig.TheOneInfinityBand))),
                        new CodeInstruction(OpCodes.Brfalse_S, vanillaUnforge),
                        new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[15]), new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Item).RequirePropertyGetter(nameof(Item.ParentSheetIndex))),
                        new CodeInstruction(OpCodes.Call,
                            typeof(Globals).RequirePropertyGetter(nameof(Globals.InfinityBandIndex))),
                        new CodeInstruction(OpCodes.Call,
                            typeof(int?).RequirePropertyGetter(nameof(Nullable<int>.Value))),
                        new CodeInstruction(OpCodes.Bne_Un_S, vanillaUnforge), new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[15]), new CodeInstruction(
                            OpCodes.Call,
                            typeof(NewForgeMenuUpdatePatcher).RequireMethod(nameof(UnforgeInfinityBand))),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    });
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

    private static void UnforgeInfinityBand(IClickableMenu menu, CombinedRing infinity)
    {
        var combinedRings = new List<Ring>(infinity.combinedRings);
        infinity.combinedRings.Clear();
        foreach (var gemstone in combinedRings.Select(ring => Gemstone.FromRing(ring.ParentSheetIndex)))
        {
            Utility.CollectOrDrop(new SObject(gemstone, 1));
            Utility.CollectOrDrop(new SObject(848, 5));
        }

        Utility.CollectOrDrop(new Ring(Globals.InfinityBandIndex!.Value));
        Reflector
            .GetUnboundFieldGetter<object, ClickableTextureComponent>(menu, "leftIngredientSpot")
            .Invoke(menu).item = null;
        Game1.playSound("coin");
    }

    #endregion injected subroutines
}
