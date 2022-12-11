namespace DaLion.Ligo.Modules.Arsenal.Patchers.Integrations;

#region using directives

using System.Collections.Generic;
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

        // Injected: if (ArsenalModule.Config.TrulyLegendaryGalaxySword && weapon.hasEnchantmentOfType<HolyEnchantment>())
        //               UnforgeHolyBlade(weapon);
        //           else ...
        // After: if (weapon != null)
        try
        {
            var vanillaUnforge = generator.DefineLabel();
            helper
                .Match(
                    new[] { new CodeInstruction(OpCodes.Pop), new CodeInstruction(OpCodes.Br) },
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
                        new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Arsenal))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ArsenalConfig).RequirePropertyGetter(nameof(ArsenalConfig.InfinityPlusOne))),
                        new CodeInstruction(OpCodes.Brfalse_S, vanillaUnforge),
                        new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[10]), new CodeInstruction(
                            OpCodes.Call,
                            typeof(Tool)
                                .RequireMethod(nameof(Tool.hasEnchantmentOfType))
                                .MakeGenericMethod(typeof(BlessedEnchantment))),
                        new CodeInstruction(OpCodes.Brfalse_S, vanillaUnforge), new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldloc_3, helper.Locals[10]), new CodeInstruction(
                            OpCodes.Call,
                            typeof(NewForgeMenuUpdatePatcher).RequireMethod(nameof(UnforgeHolyBlade))),
                        new CodeInstruction(OpCodes.Br, resumeExecution),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed modifying unforge behavior of holy blade.\nHelper returned {ex}");
            return null;
        }

        // Injected: else if (leftIngredientSpot.item is Slingshot slingshot && ArsenalModule.Config.Slingshots.AllowForges)
        //             UnforgeSlingshot(leftIngredientSpot.item);
        // Between: MeleeWeapon and CombinedRing unforge behaviors...
        try
        {
            var elseIfCombinedRing = generator.DefineLabel();
            var slingshot = generator.DeclareLocal(typeof(Slingshot));
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Isinst, typeof(CombinedRing)),
                        new CodeInstruction(OpCodes.Brfalse),
                    },
                    ILHelper.SearchOption.First)
                .Match(new[] { new CodeInstruction(OpCodes.Ldarg_0) }, ILHelper.SearchOption.Previous)
                .StripLabels(out var labels)
                .AddLabels(elseIfCombinedRing)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(
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
                        new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Arsenal))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ArsenalConfig).RequirePropertyGetter(nameof(ArsenalConfig.Slingshots))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(SlingshotConfig).RequirePropertyGetter(nameof(SlingshotConfig.AllowForges))),
                        new CodeInstruction(OpCodes.Brfalse, elseIfCombinedRing), new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldloc_S, slingshot), new CodeInstruction(
                            OpCodes.Call,
                            typeof(NewForgeMenuUpdatePatcher).RequireMethod(nameof(UnforgeSlingshot))),
                    },
                    labels);
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

    private static void UnforgeHolyBlade(IClickableMenu menu, MeleeWeapon holy)
    {
        Utility.CollectOrDrop(new SObject(Globals.HeroSoulIndex!.Value, 1));
        Reflector
            .GetUnboundFieldGetter<IClickableMenu, ClickableTextureComponent>(menu, "leftIngredientSpot")
            .Invoke(menu).item = null;
        Game1.playSound("coin");
    }

    private static void UnforgeSlingshot(IClickableMenu menu, Slingshot slingshot)
    {
        var cost = 0;
        var forgeLevels = slingshot.GetTotalForgeLevels(true);
        for (var i = 0; i < forgeLevels; i++)
        {
            cost += Reflector
                .GetUnboundMethodDelegate<Func<IClickableMenu, int, int>>(menu, "GetForgeCostAtLevel")
                .Invoke(menu, i);
        }

        if (slingshot.hasEnchantmentOfType<DiamondEnchantment>())
        {
            var leftIngredientSpot = Reflector
                .GetUnboundFieldGetter<IClickableMenu, ClickableTextureComponent>(menu, "leftIngredientSpot")
                .Invoke(menu).item;
            cost += Reflector
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

        Reflector
            .GetUnboundFieldGetter<IClickableMenu, ClickableTextureComponent>(menu, "leftIngredientSpot")
            .Invoke(menu).item = null;
        Game1.playSound("coin");
        Reflector.GetUnboundFieldSetter<IClickableMenu, Item>(menu, "heldItem")
            .Invoke(menu, slingshot);
        Utility.CollectOrDrop(new SObject(848, cost / 2));
    }

    #endregion injected subroutines
}
