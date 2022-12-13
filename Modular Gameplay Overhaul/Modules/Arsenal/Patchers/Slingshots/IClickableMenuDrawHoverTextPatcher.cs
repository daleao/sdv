namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Slingshots;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley.Tools;

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
    }

    #region harmony patches

    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? IClickableMenuDrawHoverTextTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .ForEach(
                    new[] { new CodeInstruction(OpCodes.Isinst, typeof(MeleeWeapon)) },
                    () => helper.SetOperand(typeof(Tool)));
        }
        catch (Exception ex)
        {
            Log.E($"Failed generalizing enchantment tooltips to tools.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
