namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using Common.Extensions.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDrawTooltipPatch : Common.Harmony.HarmonyPatch
{
    private static Func<Item, int>? _GetDescriptionWidth;

    /// <summary>Construct an instance.</summary>
    internal MeleeWeaponDrawTooltipPatch()
    {
        Target = RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.drawTooltip));
    }

    #region harmony patches

    /// <summary>Make weapon stats human-readable..</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponDrawTooltipPrefix(MeleeWeapon __instance, SpriteBatch spriteBatch, ref int x,
        ref int y, SpriteFont font, float alpha)
    {
        _GetDescriptionWidth ??=
            typeof(Item).RequireMethod("getDescriptionWidth").CompileUnboundDelegate<Func<Item, int>>();

        // write description
        var descriptionWidth = _GetDescriptionWidth(__instance);
        Utility.drawTextWithShadow(spriteBatch,
            Game1.parseText(__instance.description, Game1.smallFont, descriptionWidth), font, new(x + 16, y + 20),
            Game1.textColor);
        y += (int)font.MeasureString(Game1.parseText(__instance.description, Game1.smallFont, descriptionWidth)).Y;
        if (__instance.isScythe(__instance.IndexOfMenuItemView)) return false; // don't run original logic

        var co = Game1.textColor;

        // write damage
        if (__instance.hasEnchantmentOfType<RubyEnchantment>()) co = new(0, 120, 120);

        Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new(x + 20, y + 20), new(120, 428, 10, 10), Color.White,
            0f, Vector2.Zero, 4f, false, 1f);
        Utility.drawTextWithShadow(spriteBatch,
            Game1.content.LoadString("Strings\\UI:ItemHover_Damage", __instance.minDamage.Value,
                __instance.maxDamage.Value), font, new(x + 68, y + 28), co * 0.9f * alpha);
        y += (int)Math.Max(font.MeasureString("TT").Y, 48f);

        // write bonus knockback
        if (Math.Abs(__instance.knockback.Value - __instance.defaultKnockBackForThisType(__instance.type.Value)) >
            0.01f)
        {
            co = Game1.textColor;
            if (__instance.hasEnchantmentOfType<AmethystEnchantment>()) co = new(0, 120, 120);

            Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new(x + 20, y + 20), new(70, 428, 10, 10),
                Color.White, 0f, Vector2.Zero, 4f, false, 1f);
            Utility.drawTextWithShadow(spriteBatch,
                Game1.content.LoadString("Strings\\UI:ItemHover_Weight",
                    (((int)Math.Ceiling(Math.Abs(__instance.knockback.Value -
                                                 __instance.defaultKnockBackForThisType(__instance.type.Value)) *
                                        10f) > __instance.defaultKnockBackForThisType(__instance.type.Value))
                        ? "+"
                        : "") + (int)Math.Ceiling(Math.Abs(__instance.knockback.Value -
                                                           __instance.defaultKnockBackForThisType(
                                                               __instance.type.Value)) * 10f)), font,
                new(x + 68, y + 28), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus crit rate
        var effectiveCritChance = __instance.critChance.Value;
        if (__instance.type.Value == 1)
        {
            effectiveCritChance += 0.005f;
            effectiveCritChance *= 1.12f;
        }

        if (effectiveCritChance / 0.02 >= 1.1000000238418579)
        {
            co = Game1.textColor;
            if (__instance.hasEnchantmentOfType<AquamarineEnchantment>()) co = new(0, 120, 120);

            Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new(x + 20, y + 20), new(40, 428, 10, 10),
                Color.White, 0f, Vector2.Zero, 4f, false, 1f);
            Utility.drawTextWithShadow(spriteBatch,
                Game1.content.LoadString("Strings\\UI:ItemHover_CritChanceBonus",
                    (int)Math.Round((effectiveCritChance - 0.001f) / 0.02)), font, new(x + 68, y + 28),
                co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write crit power
        if ((__instance.critMultiplier.Value - 3f) / 0.02 >= 1.0)
        {
            co = Game1.textColor;
            if (__instance.hasEnchantmentOfType<JadeEnchantment>()) co = new(0, 120, 120);

            Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new(x + 16, y + 20), new(160, 428, 10, 10),
                Color.White, 0f, Vector2.Zero, 4f, false, 1f);
            Utility.drawTextWithShadow(spriteBatch,
                Game1.content.LoadString("Strings\\UI:ItemHover_CritPowerBonus",
                    (int)((__instance.critMultiplier.Value - 3f) / 0.02)), font, new(x + 204, y + 28),
                co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus swing speed
        if (__instance.speed.Value != (__instance.type.Value == 2 ? -8 : 0))
        {
            var amount = __instance.type.Value == 2 ? __instance.speed.Value + 8 : __instance.speed.Value;
            var negativeSpeed = __instance.type.Value == 2 && __instance.speed.Value < -8 ||
                                __instance.type.Value != 2 && __instance.speed.Value < 0;
            co = Game1.textColor;
            if (__instance.hasEnchantmentOfType<EmeraldEnchantment>()) co = new(0, 120, 120);

            Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new(x + 20, y + 20), new(130, 428, 10, 10),
                Color.White, 0f, Vector2.Zero, 4f, false, 1f);
            Utility.drawTextWithShadow(spriteBatch,
                Game1.content.LoadString("Strings\\UI:ItemHover_Speed", (amount > 0 ? "+" : "") + amount / 2), font,
                new(x + 68, y + 28), negativeSpeed ? Color.DarkRed : co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus cooldown reduction
        if (__instance.hasEnchantmentOfType<TopazEnchantment>() && ModEntry.Config.TopazPerk == ModConfig.Perk.Cooldown)
        {
            var cdr = __instance.GetEnchantmentLevel<TopazEnchantment>() * 0.1f;
            var amount = $"{cdr:p0}";
            co = new(0, 120, 120);
            Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new(x + 20, y + 20), new(150, 428, 10, 10),
                Color.White, 0f, Vector2.Zero, 4f, false, 1f);
            Utility.drawTextWithShadow(spriteBatch, ModEntry.i18n.Get("ui.itemhover.cdr", new { amount }), font,
                new(x + 68, y + 28), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus defense
        if (__instance.addedDefense.Value > 0)
        {
            co = Game1.textColor;
            if (__instance.hasEnchantmentOfType<TopazEnchantment>() &&
                ModEntry.Config.TopazPerk == ModConfig.Perk.Defense) co = new(0, 120, 120);

            Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new(x + 20, y + 20), new(110, 428, 10, 10),
                Color.White, 0f, Vector2.Zero, 4f, false, 1f);
            Utility.drawTextWithShadow(spriteBatch,
                Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", __instance.addedDefense.Value), font,
                new(x + 68, y + 28), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus random forges
        if (__instance.enchantments.Count > 0 && __instance.enchantments[^1] is DiamondEnchantment)
        {
            co = new(0, 120, 120);
            var randomForges = __instance.GetMaxForges() - __instance.GetTotalForgeLevels();
            var randomForgeString = randomForges != 1
                ? Game1.content.LoadString("Strings\\UI:ItemHover_DiamondForge_Plural", randomForges)
                : Game1.content.LoadString("Strings\\UI:ItemHover_DiamondForge_Singular", randomForges);
            Utility.drawTextWithShadow(spriteBatch, randomForgeString, font, new(x + 16, y + 28), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        co = new(120, 0, 210);
        // write other enchantments
        foreach (var enchantment in __instance.enchantments.Where(enchantment => enchantment.ShouldBeDisplayed()))
        {
            Utility.drawWithShadow(spriteBatch, Game1.mouseCursors2, new(x + 20, y + 20), new(127, 35, 10, 10),
                Color.White, 0f, Vector2.Zero, 4f, false, 1f);
            Utility.drawTextWithShadow(spriteBatch,
                BaseEnchantment.hideEnchantmentName ? "???" : enchantment.GetDisplayName(), font, new(x + 68, y + 28),
                co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}