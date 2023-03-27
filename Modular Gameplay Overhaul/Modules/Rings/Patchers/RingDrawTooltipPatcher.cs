namespace DaLion.Overhaul.Modules.Rings.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Rings.Extensions;
using DaLion.Overhaul.Modules.Rings.VirtualProperties;
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
    internal RingDrawTooltipPatcher()
    {
        this.Target = this.RequireMethod<Ring>(nameof(Ring.drawTooltip));
    }

    #region harmony patches

    /// <summary>Draw combined Infinity Band effects in tooltip.</summary>
    [HarmonyPrefix]
    private static bool RingDrawTooltipPrefix(
        Ring __instance, SpriteBatch spriteBatch, ref int x, ref int y, SpriteFont font, float alpha)
    {
        if (!__instance.IsCombinedInfinityBand(out var combined))
        {
            return true; // run original logic
        }

        var descriptionWidth = Reflector
            .GetUnboundMethodDelegate<Func<Item, int>>(__instance, "getDescriptionWidth")
            .Invoke(__instance);
        if (combined.combinedRings.Count == 0)
        {
            // write description
            Utility.drawTextWithShadow(
                spriteBatch,
                Game1.parseText(__instance.description, Game1.smallFont, descriptionWidth),
                font,
                new Vector2(x + 16, y + 20),
                Game1.textColor);
            y += (int)font.MeasureString(Game1.parseText(__instance.description, Game1.smallFont, descriptionWidth)).Y;

            return false; // don't run original logic
        }

        // write resonance
        var root = combined.Get_Chord()?.Root;
        if (root is not null)
        {
            Utility.drawTextWithShadow(
                spriteBatch,
                root.DisplayName + ' ' + I18n.Get("resonance"),
                font,
                new Vector2(x + 16, y + 16 + 4),
                root.TextColor,
                1f,
                -1f,
                2,
                2);
            y += (int)font.MeasureString("T").Y;
        }

        // write description
        Utility.drawTextWithShadow(
            spriteBatch,
            Game1.parseText(__instance.description, Game1.smallFont, descriptionWidth),
            font,
            new Vector2(x + 16, y + 20),
            Game1.textColor);
        y += (int)font.MeasureString(Game1.parseText(__instance.description, Game1.smallFont, descriptionWidth)).Y;

        var buffer = combined.Get_StatBuffer();
        if (!buffer.Any())
        {
            return false; // don't run original logic
        }

        Color co;
        var maxWidth = 0;

        // write bonus damage
        if (buffer.DamageModifier != 0)
        {
            var amount = $"+{buffer.DamageModifier:0.0%}";
            co = new Color(0, 120, 120);
            Utility.drawWithShadow(
                spriteBatch,
                Game1.mouseCursors,
                new Vector2(x + 20, y + 20),
                new Rectangle(120, 428, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                false,
                1f);

            string text = I18n.Get("ui.itemhover.damage", new { amount });
            var width = font.MeasureString(text).X;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(spriteBatch, text, font, new Vector2(x + 68, y + 28), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus knockback
        if (buffer.KnockbackModifier != 0)
        {
            var amount = $"+{buffer.KnockbackModifier:0.0%}";
            co = new Color(0, 120, 120);
            Utility.drawWithShadow(
                spriteBatch,
                Game1.mouseCursors,
                new Vector2(x + 20, y + 20),
                new Rectangle(70, 428, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                false,
                1f);

            string text = I18n.Get("ui.itemhover.knockback", new { amount });
            var width = font.MeasureString(text).X;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(spriteBatch, text, font, new Vector2(x + 68, y + 28), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus crit rate
        if (buffer.CritChanceModifier != 0)
        {
            var amount = $"+{buffer.CritChanceModifier:0.0%}";
            co = new Color(0, 120, 120);
            Utility.drawWithShadow(
                spriteBatch,
                Game1.mouseCursors,
                new Vector2(x + 20, y + 20),
                new Rectangle(40, 428, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                false,
                1f);

            string text = I18n.Get("ui.itemhover.crate", new { amount });
            var width = font.MeasureString(text).X;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(spriteBatch, text, font, new Vector2(x + 68, y + 28), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write crit power
        if (buffer.CritPowerModifier != 0)
        {
            var amount = $"+{buffer.CritPowerModifier:0.0%}";
            co = new Color(0, 120, 120);
            Utility.drawWithShadow(
                spriteBatch,
                Game1.mouseCursors,
                new Vector2(x + 16, y + 16 + 4),
                new Rectangle(160, 428, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                false,
                1f);

            string text = I18n.Get("ui.itemhover.cpow", new { amount });
            var width = font.MeasureString(text).X;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(
                spriteBatch,
                text,
                font,
                new Vector2(x + 16 + 44, y + 16 + 12),
                co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus precision
        if (buffer.PrecisionModifier != 0)
        {
            var amount = $"+{buffer.PrecisionModifier:0.0%}";
            co = new Color(0, 120, 120);
            Utility.drawWithShadow(
                spriteBatch,
                Game1.mouseCursors,
                new Vector2(x + 20, y + 20),
                new Rectangle(110, 428, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                false,
                1f);

            string text = I18n.Get("ui.itemhover.precision", new { amount });
            var width = font.MeasureString(text).X;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(spriteBatch, text, font, new Vector2(x + 68, y + 28), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus speed
        if (buffer.SwingSpeedModifier != 0)
        {
            var amount = $"+{buffer.SwingSpeedModifier:0.0%}";
            co = new Color(0, 120, 120);
            Utility.drawWithShadow(
                spriteBatch,
                Game1.mouseCursors,
                new Vector2(x + 20, y + 20),
                new Rectangle(130, 428, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                false,
                1f);

            var text = Game1.content.LoadString("Strings\\UI:ItemHover_Speed", amount);
            var width = font.MeasureString(text).X;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(spriteBatch, text, font, new Vector2(x + 68, y + 28), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus cooldown reduction
        if (buffer.CooldownReduction != 0)
        {
            var amount = $"-{buffer.CooldownReduction:0.0%}";
            co = new Color(0, 120, 120);
            Utility.drawWithShadow(
                spriteBatch,
                Game1.mouseCursors,
                new Vector2(x + 20, y + 20),
                new Rectangle(150, 428, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                false,
                1f);

            string text = I18n.Get("ui.itemhover.cdr", new { amount });
            var width = font.MeasureString(text).X;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(spriteBatch, text, font, new Vector2(x + 68, y + 28), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus defense
        if (buffer.DefenseModifier != 0)
        {
            var amount = $"+{buffer.DefenseModifier:0.0%}";
            co = new Color(0, 120, 120);
            Utility.drawWithShadow(
                spriteBatch,
                Game1.mouseCursors,
                new Vector2(x + 20, y + 20),
                new Rectangle(110, 428, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                false,
                1f);

            var text = CombatModule.IsEnabled && CombatModule.Config.OverhauledDefense
                ? I18n.Get("ui.itemhover.resist", new { amount })
                : Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", buffer.DefenseModifier);
            var width = font.MeasureString(text).X;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(spriteBatch, text, font, new Vector2(x + 68, y + 28), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus magnetism
        if (buffer.MagneticRadius > 0)
        {
            co = new Color(0, 120, 120);
            Utility.drawWithShadow(
                spriteBatch,
                Game1.mouseCursors,
                new Vector2(x + 20, y + 20),
                new Rectangle(90, 428, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                false,
                1f);

            var text = '+' + Game1.content.LoadString("Strings\\UI:ItemHover_Buff8", buffer.MagneticRadius);
            var width = font.MeasureString(text).X;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(spriteBatch, text, font, new Vector2(x + 68, y + 28), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        RingGetExtraSpaceNeededForTooltipSpecialIconsPatcher.MaxWidth = maxWidth;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
