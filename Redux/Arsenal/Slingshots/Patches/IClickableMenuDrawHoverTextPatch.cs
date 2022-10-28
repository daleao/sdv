namespace DaLion.Redux.Arsenal.Slingshots.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class IClickableMenuDrawHoverTextPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="IClickableMenuDrawHoverTextPatch"/> class.</summary>
    internal IClickableMenuDrawHoverTextPatch()
    {
        this.Target = this.RequireMethod<IClickableMenu>(
            nameof(IClickableMenu.drawHoverText),
            new[]
            {
                typeof(SpriteBatch), typeof(StringBuilder), typeof(SpriteFont), typeof(int), typeof(int),
                typeof(int), typeof(string), typeof(int), typeof(string[]), typeof(Item), typeof(int), typeof(int),
                typeof(int), typeof(int), typeof(int), typeof(float), typeof(CraftingRecipe), typeof(IList<Item>),
            });
    }

    #region harmony patches

    /// <summary>Compensate Slingshot enchantment effects in tooltip.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? IClickableMenuDrawHoverTextPrefix(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // Injected: CompensateHeight(hoveredItem, ref height);
        // Before: drawTextureBox( ... );
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)9), // arg 9 = Item hoveredItem
                    new CodeInstruction(OpCodes.Brfalse))
                .Advance(2)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)9),
                    new CodeInstruction(OpCodes.Ldloc_2),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(IClickableMenuDrawHoverTextPatch).RequireMethod(nameof(CompensateHeight))),
                    new CodeInstruction(OpCodes.Stloc_2));
        }
        catch (Exception ex)
        {
            Log.E($"Failed to compensate Slingshot tooltip height.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static int CompensateHeight(Item hoveredItem, int height)
    {
        if (hoveredItem is not Slingshot slingshot)
        {
            return height;
        }

        if (slingshot.GetTotalForgeLevels() > 0)
        {
            height -= 12;
        }

        if (slingshot.GetTotalForgeLevels() > 1)
        {
            height -= 48;
        }

        if (slingshot.GetTotalForgeLevels() > 2)
        {
            height -= 48;
        }

        return height;
    }

    #endregion injected subroutines
}
