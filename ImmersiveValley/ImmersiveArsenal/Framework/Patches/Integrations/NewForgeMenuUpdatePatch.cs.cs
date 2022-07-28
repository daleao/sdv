namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using Common;
using Common.Attributes;
using Common.Extensions.Reflection;
using Common.Harmony;
using Enchantments;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly, RequiresMod("spacechase0.SpaceCore")]
internal sealed class NewForgeMenuUpdatePatch : Common.Harmony.HarmonyPatch
{
    private static readonly Type _NewForgeMenuType = "SpaceCore.Interface.NewForgeMenu".ToType();

    /// <summary>Construct an instance.</summary>
    internal NewForgeMenuUpdatePatch()
    {
        Target = _NewForgeMenuType.RequireMethod("update", new[] { typeof(GameTime) });
    }

    #region harmony patches

    /// <summary>Modify unforge behavior of Holy Blade + allow unforge Slingshot.</summary>
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
                .FindLast(
                    new CodeInstruction(OpCodes.Pop),
                    new CodeInstruction(OpCodes.Br)
                )
                .Advance()
                .GetOperand(out var resumeExecution)
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[10]), // local 10 = MeleeWeapon weapon
                    new CodeInstruction(OpCodes.Brfalse)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldloc_S)
                )
                .AddLabels(vanillaUnforge)
                .Insert(
                    new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.InfinityPlusOneWeapons))),
                    new CodeInstruction(OpCodes.Brfalse_S, vanillaUnforge),
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[10]),
                    new CodeInstruction(OpCodes.Call,
                        typeof(Tool).RequireMethod(nameof(Tool.hasEnchantmentOfType))
                            .MakeGenericMethod(typeof(HolyEnchantment))),
                    new CodeInstruction(OpCodes.Brfalse_S, vanillaUnforge),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_3, helper.Locals[10]),
                    new CodeInstruction(OpCodes.Call,
                        typeof(NewForgeMenuUpdatePatch).RequireMethod(nameof(UnforgeHolyBlade))),
                    new CodeInstruction(OpCodes.Br, resumeExecution)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed modifying unforge behavior of holy blade.\nHelper returned {ex}");
            return null;
        }

        /// Injected: else if (leftIngredientSpot.item is Slingshot slingshot && ModEntry.Config.AllowSlingshotForges)
        ///             UnforgeSlingshot(leftIngredientSpot.item);
        /// Between: MeleeWeapon and CombinedRing unforge behaviors...

        var elseIfCombinedRing = generator.DefineLabel();
        var slingshot = generator.DeclareLocal(typeof(Slingshot));
        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Isinst, typeof(CombinedRing)),
                    new CodeInstruction(OpCodes.Brfalse)
                )
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Ldarg_0)
                )
                .StripLabels(out var labels)
                .AddLabels(elseIfCombinedRing)
                .InsertWithLabels(
                    labels,
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld,
                        _NewForgeMenuType.RequireField("leftIngredientSpot")),
                    new CodeInstruction(OpCodes.Ldfld,
                        typeof(ClickableTextureComponent).RequireField(nameof(ClickableTextureComponent.item))),
                    new CodeInstruction(OpCodes.Isinst, typeof(Slingshot)),
                    new CodeInstruction(OpCodes.Stloc_S, slingshot),
                    new CodeInstruction(OpCodes.Ldloc_S, slingshot),
                    new CodeInstruction(OpCodes.Brfalse, elseIfCombinedRing),
                    new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.AllowSlingshotForges))),
                    new CodeInstruction(OpCodes.Brfalse, elseIfCombinedRing),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_S, slingshot),
                    new CodeInstruction(OpCodes.Call,
                        typeof(NewForgeMenuUpdatePatch).RequireMethod(nameof(UnforgeSlingshot)))
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

    internal static void UnforgeHolyBlade(IClickableMenu menu, MeleeWeapon holy)
    {
        var heroSoul = (SObject)ModEntry.DynamicGameAssetsApi!.SpawnDGAItem(ModEntry.Manifest.UniqueID + "/Hero Soul");
        heroSoul.Stack = 3;
        StardewValley.Utility.CollectOrDrop(heroSoul);
        SpaceCoreUtils.GetNewForgeMenuLeftIngredientSpot.Value(menu).item = null;
        Game1.playSound("coin");
    }

    internal static void UnforgeSlingshot(IClickableMenu menu, Slingshot slingshot)
    {
        var cost = 0;
        var forgeLevels = slingshot.GetTotalForgeLevels(true);
        for (var i = 0; i < forgeLevels; ++i)
            cost += SpaceCoreUtils.GetNewForgeMenuForgeCostAtLevel.Value(menu, i);

        if (slingshot.hasEnchantmentOfType<DiamondEnchantment>())
            cost += SpaceCoreUtils.GetNewForgeMenuForgeCost.Value(menu,
                SpaceCoreUtils.GetNewForgeMenuLeftIngredientSpot.Value(menu).item, new SObject(72, 1));

        for (var i = slingshot.enchantments.Count - 1; i >= 0; --i)
            if (slingshot.enchantments[i].IsForge())
                slingshot.RemoveEnchantment(slingshot.enchantments[i]);

        SpaceCoreUtils.GetNewForgeMenuLeftIngredientSpot.Value(menu).item = null;
        Game1.playSound("coin");
        SpaceCoreUtils.SetNewForgeMenuHeldItem.Value(menu, slingshot);
        StardewValley.Utility.CollectOrDrop(new SObject(848, cost / 2));
    }

    #endregion injected subroutines
}