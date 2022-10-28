namespace DaLion.Redux.Rings.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Redux.Rings.Extensions;
using DaLion.Redux.Rings.VirtualProperties;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class RingDrawTooltipPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="RingDrawTooltipPatch"/> class.</summary>
    internal RingDrawTooltipPatch()
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

        // write description
        var descriptionWidth = ModEntry.Reflector
            .GetUnboundMethodDelegate<Func<Item, int>>(__instance, "getDescriptionWidth")
            .Invoke(__instance);
        Utility.drawTextWithShadow(
            spriteBatch,
            Game1.parseText(__instance.description, Game1.smallFont, descriptionWidth),
            font,
            new Vector2(x + 16, y + 20),
            Game1.textColor);
        y += (int)font.MeasureString(Game1.parseText(__instance.description, Game1.smallFont, descriptionWidth)).Y;

        if (combined.combinedRings.Count == 0)
        {
            return false; // don't run original logic
        }

        var buffer = combined.Get_StatBuffer();
        if (!buffer.Any())
        {
            return false; // don't run original logic
        }

        Color co;
        var maxWidth = 0;

        // write bonus damage
        if (buffer.DamageModifier > 0)
        {
            var amount = $"+{buffer.DamageModifier:0%}";
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

            var text = ModEntry.i18n.Get("ui.itemhover.damage", new { amount });
            var width = font.MeasureString(text).X;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(spriteBatch, text, font, new Vector2(x + 68, y + 28), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus crit rate
        if (buffer.CritChanceModifier > 0)
        {
            var amount = $"{buffer.CritChanceModifier:0%}";
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

            var text = Game1.content.LoadString("Strings\\UI:ItemHover_CritChanceBonus", amount);
            var width = font.MeasureString(text).X;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(spriteBatch, text, font, new Vector2(x + 68, y + 28), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write crit power
        if (buffer.CritPowerModifier > 0)
        {
            var amount = $"{buffer.CritPowerModifier:0%}";
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

            var text = Game1.content.LoadString("Strings\\UI:ItemHover_CritPowerBonus", amount);
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
        if (buffer.PrecisionModifier > 0)
        {
            var amount = $"+{buffer.PrecisionModifier:0%}";
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

            var text = ModEntry.i18n.Get("ui.itemhover.precision", new { amount });
            var width = font.MeasureString(text).X;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(spriteBatch, text, font, new Vector2(x + 68, y + 28), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus knockback
        if (buffer.KnockbackModifier > 0)
        {
            var amount = $"+{buffer.KnockbackModifier:0%}";
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

            var text = Game1.content.LoadString("Strings\\UI:ItemHover_Weight", amount);
            var width = font.MeasureString(text).X;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(spriteBatch, text, font, new Vector2(x + 68, y + 28), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus swing speed
        if (buffer.SwingSpeedModifier > 0)
        {
            var amount = $"+{buffer.SwingSpeedModifier:0%}";
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
        if (buffer.CooldownReduction > 0)
        {
            var amount = $"+{buffer.CooldownReduction:0%}";
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

            var text = ModEntry.i18n.Get("ui.itemhover.cdr", new { amount });
            var width = font.MeasureString(text).X;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(spriteBatch, text, font, new Vector2(x + 68, y + 28), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus defense
        if (buffer.AddedDefense > 0)
        {
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

            var text = Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", buffer.AddedDefense);
            var width = font.MeasureString(text).X;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(spriteBatch, text, font, new Vector2(x + 68, y + 28), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus magnetism
        if (buffer.AddedMagneticRadius > 0)
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

            var text = '+' + Game1.content.LoadString("Strings\\UI:ItemHover_Buff8", buffer.AddedMagneticRadius);
            var width = font.MeasureString(text).X;
            maxWidth = (int)Math.Max(width, maxWidth);
            Utility.drawTextWithShadow(spriteBatch, text, font, new Vector2(x + 68, y + 28), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        RingGetExtraSpaceNeededForTooltipSpecialIconsPatch.MaxWidth = maxWidth;
        return false; // don't run original logic
    }

    /// <summary>Fix Crab Ring tooltip.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? RingDrawTooltipTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        try
        {
            var displayVanillaEffect = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .FindFirst(new CodeInstruction(OpCodes.Ldc_I4_5))
                .AddLabels(displayVanillaEffect)
                .InsertInstructions(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Rings))),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Config).RequirePropertyGetter(nameof(Config.RebalancedRings))),
                    new CodeInstruction(OpCodes.Brfalse_S, displayVanillaEffect),
                    new CodeInstruction(OpCodes.Ldc_I4_S, 10),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution))
                .Advance()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting custom Crab Ring tooltip.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
