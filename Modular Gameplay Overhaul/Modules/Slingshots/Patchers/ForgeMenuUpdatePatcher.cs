namespace DaLion.Overhaul.Modules.Slingshots.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using StardewValley.Objects;
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

    /// <summary>Implement unforge behavior of Slingshot.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ForgeMenuUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: else if (leftIngredientSpot.item is Slingshot slingshot && SlingshotsModule.Config.EnableEnchantments)
        //     UnforgeSlingshot(leftIngredientSpot.item);
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
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(ForgeMenu).RequireField(nameof(ForgeMenu.leftIngredientSpot))),
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
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Slingshots))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Config).RequirePropertyGetter(nameof(Config.EnableEnchantments))),
                        new CodeInstruction(OpCodes.Brfalse, elseIfCombinedRing),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldloc_S, slingshot),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ForgeMenuUpdatePatcher).RequireMethod(nameof(UnforgeSlingshot))),
                    },
                    labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting unforge behavior of Slingshots.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void UnforgeSlingshot(ForgeMenu menu, Slingshot slingshot)
    {
        var cost = 0;
        var forgeLevels = slingshot.GetTotalForgeLevels(true);
        for (var i = 0; i < forgeLevels; i++)
        {
            cost += menu.GetForgeCostAtLevel(i);
        }

        if (slingshot.hasEnchantmentOfType<DiamondEnchantment>())
        {
            cost += menu.GetForgeCost(menu.leftIngredientSpot.item, new SObject(SObject.diamondIndex, 1));
        }

        for (var i = slingshot.enchantments.Count - 1; i >= 0; i--)
        {
            var enchantment = slingshot.enchantments[i];
            if (enchantment.IsForge())
            {
                slingshot.RemoveEnchantment(enchantment);
            }
        }

        menu.leftIngredientSpot.item = null;
        Game1.playSound("coin");
        menu.heldItem = slingshot;
        Utility.CollectOrDrop(new SObject(848, cost / 2));
    }

    #endregion injected subroutines
}
