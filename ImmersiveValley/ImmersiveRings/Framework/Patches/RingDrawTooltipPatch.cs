namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using Common;
using Common.Extensions.Collections;
using Common.Extensions.Reflection;
using Common.Harmony;
using Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class RingDrawTooltipPatch : Common.Harmony.HarmonyPatch
{
    private static readonly Lazy<Func<Item, int>> _GetDescriptionWidth = new(() =>
        typeof(Item).RequireMethod("getDescriptionWidth").CompileUnboundDelegate<Func<Item, int>>());

    /// <summary>Construct an instance.</summary>
    internal RingDrawTooltipPatch()
    {
        Target = RequireMethod<Ring>(nameof(Ring.drawTooltip));
    }

    #region harmony patches

    /// <summary>Draw combined Iridium Band effects in tooltip.</summary>
    [HarmonyPrefix]
    private static bool RingDrawTooltipPrefix(Ring __instance, SpriteBatch spriteBatch, ref int x, ref int y,
        SpriteFont font, float alpha)
    {
        if (!__instance.IsCombinedIridiumBand(out var combined)) return true; // run original logic

        var modifiers = new StatModifiers();
        combined.combinedRings.ForEach(r => Gemstone.FromRing(r.ParentSheetIndex).ApplyModifier(ref modifiers));
        if (!modifiers.Any()) return false; // don't run original logic

        combined.get_Resonances().ForEach(r => r.Key.ApplyToModifiers(ref modifiers, r.Value));

        // write description
        var descriptionWidth = _GetDescriptionWidth.Value(__instance);
        StardewValley.Utility.drawTextWithShadow(spriteBatch,
            Game1.parseText(__instance.description, Game1.smallFont, descriptionWidth), font, new(x + 16, y + 20),
            Game1.textColor);
        y += (int)font.MeasureString(Game1.parseText(__instance.description, Game1.smallFont, descriptionWidth)).Y;

        Color co;
        var maxWidth = 0;

        // write bonus damage
        if (modifiers.DamageModifier > 0)
        {
            var amount = $"{modifiers.DamageModifier:p0}";
            co = new(0, 120, 120);
            StardewValley.Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new(x + 20, y + 20), new(120, 428, 10, 10), Color.White,
                0f, Vector2.Zero, 4f, false, 1f);

            var text = ModEntry.i18n.Get("ui.itemhover.damage", new { amount });
            var width = font.MeasureString(text).X;
            maxWidth = (int)Math.Max(width, maxWidth);
            StardewValley.Utility.drawTextWithShadow(spriteBatch, text, font, new(x + 68, y + 28), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus crit rate
        if (modifiers.CritChanceModifier > 0)
        {
            var amount = $"{modifiers.CritChanceModifier:p0}";
            co = new(0, 120, 120);
            StardewValley.Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new(x + 20, y + 20), new(40, 428, 10, 10),
                Color.White, 0f, Vector2.Zero, 4f, false, 1f);

            var text = Game1.content.LoadString("Strings\\UI:ItemHover_CritChanceBonus", amount);
            var width = font.MeasureString(text).X;
            maxWidth = (int)Math.Max(width, maxWidth);
            StardewValley.Utility.drawTextWithShadow(spriteBatch, text, font, new(x + 68, y + 28), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write crit power
        if (modifiers.CritPowerModifier > 0)
        {
            var amount = $"{modifiers.CritPowerModifier:p0}";
            co = new(0, 120, 120);
            StardewValley.Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new(x + 16, y + 16 + 4),
                new(160, 428, 10, 10), Color.White, 0f, Vector2.Zero, 4f, false, 1f);

            var text = Game1.content.LoadString("Strings\\UI:ItemHover_CritPowerBonus", amount);
            var width = font.MeasureString(text).X;
            maxWidth = (int)Math.Max(width, maxWidth);
            StardewValley.Utility.drawTextWithShadow(spriteBatch, text, font, new(x + 16 + 44, y + 16 + 12), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus precision
        if (modifiers.PrecisionModifier > 0)
        {
            var amount = $"{modifiers.PrecisionModifier:p0}";
            co = new(0, 120, 120);
            StardewValley.Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new(x + 20, y + 20), new(110, 428, 10, 10),
                Color.White, 0f, Vector2.Zero, 4f, false, 1f);

            var text = ModEntry.i18n.Get("ui.itemhover.precision", new { amount });
            var width = font.MeasureString(text).X;
            maxWidth = (int)Math.Max(width, maxWidth);
            StardewValley.Utility.drawTextWithShadow(spriteBatch, text, font, new(x + 68, y + 28), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus knockback
        if (modifiers.KnockbackModifier > 0)
        {
            var amount = $"+{modifiers.KnockbackModifier:p0}";
            co = new(0, 120, 120);
            StardewValley.Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new(x + 20, y + 20), new(70, 428, 10, 10),
                Color.White, 0f, Vector2.Zero, 4f, false, 1f);

            var text = Game1.content.LoadString("Strings\\UI:ItemHover_Weight", amount);
            var width = font.MeasureString(text).X;
            maxWidth = (int)Math.Max(width, maxWidth);
            StardewValley.Utility.drawTextWithShadow(spriteBatch, text, font, new(x + 68, y + 28), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus swing speed
        if (modifiers.SwingSpeedModifier > 0)
        {
            var amount = $"+{modifiers.SwingSpeedModifier:p0}";
            co = new(0, 120, 120);
            StardewValley.Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new(x + 20, y + 20), new(130, 428, 10, 10),
                Color.White, 0f, Vector2.Zero, 4f, false, 1f);

            var text = Game1.content.LoadString("Strings\\UI:ItemHover_Speed", amount);
            var width = font.MeasureString(text).X;
            maxWidth = (int)Math.Max(width, maxWidth);
            StardewValley.Utility.drawTextWithShadow(spriteBatch, text, font, new(x + 68, y + 28), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus cooldown reduction
        if (modifiers.CooldownReduction > 0)
        {
            var amount = $"{modifiers.CooldownReduction:p0}";
            co = new(0, 120, 120);
            StardewValley.Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new(x + 20, y + 20), new(150, 428, 10, 10),
                Color.White, 0f, Vector2.Zero, 4f, false, 1f);

            var text = ModEntry.i18n.Get("ui.itemhover.cdr", new { amount });
            var width = font.MeasureString(text).X;
            maxWidth = (int)Math.Max(width, maxWidth);
            StardewValley.Utility.drawTextWithShadow(spriteBatch, text, font, new(x + 68, y + 28), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus defense
        if (modifiers.AddedDefense > 0)
        {
            co = new(0, 120, 120);
            StardewValley.Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new(x + 20, y + 20), new(110, 428, 10, 10),
                Color.White, 0f, Vector2.Zero, 4f, false, 1f);

            var text = Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", modifiers.AddedDefense);
            var width = font.MeasureString(text).X;
            maxWidth = (int)Math.Max(width, maxWidth);
            StardewValley.Utility.drawTextWithShadow(spriteBatch, text, font, new(x + 68, y + 28), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        RingGetExtraSpaceNeededForTooltipSpecialIconsPatch.MaxWidth = maxWidth;
        return false; // don't run original logic
    }

    /// <summary>Fix crab ring tooltip.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? RingDrawTooltipTranspiler(IEnumerable<CodeInstruction> instructions,
        ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        var displayVanillaEffect = generator.DefineLabel();
        var resumeExecution = generator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldc_I4_5)
                )
                .AddLabels(displayVanillaEffect)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.RebalancedRings))),
                    new CodeInstruction(OpCodes.Brfalse_S, displayVanillaEffect),
                    new CodeInstruction(OpCodes.Ldc_I4_S, 10),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                )
                .Advance()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting custom crabshell tooltip.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}