namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Common.Attributes;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using DaLion.Common.Integrations.SpaceCore;
using DaLion.Stardew.Arsenal.Framework.Enchantments;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;
using Utility = StardewValley.Utility;

#endregion using directives

[UsedImplicitly]
[RequiresMod("spacechase0.SpaceCore")]
internal sealed class NewForgeMenuUpdatePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="NewForgeMenuUpdatePatch"/> class.</summary>
    internal NewForgeMenuUpdatePatch()
    {
        this.Target = "SpaceCore.Interface.NewForgeMenu"
            .ToType()
            .RequireMethod("update", new[] { typeof(GameTime) });
    }

    #region injected subroutines

    internal static void UnforgeHolyBlade(IClickableMenu menu, MeleeWeapon holy)
    {
        var heroSoul = (SObject)ModEntry.DynamicGameAssetsApi!.SpawnDGAItem(ModEntry.Manifest.UniqueID + "/Hero Soul");
        heroSoul.Stack = 3;
        Utility.CollectOrDrop(heroSoul);
        ExtendedSpaceCoreApi.GetNewForgeMenuLeftIngredientSpot.Value(menu).item = null;
        Game1.playSound("coin");
    }

    #endregion injected subroutines

    #region harmony patches

    /// <summary>Modify unforge behavior of Holy Blade.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ForgeMenuUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // Injected: if (ModEntry.Config.TrulyLegendaryGalaxySword && weapon.hasEnchantmentOfType<HolyEnchantment>())
        //               UnforgeHolyBlade(weapon);
        //           else ...
        // After: if (weapon != null)
        try
        {
            var vanillaUnforge = generator.DefineLabel();
            helper
                .FindLast(
                    new CodeInstruction(OpCodes.Pop),
                    new CodeInstruction(OpCodes.Br))
                .Advance()
                .GetOperand(out var resumeExecution)
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[10]), // local 10 = MeleeWeapon weapon
                    new CodeInstruction(OpCodes.Brfalse))
                .AdvanceUntil(new CodeInstruction(OpCodes.Ldloc_S))
                .AddLabels(vanillaUnforge)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.InfinityPlusOneWeapons))),
                    new CodeInstruction(OpCodes.Brfalse_S, vanillaUnforge),
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[10]),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Tool)
                            .RequireMethod(nameof(Tool.hasEnchantmentOfType))
                            .MakeGenericMethod(typeof(BlessedEnchantment))),
                    new CodeInstruction(OpCodes.Brfalse_S, vanillaUnforge),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_3, helper.Locals[10]),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(NewForgeMenuUpdatePatch).RequireMethod(nameof(UnforgeHolyBlade))),
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
}
