namespace DaLion.Overhaul.Modules.Rings.Patchers.Integration;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using SpaceCore.Interface;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
[ModRequirement("spacechase0.SpaceCore")]
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

        // Injected: if (ModEntry.Config.Rings.TheOneInfinityBand && ring.ParentSheetIndex == Globals.InfinityBandIndex.Value)
        //               UnforgeInfinityBand(ring);
        //           else ...
        // After: if (leftIngredientSpot.item is CombinedRing ring)
        try
        {
            var vanillaUnforge = generator.DefineLabel();
            var infinityBandIndex = generator.DeclareLocal(typeof(int?));
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
                        new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Rings))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Config).RequirePropertyGetter(nameof(Config.TheOneInfinityBand))),
                        new CodeInstruction(OpCodes.Brfalse_S, vanillaUnforge),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Globals).RequirePropertyGetter(nameof(Globals.InfinityBandIndex))),
                        new CodeInstruction(OpCodes.Stloc_S, infinityBandIndex),
                        new CodeInstruction(OpCodes.Ldloca_S, infinityBandIndex),
                        new CodeInstruction(OpCodes.Call, typeof(int?).RequirePropertyGetter(nameof(Nullable<int>.HasValue))),
                        new CodeInstruction(OpCodes.Brfalse_S, vanillaUnforge),
                        new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[15]),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Item).RequirePropertyGetter(nameof(Item.ParentSheetIndex))),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Globals).RequirePropertyGetter(nameof(Globals.InfinityBandIndex))),
                        new CodeInstruction(OpCodes.Stloc_S, infinityBandIndex),
                        new CodeInstruction(OpCodes.Ldloca_S, infinityBandIndex),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(int?).RequirePropertyGetter(nameof(Nullable<int>.Value))),
                        new CodeInstruction(OpCodes.Bne_Un_S, vanillaUnforge),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[15]),
                        new CodeInstruction(
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

    private static void UnforgeInfinityBand(NewForgeMenu menu, CombinedRing infinity)
    {
        for (var i = 0; i < infinity.combinedRings.Count; i++)
        {
            var ring = infinity.combinedRings[i];
            var gemstone = Gemstone.FromRing(ring.ParentSheetIndex);
            Utility.CollectOrDrop(new SObject(gemstone.ObjectIndex, 1));
            Utility.CollectOrDrop(new SObject(848, 5));
        }

        infinity.combinedRings.Clear();
        Utility.CollectOrDrop(new Ring(Globals.InfinityBandIndex!.Value));
        menu.leftIngredientSpot.item = null;
        Game1.playSound("coin");
    }

    #endregion injected subroutines
}
