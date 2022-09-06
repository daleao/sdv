namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using Common;
using Common.Extensions.Reflection;
using Common.Harmony;
using Enchantments;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class ForgeMenuUpdatePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal ForgeMenuUpdatePatch()
    {
        Target = RequireMethod<ForgeMenu>(nameof(ForgeMenu.update), new[] { typeof(GameTime) });
    }

    #region harmony patches

    /// <summary>Set unforge behavior of Holy Blade.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ForgeMenuUpdateTranspiler(IEnumerable<CodeInstruction> instructions,
        ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: if (ModEntry.Config.TrulyLegendaryGalaxySword && weapon.hasEnchantmentOfType<HolyEnchantment>())
        ///               UnforgeHolyBlade(weapon);
        ///           else ...
        /// After: if (weapon != null)

        var vanillaUnforge = generator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[9]), // local 9 = MeleeWeapon weapon
                    new CodeInstruction(OpCodes.Brfalse)
                )
                .Advance()
                .GetOperand(out var resumeExecution)
                .Advance()
                .AddLabels(vanillaUnforge)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.InfinityPlusOneWeapons))),
                    new CodeInstruction(OpCodes.Brfalse_S, vanillaUnforge),
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[9]),
                    new CodeInstruction(OpCodes.Call,
                        typeof(Tool).RequireMethod(nameof(Tool.hasEnchantmentOfType))
                            .MakeGenericMethod(typeof(BlessedEnchantment))),
                    new CodeInstruction(OpCodes.Brfalse_S, vanillaUnforge),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_3, helper.Locals[9]),
                    new CodeInstruction(OpCodes.Call,
                        typeof(ForgeMenuUpdatePatch).RequireMethod(nameof(UnforgeHolyBlade))),
                    new CodeInstruction(OpCodes.Br, resumeExecution)
                );
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

    internal static void UnforgeHolyBlade(ForgeMenu menu, MeleeWeapon holy)
    {
        var heroSoul = (SObject)ModEntry.DynamicGameAssetsApi!.SpawnDGAItem(ModEntry.Manifest.UniqueID + "/Hero Soul");
        heroSoul.Stack = 3;
        StardewValley.Utility.CollectOrDrop(heroSoul);
        menu.leftIngredientSpot.item = null;
        Game1.playSound("coin");
    }

    #endregion injected subroutines
}