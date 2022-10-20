namespace DaLion.Stardew.Slingshots.Framework.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Common.Attributes;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using DaLion.Common.Integrations.SpaceCore;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using StardewValley.Objects;
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

    internal static void UnforgeSlingshot(IClickableMenu menu, Slingshot slingshot)
    {
        var cost = 0;
        var forgeLevels = slingshot.GetTotalForgeLevels(true);
        for (var i = 0; i < forgeLevels; ++i)
        {
            cost += ExtendedSpaceCoreApi.GetNewForgeMenuForgeCostAtLevel.Value(menu, i);
        }

        if (slingshot.hasEnchantmentOfType<DiamondEnchantment>())
        {
            cost += ExtendedSpaceCoreApi.GetNewForgeMenuForgeCost.Value(
                menu,
                ExtendedSpaceCoreApi.GetNewForgeMenuLeftIngredientSpot.Value(menu).item,
                new SObject(72, 1));
        }

        for (var i = slingshot.enchantments.Count - 1; i >= 0; --i)
        {
            if (slingshot.enchantments[i].IsForge())
            {
                slingshot.RemoveEnchantment(slingshot.enchantments[i]);
            }
        }

        ExtendedSpaceCoreApi.GetNewForgeMenuLeftIngredientSpot.Value(menu).item = null;
        Game1.playSound("coin");
        ExtendedSpaceCoreApi.SetNewForgeMenuHeldItem.Value(menu, slingshot);
        Utility.CollectOrDrop(new SObject(848, cost / 2));
    }

    #endregion injected subroutines

    #region harmony patches

    /// <summary>Allow unforge Slingshot.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ForgeMenuUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // Injected: else if (leftIngredientSpot.item is Slingshot slingshot && ModEntry.Config.EnableSlingshotForges)
        //             UnforgeSlingshot(leftIngredientSpot.item);
        // Between: MeleeWeapon and CombinedRing unforge behaviors...
        try
        {
            var elseIfCombinedRing = generator.DefineLabel();
            var slingshot = generator.DeclareLocal(typeof(Slingshot));
            helper
                .FindNext(
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
                        OpCodes.Call,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.EnableSlingshotForges))),
                    new CodeInstruction(OpCodes.Brfalse, elseIfCombinedRing),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_S, slingshot),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(NewForgeMenuUpdatePatch).RequireMethod(nameof(UnforgeSlingshot))));
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
