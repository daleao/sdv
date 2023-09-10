namespace DaLion.Overhaul.Modules.Combat.Patchers;

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

#endregion using directives

[UsedImplicitly]
internal sealed class IClickableMenuDrawHoverTextPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="IClickableMenuDrawHoverTextPatcher"/> class.</summary>
    internal IClickableMenuDrawHoverTextPatcher()
    {
        this.Target = this.RequireMethod<IClickableMenu>(
            nameof(IClickableMenu.drawHoverText),
            new[]
            {
                typeof(SpriteBatch), typeof(StringBuilder), typeof(SpriteFont), typeof(int), typeof(int),
                typeof(int), typeof(string), typeof(int), typeof(string[]), typeof(Item), typeof(int), typeof(int),
                typeof(int), typeof(int), typeof(int), typeof(float), typeof(CraftingRecipe), typeof(IList<Item>),
            });
        this.Transpiler!.after = new[] { OverhaulModule.Tools.Namespace };
    }

    #region harmony patches

    /// <summary>Adds "Forged" text to Slingshots.</summary>
    [HarmonyTranspiler]
    [HarmonyAfter("DaLion.Overhaul.Modules.Tools")]
    private static IEnumerable<CodeInstruction>? IClickableMenuDrawHoverTextTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2(x + 16, y + 16 + 4), Game1.textColor);
        // To: b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2(x + 16, y + 16 + 4), GetTitleColorFor(hoveredItem);
        try
        {
            helper
                .Match(// find second occurrence of `if (bold_title_subtext != null)`
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_0), // local 0 = string bold_title_subtext
                        new CodeInstruction(OpCodes.Brfalse_S),
                    },
                    ILHelper.SearchOption.Last)
                .Match(
                    new[] { new CodeInstruction(OpCodes.Ldsfld, typeof(Game1).RequireField(nameof(Game1.textColor))) },
                    ILHelper.SearchOption.Previous)
                .ReplaceWith(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Extensions.ItemExtensions).RequireMethod(nameof(Extensions.ItemExtensions.GetTitleColorFor))))
                .Insert(new[] { new CodeInstruction(OpCodes.Ldarg_S, (byte)9) }); // arg 9 = Item hoveredItem
        }
        catch (Exception ex)
        {
            Log.E($"Failed modifying hovered tool title color.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
