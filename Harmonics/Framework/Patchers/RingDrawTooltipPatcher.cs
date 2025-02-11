namespace DaLion.Harmonics.Framework.Patchers;

#region using directives

using DaLion.Core.Framework.Extensions;
using DaLion.Harmonics.Framework.VirtualProperties;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class RingDrawTooltipPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="RingDrawTooltipPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal RingDrawTooltipPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Ring>(nameof(Ring.drawTooltip));
    }

    #region harmony patches

    /// <summary>Draw combined Infinity Band tooltip.</summary>
    [HarmonyPrefix]
    private static bool RingDrawTooltipPrefix(
        Ring __instance, SpriteBatch spriteBatch, ref int x, ref int y, SpriteFont font, float alpha)
    {
        if (__instance is not CombinedRing combined || combined.ItemId != InfinityBandId)
        {
            return true; // run original logic
        }

        DrawForInfinityBand(combined, spriteBatch, x, ref y, font, alpha, out var maxWidth);
        RingGetExtraSpaceNeededForTooltipSpecialIconsPatcher.MinWidth = maxWidth;
        return false; // don't run original logic
    }

    #endregion harmony patches

    private static void DrawForInfinityBand(
        CombinedRing band, SpriteBatch b, int x, ref int y, SpriteFont font, float alpha, out int maxWidth)
    {
        #region non-combined

        var descriptionWidth = Reflector
            .GetUnboundMethodDelegate<Func<Item, int>>(band, "getDescriptionWidth")
            .Invoke(band);
        maxWidth = descriptionWidth;
        if (band.combinedRings.Count == 0)
        {
            Utility.drawTextWithShadow(
                b,
                Game1.parseText(band.description, Game1.smallFont, descriptionWidth),
                font,
                new Vector2(x + 16f, y + 20f),
                Game1.textColor);
            y += (int)font.MeasureString(Game1.parseText(band.description, Game1.smallFont, descriptionWidth)).Y;
            return;
        }

        #endregion non-combined

        #region resonance text

        var chord = band.Get_Chord();
        var root = chord?.Root;
        if (root is not null)
        {
            Utility.drawTextWithShadow(
                b,
                root.DisplayName + ' ' + I18n.Ui_Resonance(),
                font,
                new Vector2(x + 16f, y + 20f),
                root.TextColor,
                1f,
                -1f,
                2,
                2);
            y += (int)font.MeasureString("T").Y;
        }

        #endregion resonance text

        #region description text

        Utility.drawTextWithShadow(
            b,
            Game1.parseText(band.description, Game1.smallFont, descriptionWidth),
            font,
            new Vector2(x + 16f, y + 20f),
            Game1.textColor);
        y += (int)font.MeasureString(Game1.parseText(band.description, Game1.smallFont, descriptionWidth)).Y;

        #endregion description text

        var buffer = band.Get_StatBuffer();
        if (buffer.Any() != true)
        {
            return;
        }

        Color co;
        string text;
        float width;

        if (buffer.AttackMultiplier != 0f)
        {
            var amount = $"+{buffer.AttackMultiplier:#.#%}";
            co = new Color(0, 120, 120);
            b.DrawAttackIcon(new Vector2(x + 20f, y + 20f));
            text = I18n.Ui_ItemHover_Damage(amount);
            width = font.MeasureString(text).X + 88f;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(b, text, font, new Vector2(x + 68f, y + 28f), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        if (buffer.KnockbackMultiplier != 0f)
        {
            var amount = $"+{buffer.KnockbackMultiplier:#.#%}";
            co = new Color(0, 120, 120);
            b.DrawWeightIcon(new Vector2(x + 20f, y + 20f));
            text = I18n.Ui_ItemHover_Knockback(amount);
            width = font.MeasureString(text).X + 88f;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(b, text, font, new Vector2(x + 68f, y + 28f), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        if (buffer.CriticalChanceMultiplier != 0f)
        {
            var amount = $"+{buffer.CriticalChanceMultiplier:#.#%}";
            co = new Color(0, 120, 120);
            b.DrawCritChanceIcon(new Vector2(x + 20f, y + 20f));
            text = I18n.Ui_ItemHover_CRate(amount);
            width = font.MeasureString(text).X + 88f;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(b, text, font, new Vector2(x + 68f, y + 28f), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        if (buffer.CriticalPowerMultiplier != 0f)
        {
            var amount = $"+{buffer.CriticalPowerMultiplier:#.#%}";
            co = new Color(0, 120, 120);
            b.DrawCritPowerIcon(new Vector2(x + 20f, y + 20f));
            text = I18n.Ui_ItemHover_CPow(amount);
            width = font.MeasureString(text).X + 88f;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(b, text, font, new Vector2(x + 68f, y + 28f), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        if (buffer.WeaponSpeedMultiplier != 0f)
        {
            var amount = $"+{buffer.WeaponSpeedMultiplier:#.#%}";
            co = new Color(0, 120, 120);
            b.DrawSpeedIcon(new Vector2(x + 20f, y + 20f));
            text = I18n.Ui_ItemHover_AttackSpeed(amount);
            width = font.MeasureString(text).X + 88f;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(b, text, font, new Vector2(x + 68f, y + 28f), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        if (buffer.CooldownReduction != 0f)
        {
            var amount = $"-{buffer.CooldownReduction:#.#%}";
            co = new Color(0, 120, 120);
            b.DrawCooldownIcon(new Vector2(x + 20f, y + 20f));
            text = I18n.Ui_ItemHover_Cdr(amount);
            width = font.MeasureString(text).X + 88f;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(b, text, font, new Vector2(x + 68f, y + 28f), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        if (buffer.Defense != 0f)
        {
            var amount = $"+{buffer.Defense:#.#}";
            co = new Color(0, 120, 120);
            b.DrawDefenseIcon(new Vector2(x + 20f, y + 20f));
            text = I18n.Ui_ItemHover_Resist(amount);
            width = font.MeasureString(text).X + 88f;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(b, text, font, new Vector2(x + 68f, y + 28f), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        if (buffer.MagneticRadius > 0)
        {
            co = Game1.textColor;
            b.DrawMagnetismIcon(new Vector2(x + 20f, y + 20f));
            text = I18n.Ui_ItemHover_Magnetic();
            width = font.MeasureString(text).X + 88f;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(b, text, font, new Vector2(x + 68f, y + 28f), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        //if (root is null)
        //{
        //    return;
        //}

        //co = root.TextColor;
        //b.DrawLightIcon(new Vector2(x + 20f, y + 20f));
        //text = I18n.Ui_ItemHover_Light();
        //width = font.MeasureString(text).X + 88f;
        //maxWidth = (int)Math.Max(width, maxWidth);
        //Utility.drawTextWithShadow(b, text, font, new Vector2(x + 68f, y + 28f), co * 0.9f * alpha);
        //y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
    }
}
