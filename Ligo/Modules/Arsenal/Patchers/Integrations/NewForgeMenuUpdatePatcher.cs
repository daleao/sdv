namespace DaLion.Ligo.Modules.Arsenal.Patchers;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Arsenal.Configs;
using DaLion.Ligo.Modules.Arsenal.Enchantments;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using SpaceCore.Interface;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[RequiresMod("spacechase0.SpaceCore")]
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

        // Injected: if (ModEntry.Config.Arsenal.TrulyLegendaryGalaxySword && weapon.hasEnchantmentOfType<HolyEnchantment>())
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
                        OpCodes.Callvirt,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Arsenal))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Config).RequirePropertyGetter(nameof(Config.InfinityPlusOne))),
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
                        typeof(NewForgeMenuUpdatePatcher).RequireMethod(nameof(UnforgeHolyBlade))),
                    new CodeInstruction(OpCodes.Br, resumeExecution));
        }
        catch (Exception ex)
        {
            Log.E($"Failed modifying unforge behavior of holy blade.\nHelper returned {ex}");
            return null;
        }

        // Injected: else if (leftIngredientSpot.item is Slingshot slingshot && ModEntry.Config.Arsenal.Slingshots.AllowForges)
        //             UnforgeSlingshot(leftIngredientSpot.item);
        // Between: MeleeWeapon and CombinedRing unforge behaviors...
        try
        {
            var elseIfCombinedRing = generator.DefineLabel();
            var slingshot = generator.DeclareLocal(typeof(Slingshot));
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Isinst, typeof(CombinedRing)),
                    new CodeInstruction(OpCodes.Brfalse))
                .RetreatUntil(new CodeInstruction(OpCodes.Ldarg_0))
                .StripLabels(out var labels)
                .AddLabels(elseIfCombinedRing)
                .InsertWithLabels(
                    labels,
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Ldfld,
                        "SpaceCore.Interface.NewForgeMenu"
                            .ToType()
                            .RequireField("leftIngredientSpot")),
                    new CodeInstruction(
                        OpCodes.Ldfld,
                        typeof(ClickableTextureComponent).RequireField(nameof(ClickableTextureComponent.item))),
                    new CodeInstruction(OpCodes.Isinst, typeof(Slingshot)),
                    new CodeInstruction(OpCodes.Stloc_S, slingshot),
                    new CodeInstruction(OpCodes.Ldloc_S, slingshot),
                    new CodeInstruction(OpCodes.Brfalse, elseIfCombinedRing),
                    new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Arsenal))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Config).RequirePropertyGetter(nameof(Config.Slingshots))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(SlingshotConfig).RequirePropertyGetter(nameof(SlingshotConfig.AllowForges))),
                    new CodeInstruction(OpCodes.Brfalse, elseIfCombinedRing),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_S, slingshot),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(NewForgeMenuUpdatePatcher).RequireMethod(nameof(UnforgeSlingshot))));
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
    internal static void UnforgeHolyBlade(IClickableMenu menu, MeleeWeapon holy)
    {
        Utility.CollectOrDrop(new SObject(Globals.HeroSoulindex!.Value, 1));
        ModEntry.Reflector
            .GetUnboundFieldGetter<IClickableMenu, ClickableTextureComponent>(menu, "leftIngredientSpot")
            .Invoke(menu).item = null;
        Game1.playSound("coin");
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:Elements should be ordered by access", Justification = "Preference for patch classes with injected subroutines.")]
    internal static void UnforgeSlingshot(IClickableMenu menu, Slingshot slingshot)
    {
        var cost = 0;
        var forgeLevels = slingshot.GetTotalForgeLevels(true);
        for (var i = 0; i < forgeLevels; i++)
        {
            cost += ModEntry.Reflector
                .GetUnboundMethodDelegate<Func<IClickableMenu, int, int>>(menu, "GetForgeCostAtLevel")
                .Invoke(menu, i);
        }

        if (slingshot.hasEnchantmentOfType<DiamondEnchantment>())
        {
            var leftIngredientSpot = ModEntry.Reflector
                .GetUnboundFieldGetter<IClickableMenu, ClickableTextureComponent>(menu, "leftIngredientSpot")
                .Invoke(menu).item;
            cost += ModEntry.Reflector
                .GetUnboundMethodDelegate<Func<IClickableMenu, Item, Item, int>>(menu, "GetForgeCost")
                .Invoke(
                    menu,
                    leftIngredientSpot,
                    new SObject(72, 1));
        }

        for (var i = slingshot.enchantments.Count - 1; i >= 0; i--)
        {
            if (slingshot.enchantments[i].IsForge())
            {
                slingshot.RemoveEnchantment(slingshot.enchantments[i]);
            }
        }

        ModEntry.Reflector
            .GetUnboundFieldGetter<IClickableMenu, ClickableTextureComponent>(menu, "leftIngredientSpot")
            .Invoke(menu).item = null;
        Game1.playSound("coin");
        ModEntry.Reflector.GetUnboundFieldSetter<IClickableMenu, Item>(menu, "heldItem")
            .Invoke(menu, slingshot);
        Utility.CollectOrDrop(new SObject(848, cost / 2));
    }

    #endregion injected subroutines

}
