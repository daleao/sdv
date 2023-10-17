namespace DaLion.Overhaul.Modules.Combat.Patchers.Rings;

#region using directives

using System.Collections.Generic;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class RingDrawTooltipPatcher : HarmonyPatcher
{
    private static HashSet<int>? _ids;

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
        if (__instance.IsCombinedInfinityBand(out var band))
        {
            DrawForInfinityBand(band, spriteBatch, x, ref y, font, alpha, out var maxWidth1);
            RingGetExtraSpaceNeededForTooltipSpecialIconsPatcher.MinWidth = maxWidth1;
            return false; // don't run original logic
        }

        if (!CombatModule.Config.RebalancedRings)
        {
            return true; // run original logic
        }

        if (_ids is null)
        {
            _ids = new HashSet<int>
            {
                ObjectIds.RubyRing,
                ObjectIds.AquamarineRing,
                ObjectIds.AmethystRing,
                ObjectIds.EmeraldRing,
                ObjectIds.JadeRing,
                ObjectIds.TopazRing,
                ObjectIds.SmallGlowRing,
                ObjectIds.GlowRing,
                ObjectIds.SmallMagnetRing,
                ObjectIds.MagnetRing,
                ObjectIds.GlowstoneRing,
                ObjectIds.CrabshellRing,
                ObjectIds.ImmunityRing,
            };

            if (JsonAssetsIntegration.GarnetRingIndex.HasValue)
            {
                _ids.Add(JsonAssetsIntegration.GarnetRingIndex.Value);
            }
        }

        if (__instance is CombinedRing combined)
        {
            for (var i = 0; i < combined.combinedRings.Count; i++)
            {
                var ring = combined.combinedRings[i];
                DrawForOther(ring, spriteBatch, x, ref y, font, alpha, combined.DisplayName);
            }

            return false; // don't run original logic
        }

        if (_ids.Contains(__instance.indexInTileSheet.Value))
        {
            DrawForOther(__instance, spriteBatch, x, ref y, font, alpha);
            return false; // don't run original logic
        }

        return true; // run original logic
    }

    #endregion harmony patches

    #region for infinity band

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

        #region resonance

        var root = band.Get_Chord()?.Root;
        if (root is not null)
        {
            Utility.drawTextWithShadow(
                b,
                root.DisplayName + ' ' + I18n.Resonance(),
                font,
                new Vector2(x + 16f, y + 20f),
                root.TextColor,
                1f,
                -1f,
                2,
                2);
            y += (int)font.MeasureString("T").Y;
        }

        #endregion resonance

        #region description

        Utility.drawTextWithShadow(
            b,
            Game1.parseText(band.description, Game1.smallFont, descriptionWidth),
            font,
            new Vector2(x + 16f, y + 20f),
            Game1.textColor);
        y += (int)font.MeasureString(Game1.parseText(band.description, Game1.smallFont, descriptionWidth)).Y;

        var buffer = band.Get_StatBuffer();
        if (!buffer.Any())
        {
            return;
        }

        #endregion description

        Color co;

        #region damage

        if (buffer.DamageModifier != 0)
        {
            var amount = $"+{buffer.DamageModifier:#.#%}";
            co = new Color(0, 120, 120);
            Utility.drawWithShadow(
                b,
                Game1.mouseCursors,
                new Vector2(x + 20f, y + 20f),
                new Rectangle(120, 428, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                false,
                1f);

            var text = I18n.Ui_ItemHover_Damage(amount);
            var width = font.MeasureString(text).X + 48f;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(b, text, font, new Vector2(x + 68f, y + 28f), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        #endregion damage

        #region knockback

        if (buffer.KnockbackModifier != 0)
        {
            var amount = $"+{buffer.KnockbackModifier:#.#%}";
            co = new Color(0, 120, 120);
            Utility.drawWithShadow(
                b,
                Game1.mouseCursors,
                new Vector2(x + 20f, y + 20f),
                new Rectangle(70, 428, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                false,
                1f);

            var text = I18n.Ui_ItemHover_Knockback(amount);
            var width = font.MeasureString(text).X + 48f;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(b, text, font, new Vector2(x + 68f, y + 28f), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        #endregion knockback

        #region crit chance

        if (buffer.CritChanceModifier != 0)
        {
            var amount = $"+{buffer.CritChanceModifier:#.#%}";
            co = new Color(0, 120, 120);
            Utility.drawWithShadow(
                b,
                Game1.mouseCursors,
                new Vector2(x + 20f, y + 20f),
                new Rectangle(40, 428, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                false,
                1f);

            var text = I18n.Ui_ItemHover_CRate(amount);
            var width = font.MeasureString(text).X + 48f;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(b, text, font, new Vector2(x + 68f, y + 28f), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        #endregion crit chance

        #region crit power

        if (buffer.CritPowerModifier != 0)
        {
            var amount = $"+{buffer.CritPowerModifier:#.#%}";
            co = new Color(0, 120, 120);
            Utility.drawWithShadow(
                b,
                Game1.mouseCursors,
                new Vector2(x + 20f, y + 20f),
                new Rectangle(160, 428, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                false,
                1f);

            var text = I18n.Ui_ItemHover_CPow(amount);
            var width = font.MeasureString(text).X + 48f;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(
                b,
                text,
                font,
                new Vector2(x + 68f, y + 28f),
                co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        #endregion crit power

        #region precision

        if (buffer.PrecisionModifier != 0)
        {
            var amount = $"+{buffer.PrecisionModifier:#.#%}";
            co = new Color(0, 120, 120);
            Utility.drawWithShadow(
                b,
                Game1.mouseCursors,
                new Vector2(x + 20f, y + 20f),
                new Rectangle(110, 428, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                false,
                1f);

            var text = I18n.Ui_ItemHover_Precision(amount);
            var width = font.MeasureString(text).X + 48f;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(b, text, font, new Vector2(x + 68f, y + 28f), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        #endregion precision

        #region speed

        if (buffer.SwingSpeedModifier != 0)
        {
            var amount = $"+{buffer.SwingSpeedModifier:#.#%}";
            co = new Color(0, 120, 120);
            Utility.drawWithShadow(
                b,
                Game1.mouseCursors,
                new Vector2(x + 20f, y + 20f),
                new Rectangle(130, 428, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                false,
                1f);

            var text = Game1.content.LoadString("Strings\\UI:ItemHover_Speed", amount);
            var width = font.MeasureString(text).X + 48f;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(b, text, font, new Vector2(x + 68f, y + 28f), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        #endregion speed

        #region cooldown reduction

        if (buffer.CooldownReduction != 0)
        {
            var amount = $"-{buffer.CooldownReduction:#.#%}";
            co = new Color(0, 120, 120);
            Utility.drawWithShadow(
                b,
                Textures.TooltipsTx,
                new Vector2(x + 20f, y + 20f),
                new Rectangle(10, 0, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                false,
                1f);

            var text = I18n.Ui_ItemHover_Cdr(amount);
            var width = font.MeasureString(text).X + 48f;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(b, text, font, new Vector2(x + 68f, y + 28f), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        #endregion cooldown reduction

        #region resistance

        if (buffer.DefenseModifier != 0)
        {
            var amount = $"+{buffer.DefenseModifier:#.#%}";
            co = new Color(0, 120, 120);
            Utility.drawWithShadow(
                b,
                Game1.mouseCursors,
                new Vector2(x + 20f, y + 20f),
                new Rectangle(110, 428, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                false,
                1f);

            var text = CombatModule.Config.NewResistanceFormula
                ? I18n.Ui_ItemHover_Resist(amount)
                : Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", buffer.DefenseModifier);
            var width = font.MeasureString(text).X + 48f;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(b, text, font, new Vector2(x + 68f, y + 28f), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        #endregion resistance

        #region magnetism

        if (buffer.MagneticRadius > 0)
        {
            co = Game1.textColor;
            Utility.drawWithShadow(
                b,
                Game1.mouseCursors,
                new Vector2(x + 20f, y + 20f),
                new Rectangle(90, 428, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                false,
                1f);

            var text = I18n.Ui_ItemHover_Magnetic();
            var width = font.MeasureString(text).X + 48f;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(b, text, font, new Vector2(x + 68f, y + 28f), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        #endregion magnetism

        #region light emittance

        if (root is not null)
        {
            co = root.TextColor;
            Utility.drawWithShadow(
                b,
                Textures.TooltipsTx,
                new Vector2(x + 20f, y + 20f),
                new Rectangle(0, 0, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                false,
                1f);

            var text = I18n.Ui_ItemHover_Light();
            var width = font.MeasureString(text).X + 48f;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(b, text, font, new Vector2(x + 68f, y + 28f), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        #endregion light emittance
    }

    #endregion for infinity band

    #region for other

    private static void DrawForOther(Ring ring, SpriteBatch b, int x, ref int y, SpriteFont font, float alpha, string? name = null)
    {
        name ??= ring.DisplayName;
        var descriptionSize = font.MeasureString(ring.getDescription());
        var titleWidth = font.MeasureString(name).X;
        descriptionSize.X = Math.Max(descriptionSize.X - 48f, titleWidth);

        string parsedDescription;
        float fontHeight, iconOffset, textOffset;
        int numLines;
        switch (ring.indexInTileSheet.Value)
        {
            case ObjectIds.RubyRing:
                parsedDescription = Game1.parseText(ring.description, Game1.smallFont, (int)descriptionSize.X);
                fontHeight = font.MeasureString(parsedDescription).Y;
                descriptionSize.Y = Math.Max(fontHeight, 48f);
                iconOffset = Math.Min(descriptionSize.Y / 2f, 30);
                b.DrawAttackIcon(new Vector2(x + 20f, y + iconOffset));

                numLines = parsedDescription.Split(Environment.NewLine).Length;
                textOffset = numLines == 1 ? (48f - fontHeight) / 2f : 0f;
                Utility.drawTextWithShadow(b, parsedDescription, font, new Vector2(x + 68f, y + textOffset + 20f), Game1.textColor * 0.9f * alpha);
                y += (int)descriptionSize.Y;
                break;

            case ObjectIds.AquamarineRing:
                parsedDescription = Game1.parseText(ring.description, Game1.smallFont, (int)descriptionSize.X);
                fontHeight = font.MeasureString(parsedDescription).Y;
                descriptionSize.Y = Math.Max(fontHeight, 48f);
                iconOffset = Math.Min(descriptionSize.Y / 2f, 30);
                b.DrawCritChanceIcon(new Vector2(x + 20f, y + iconOffset));

                numLines = parsedDescription.Split(Environment.NewLine).Length;
                textOffset = numLines == 1 ? (48f - fontHeight) / 2f : 0f;
                Utility.drawTextWithShadow(b, parsedDescription, font, new Vector2(x + 68f, y + textOffset + 20f), Game1.textColor * 0.9f * alpha);
                y += (int)descriptionSize.Y;
                break;

            case ObjectIds.AmethystRing:
                parsedDescription = Game1.parseText(ring.description, Game1.smallFont, (int)descriptionSize.X);
                fontHeight = font.MeasureString(parsedDescription).Y;
                descriptionSize.Y = Math.Max(fontHeight, 48f);
                iconOffset = Math.Min(descriptionSize.Y / 2f, 30);
                b.DrawWeightIcon(new Vector2(x + 20, y + iconOffset));

                numLines = parsedDescription.Split(Environment.NewLine).Length;
                textOffset = numLines == 1 ? (48f - fontHeight) / 2f : 0f;
                Utility.drawTextWithShadow(b, parsedDescription, font, new Vector2(x + 68f, y + textOffset + 20f), Game1.textColor * 0.9f * alpha);
                y += (int)descriptionSize.Y;
                break;

            case ObjectIds.EmeraldRing:
                parsedDescription = Game1.parseText(ring.description, Game1.smallFont, (int)descriptionSize.X);
                fontHeight = font.MeasureString(parsedDescription).Y;
                descriptionSize.Y = Math.Max(fontHeight, 48f);
                iconOffset = Math.Min(descriptionSize.Y / 2f, 30);
                b.DrawSpeedIcon(new Vector2(x + 20, y + iconOffset));

                numLines = parsedDescription.Split(Environment.NewLine).Length;
                textOffset = numLines == 1 ? (48f - fontHeight) / 2f : 0f;
                Utility.drawTextWithShadow(b, parsedDescription, font, new Vector2(x + 68f, y + textOffset + 20f), Game1.textColor * 0.9f * alpha);
                y += (int)descriptionSize.Y;
                break;

            case ObjectIds.JadeRing:
                parsedDescription = Game1.parseText(ring.description, Game1.smallFont, (int)descriptionSize.X);
                fontHeight = font.MeasureString(parsedDescription).Y;
                descriptionSize.Y = Math.Max(fontHeight, 48f);
                iconOffset = Math.Min(descriptionSize.Y / 2f, 30);
                b.DrawCritPowerIcon(new Vector2(x + 20, y + iconOffset));

                numLines = parsedDescription.Split(Environment.NewLine).Length;
                textOffset = numLines == 1 ? (48f - fontHeight) / 2f : 0f;
                Utility.drawTextWithShadow(b, parsedDescription, font, new Vector2(x + 68f, y + textOffset + 20f), Game1.textColor * 0.9f * alpha);
                y += (int)descriptionSize.Y;
                break;

            case ObjectIds.TopazRing:
                parsedDescription = Game1.parseText(ring.description, Game1.smallFont, (int)descriptionSize.X);
                fontHeight = font.MeasureString(parsedDescription).Y;
                descriptionSize.Y = Math.Max(fontHeight, 48f);
                iconOffset = Math.Min(descriptionSize.Y / 2f, 30);
                b.DrawDefenseIcon(new Vector2(x + 20, y + iconOffset));

                numLines = parsedDescription.Split(Environment.NewLine).Length;
                textOffset = numLines == 1 ? (48f - fontHeight) / 2f : 0f;
                Utility.drawTextWithShadow(b, parsedDescription, font, new Vector2(x + 68f, y + textOffset + 20f), Game1.textColor * 0.9f * alpha);
                y += (int)descriptionSize.Y;
                break;

            case ObjectIds.SmallGlowRing:
            case ObjectIds.GlowRing:
                parsedDescription = Game1.parseText(ring.description, Game1.smallFont, (int)descriptionSize.X);
                fontHeight = font.MeasureString(parsedDescription).Y;
                descriptionSize.Y = Math.Max(fontHeight, 48f);
                iconOffset = Math.Min(descriptionSize.Y / 2f, 30);
                b.DrawLightIcon(new Vector2(x + 20, y + iconOffset));

                numLines = parsedDescription.Split(Environment.NewLine).Length;
                textOffset = numLines == 1 ? (48f - fontHeight) / 2f : 0f;
                Utility.drawTextWithShadow(b, parsedDescription, font, new Vector2(x + 68f, y + textOffset + 20), Game1.textColor * 0.9f * alpha);
                y += (int)descriptionSize.Y;
                break;

            case ObjectIds.SmallMagnetRing:
            case ObjectIds.MagnetRing:
                parsedDescription = Game1.parseText(ring.description, Game1.smallFont, (int)descriptionSize.X);
                fontHeight = font.MeasureString(parsedDescription).Y;
                descriptionSize.Y = Math.Max(fontHeight, 48f);
                iconOffset = Math.Min(descriptionSize.Y / 2f, 30);
                b.DrawMagnetismIcon(new Vector2(x + 20, y + iconOffset));

                numLines = parsedDescription.Split(Environment.NewLine).Length;
                textOffset = numLines == 1 ? (48f - fontHeight) / 2f : 0f;
                Utility.drawTextWithShadow(b, parsedDescription, font, new Vector2(x + 68f, y + textOffset + 20f), Game1.textColor * 0.9f * alpha);
                y += (int)descriptionSize.Y;
                break;

            case ObjectIds.GlowstoneRing:
                if (Game1.objectInformation is null)
                {
                    break;
                }

                parsedDescription = Game1.parseText(
                    Game1.objectInformation[ObjectIds.GlowRing].Split('/')[5],
                    Game1.smallFont,
                    (int)descriptionSize.X);
                fontHeight = font.MeasureString(parsedDescription).Y;
                descriptionSize.Y = Math.Max(fontHeight, 48f);
                iconOffset = Math.Min(descriptionSize.Y / 2f, 30);
                b.DrawLightIcon(new Vector2(x + 20, y + iconOffset));

                numLines = parsedDescription.Split(Environment.NewLine).Length;
                textOffset = numLines == 1 ? (48f - fontHeight) / 2f : 0f;
                Utility.drawTextWithShadow(b, parsedDescription, font, new Vector2(x + 68f, y + textOffset + 20), Game1.textColor * 0.9f * alpha);
                y += (int)descriptionSize.Y;

                parsedDescription = Game1.parseText(
                    Game1.objectInformation[ObjectIds.MagnetRing].Split('/')[5],
                    Game1.smallFont,
                    (int)descriptionSize.X);
                fontHeight = font.MeasureString(parsedDescription).Y;
                descriptionSize.Y = Math.Max(fontHeight, 48f);
                iconOffset = Math.Min(descriptionSize.Y / 2f, 30);
                b.DrawMagnetismIcon(new Vector2(x + 20, y + iconOffset));

                numLines = parsedDescription.Split(Environment.NewLine).Length;
                textOffset = numLines == 1 ? (48f - fontHeight) / 2f : 0f;
                Utility.drawTextWithShadow(b, parsedDescription, font, new Vector2(x + 68f, y + textOffset + 20f), Game1.textColor * 0.9f * alpha);
                y += (int)descriptionSize.Y;
                break;

            case ObjectIds.CrabshellRing:
                parsedDescription = Game1.parseText(ring.description, Game1.smallFont, (int)descriptionSize.X);
                Utility.drawTextWithShadow(b, parsedDescription, font, new Vector2(x + 16, y + 16 + 4), Game1.textColor);
                fontHeight = font.MeasureString(parsedDescription).Y;
                descriptionSize.Y = Math.Max(fontHeight, 48f);
                y += (int)fontHeight;
                b.DrawDefenseIcon(new Vector2(x + 20f, y + 20f));

                parsedDescription = Game1.parseText(
                    CombatModule.Config.NewResistanceFormula
                        ? I18n.Ui_ItemHover_Resist("+5")
                        : Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", 5),
                    Game1.smallFont,
                    (int)descriptionSize.X);
                fontHeight = font.MeasureString(parsedDescription).Y;
                numLines = parsedDescription.Split(Environment.NewLine).Length;
                textOffset = numLines == 1 ? (48f - fontHeight) / 2f : 0f;
                Utility.drawTextWithShadow(b, parsedDescription, font, new Vector2(x + 68f, y + textOffset + 20f), Game1.textColor * 0.9f * alpha);
                descriptionSize.Y += Math.Max(fontHeight, 48f);
                y += (int)descriptionSize.Y;
                break;

            case ObjectIds.ImmunityRing:
                parsedDescription = Game1.parseText(ring.description, Game1.smallFont, (int)descriptionSize.X);
                Utility.drawTextWithShadow(b, parsedDescription, font, new Vector2(x + 16, y + 16 + 4), Game1.textColor);
                fontHeight = font.MeasureString(parsedDescription).Y;
                descriptionSize.Y = Math.Max(fontHeight, 48f);
                y += (int)fontHeight;
                b.DrawImmunityIcon(new Vector2(x + 20f, y + 20f));

                parsedDescription = Game1.parseText(
                    Game1.content.LoadString("Strings\\UI:ItemHover_ImmunityBonus", 10),
                    Game1.smallFont,
                    (int)descriptionSize.X);
                fontHeight = font.MeasureString(parsedDescription).Y;
                numLines = parsedDescription.Split(Environment.NewLine).Length;
                textOffset = numLines == 1 ? (48f - fontHeight) / 2f : 0f;
                Utility.drawTextWithShadow(b, parsedDescription, font, new Vector2(x + 68f, y + textOffset + 20f), Game1.textColor * 0.9f * alpha);
                descriptionSize.Y += Math.Max(fontHeight, 48f);
                y += (int)descriptionSize.Y;
                break;

            default:
                parsedDescription = Game1.parseText(ring.description, Game1.smallFont, (int)descriptionSize.X);
                fontHeight = font.MeasureString(parsedDescription).Y;
                descriptionSize.Y = Math.Max(fontHeight, 48f);
                iconOffset = Math.Min(descriptionSize.Y / 2f, 30);
                if (JsonAssetsIntegration.GarnetRingIndex.HasValue &&
                    ring.indexInTileSheet.Value == JsonAssetsIntegration.GarnetRingIndex.Value)
                {
                    b.DrawCooldownIcon(new Vector2(x + 20f, y + iconOffset));
                    numLines = parsedDescription.Split(Environment.NewLine).Length;
                    textOffset = numLines == 1 ? (48f - fontHeight) / 2f : 0f;
                    Utility.drawTextWithShadow(
                        b,
                        parsedDescription,
                        font,
                        new Vector2(x + 68f, y + textOffset + 20f),
                        Game1.textColor * 0.9f * alpha);
                    y += (int)descriptionSize.Y;
                    break;
                }

                Utility.drawTextWithShadow(
                    b,
                    parsedDescription,
                    font,
                    new Vector2(x + 16f, y + 20f),
                    Game1.textColor);
                y += (int)descriptionSize.Y;
                break;
        }
    }

    #endregion for other
}
