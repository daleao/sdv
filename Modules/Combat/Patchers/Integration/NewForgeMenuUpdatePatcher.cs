namespace DaLion.Overhaul.Modules.Combat.Patchers.Integration;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Combat;
using DaLion.Overhaul.Modules.Combat.Enchantments;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Overhaul.Modules.Combat.Resonance;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using SpaceCore.Interface;
using StardewValley.Objects;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[ModRequirement("spacechase0.SpaceCore")]
internal sealed class NewForgeMenuUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NewForgeMenuUpdatePatcher"/> class.</summary>
    internal NewForgeMenuUpdatePatcher()
    {
        this.Target = this.RequireMethod<NewForgeMenu>(nameof(NewForgeMenu.update), new[] { typeof(GameTime) });
    }

    #region harmony patches

    /// <summary>Modify unforge behavior of Holy Blade.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? NewForgeMenuUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: if (CombatModule.Config.InfinityPlusOne && weapon.hasEnchantmentOfType<HolyEnchantment>() && weapon.GetTotalForgeLevels <= 0)
        //               UnforgeHolyBlade(weapon);
        //           else ...
        // After: if (weapon != null)
        try
        {
            var vanillaUnforge = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Pop),
                        new CodeInstruction(OpCodes.Br),
                    },
                    ILHelper.SearchOption.Last)
                .Move()
                .GetOperand(out var resumeExecution)
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[10]), // local 10 = MeleeWeapon weapon
                        new CodeInstruction(OpCodes.Brfalse),
                    },
                    ILHelper.SearchOption.First)
                .Match(new[] { new CodeInstruction(OpCodes.Ldloc_S) })
                .AddLabels(vanillaUnforge)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Combat))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Config).RequirePropertyGetter(nameof(Config.EnableHeroQuest))),
                        new CodeInstruction(OpCodes.Brfalse_S, vanillaUnforge),
                        new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[10]),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Tool)
                                .RequireMethod(nameof(Tool.hasEnchantmentOfType))
                                .MakeGenericMethod(typeof(BlessedEnchantment))),
                        new CodeInstruction(OpCodes.Brfalse_S, vanillaUnforge),
                        new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[10]),
                        new CodeInstruction(OpCodes.Ldc_I4_1), // 1 is for true
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Tool)
                                .RequireMethod(nameof(Tool.GetTotalForgeLevels))),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Bgt_S, vanillaUnforge),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldloc_3, helper.Locals[10]),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(NewForgeMenuUpdatePatcher).RequireMethod(nameof(UnforgeHolyBlade))),
                        new CodeInstruction(OpCodes.Br, resumeExecution),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting unforge behavior of Holy Blade.\nHelper returned {ex}");
            return null;
        }

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
                    new[] { new CodeInstruction(OpCodes.Stloc_S, helper.Locals[15]) }, // local 15 = CombinedRing ring
                    ILHelper.SearchOption.First)
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
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Combat))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Config).RequirePropertyGetter(nameof(Config.EnableInfinityBand))),
                        new CodeInstruction(OpCodes.Brfalse_S, vanillaUnforge),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(JsonAssetsIntegration).RequirePropertyGetter(nameof(JsonAssetsIntegration.InfinityBandIndex))),
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
                            typeof(JsonAssetsIntegration).RequirePropertyGetter(nameof(JsonAssetsIntegration.InfinityBandIndex))),
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
                  $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void UnforgeHolyBlade(NewForgeMenu menu, MeleeWeapon holy)
    {
        Utility.CollectOrDrop(new SObject(JsonAssetsIntegration.HeroSoulIndex!.Value, 1));
        menu.leftIngredientSpot.item = null;
        Game1.playSound("coin");
    }

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
        Utility.CollectOrDrop(new Ring(JsonAssetsIntegration.InfinityBandIndex!.Value));
        menu.leftIngredientSpot.item = null;
        Game1.playSound("coin");
    }

    #endregion injected subroutines
}
