﻿namespace DaLion.Ligo.Modules.Arsenal.Common.Patches;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Arsenal.Common.Enchantments;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ForgeMenuUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ForgeMenuUpdatePatcher"/> class.</summary>
    internal ForgeMenuUpdatePatcher()
    {
        this.Target = this.RequireMethod<ForgeMenu>(nameof(ForgeMenu.update), new[] { typeof(GameTime) });
    }

    #region harmony patches

    /// <summary>Set unforge behavior of Holy Blade.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ForgeMenuUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // Injected: if (ModEntry.Config.Arsenal.Slingshots.TrulyLegendaryGalaxySword && weapon.hasEnchantmentOfType<HolyEnchantment>())
        //               UnforgeHolyBlade(weapon);
        //           else ...
        // After: if (weapon != null)
        try
        {
            var vanillaUnforge = generator.DefineLabel();
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[9]), // local 9 = MeleeWeapon weapon
                    new CodeInstruction(OpCodes.Brfalse))
                .Advance()
                .GetOperand(out var resumeExecution)
                .Advance()
                .AddLabels(vanillaUnforge)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Arsenal))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Config).RequirePropertyGetter(nameof(Config.InfinityPlusOne))),
                    new CodeInstruction(OpCodes.Brfalse_S, vanillaUnforge),
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[9]),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Tool)
                            .RequireMethod(nameof(Tool.hasEnchantmentOfType))
                            .MakeGenericMethod(typeof(BlessedEnchantment))),
                    new CodeInstruction(OpCodes.Brfalse_S, vanillaUnforge),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_3, helper.Locals[9]),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ForgeMenuUpdatePatcher).RequireMethod(nameof(UnforgeHolyBlade))),
                    new CodeInstruction(OpCodes.Br, resumeExecution));
        }
        catch (Exception ex)
        {
            Log.E($"Failed modifying unforge behavior of holy blade.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:Elements should be ordered by access", Justification = "Preference for patch classes with injected subroutines.")]
    internal static void UnforgeHolyBlade(ForgeMenu menu, MeleeWeapon holy)
    {
        var heroSoul =
            (SObject)Ligo.Integrations.DynamicGameAssetsApi!.SpawnDGAItem(ModEntry.Manifest.UniqueID + "/Hero Soul");
        heroSoul.Stack = 3;
        Utility.CollectOrDrop(heroSoul);
        menu.leftIngredientSpot.item = null;
        Game1.playSound("coin");
    }

    #endregion injected subroutines
}