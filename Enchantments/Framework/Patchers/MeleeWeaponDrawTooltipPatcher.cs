namespace DaLion.Enchantments.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDrawTooltipPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponDrawTooltipPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal MeleeWeaponDrawTooltipPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.drawTooltip));
    }

    #region harmony patches

    /// <summary>Display enchanted Scythe tooltip.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void MeleeWeaponDrawTooltipPostfix(
        MeleeWeapon __instance,
        SpriteBatch spriteBatch,
        ref int x,
        ref int y,
        SpriteFont font,
        float alpha)
    {
        if (!__instance.isScythe() || __instance.enchantments.Count == 0)
        {
            return;
        }

        var co = new Color(120, 0, 210);
        foreach (var enchantment in __instance.enchantments)
        {
            if (!enchantment.ShouldBeDisplayed())
            {
                continue;
            }

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
