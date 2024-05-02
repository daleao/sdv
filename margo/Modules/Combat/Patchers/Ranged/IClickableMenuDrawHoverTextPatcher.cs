namespace DaLion.Overhaul.Modules.Combat.Patchers.Ranged;

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

    /// <summary>Adds "Forged" text to Slingshots.</summary>
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
                    _ => helper.SetOperand(typeof(Tool)));
        }
        catch (Exception ex)
        {
            Log.E($"Failed generalizing enchantment tooltips to tools.\nHelper returned {ex}");
            return null;
        }

        // hack to fix price tag position in tooltip
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Item).RequireMethod(nameof(Item.attachmentSlots))),
                    },
                    ILHelper.SearchOption.Last)
                .Move(2)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4_S, 12),
                        new CodeInstruction(OpCodes.Add),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding buffer space between ammo slots and money required.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
