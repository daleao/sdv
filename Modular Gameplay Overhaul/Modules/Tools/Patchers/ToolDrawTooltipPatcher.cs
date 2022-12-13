namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using System.Linq;
using System.Text;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolDrawTooltipPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolDrawTooltipPatcher"/> class.</summary>
    internal ToolDrawTooltipPatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.drawTooltip));
    }

    #region harmony patches

    /// <summary>Draw Scythe enchantment effects in tooltip.</summary>
    [HarmonyPostfix]
    private static void ToolDrawTooltipPostfix(
        MeleeWeapon __instance,
        SpriteBatch spriteBatch,
        ref int x,
        ref int y,
        SpriteFont font,
        float alpha,
        StringBuilder? overrideText)
    {
        if (__instance is not MeleeWeapon weapon || !weapon.isScythe() || weapon.enchantments.Count == 0)
        {
            return;
        }

        var co = new Color(120, 0, 210);
        foreach (var enchantment in __instance.enchantments.Where(enchantment => enchantment.ShouldBeDisplayed()))
        {
            Utility.drawWithShadow(
                spriteBatch,
                Game1.mouseCursors2,
                new Vector2(x + 20, y + 20),
                new Rectangle(127, 35, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                false,
                1f);
            Utility.drawTextWithShadow(
                spriteBatch,
                BaseEnchantment.hideEnchantmentName ? "???" : enchantment.GetDisplayName(),
                font,
                new Vector2(x + 68, y + 28),
                co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }
    }

    #endregion harmony patches
}
