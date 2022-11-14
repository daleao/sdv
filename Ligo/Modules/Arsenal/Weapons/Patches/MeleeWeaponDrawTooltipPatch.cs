namespace DaLion.Ligo.Modules.Arsenal.Weapons.Patches;

#region using directives

using System.Linq;
using DaLion.Ligo.Modules.Arsenal.Common.Enchantments;
using DaLion.Ligo.Modules.Arsenal.Weapons.Extensions;
using DaLion.Shared.Extensions.Stardew;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDrawTooltipPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponDrawTooltipPatch"/> class.</summary>
    internal MeleeWeaponDrawTooltipPatch()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.drawTooltip));
    }

    #region harmony patches

    /// <summary>Make weapon stats human-readable..</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponDrawTooltipPrefix(
        MeleeWeapon __instance, SpriteBatch spriteBatch, ref int x, ref int y, SpriteFont font, float alpha)
    {
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
        if (__instance.isScythe(__instance.IndexOfMenuItemView))
        {
            return false; // don't run original logic
        }

        var co = Game1.textColor;

        // write damage
        if (__instance.hasEnchantmentOfType<RubyEnchantment>())
        {
            co = new Color(0, 120, 120);
        }

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
        Utility.drawTextWithShadow(
            spriteBatch,
            Game1.content.LoadString(
                "Strings\\UI:ItemHover_Damage",
                (int)(__instance.minDamage.Value * (1f + __instance.Read<float>(DataFields.ResonantDamage))),
                (int)(__instance.maxDamage.Value * (1f + __instance.Read<float>(DataFields.ResonantDamage)))),
            font,
            new Vector2(x + 68, y + 28),
            co * 0.9f * alpha);
        y += (int)Math.Max(font.MeasureString("TT").Y, 48f);

        var type = __instance.type.Value;

        // write bonus crit rate
        var crate = __instance.critChance.Value * (1f + __instance.Read<float>(DataFields.ResonantCritChance));
        var baseCrate = __instance.DefaultCritChance();
        if (crate > baseCrate)
        {
            co = __instance.hasEnchantmentOfType<AquamarineEnchantment>() ? new Color(0, 120, 120) : Game1.textColor;
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
            Utility.drawTextWithShadow(
                spriteBatch,
                ModEntry.i18n.Get(
                    "ui.itemhover.crate",
                    new { amount = (crate > baseCrate ? "+" : "-") + $"{crate - baseCrate:0%}" }),
                font,
                new Vector2(x + 68, y + 28),
                co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus crit power
        var cpow = __instance.critMultiplier.Value * (1f + __instance.Read<float>(DataFields.ResonantCritPower));
        var baseCpow = __instance.DefaultCritPower();
        if (cpow > baseCpow)
        {
            co = __instance.hasEnchantmentOfType<JadeEnchantment>() ? new Color(0, 120, 120) : Game1.textColor;
            Utility.drawWithShadow(
                spriteBatch,
                Game1.mouseCursors,
                new Vector2(x + 16, y + 20),
                new Rectangle(160, 428, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                false,
                1f);
            Utility.drawTextWithShadow(
                spriteBatch,
                ModEntry.i18n.Get(
                    "ui.itemhover.cpow",
                    new { amount = (cpow > baseCpow ? "+" : "-") + $"{cpow - baseCpow:0%}" }),
                font,
                new Vector2(x + 68, y + 28),
                co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus knockback
        var knockback = __instance.knockback.Value * (1f + __instance.Read<float>(DataFields.ResonantKnockback));
        var baseKnockback = __instance.defaultKnockBackForThisType(type);
        if (knockback != baseKnockback)
        {
            co = __instance.hasEnchantmentOfType<AmethystEnchantment>() ? new Color(0, 120, 120) : Game1.textColor;
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
            Utility.drawTextWithShadow(
                spriteBatch,
                ModEntry.i18n.Get(
                    "ui.itemhover.knockback",
                    new { amount = (knockback > baseKnockback ? "+" : "-") + $"{knockback - baseKnockback:0%}" }),
                font,
                new Vector2(x + 68, y + 28),
                co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus swing speed
        var speed = (__instance.speed.Value + __instance.Read<float>(DataFields.ResonantSpeed)) * 0.1f;
        if (speed != 0)
        {
            co = __instance.hasEnchantmentOfType<EmeraldEnchantment>() ? new Color(0, 120, 120) : Game1.textColor;
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
            Utility.drawTextWithShadow(
                spriteBatch,
                ModEntry.i18n.Get(
                    "ui.itemhover.swingspeed",
                    new { amount = (speed > 0 ? "+" : "-") + $"{speed:0%}" }),
                font,
                new Vector2(x + 68, y + 28),
                co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus cooldown reduction
        if (__instance.hasEnchantmentOfType<GarnetEnchantment>())
        {
            var cdr =
                $"-{(__instance.GetEnchantmentLevel<GarnetEnchantment>() + __instance.Read<float>(DataFields.ResonantCooldownReduction)) * 0.1f:0%}";
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
            Utility.drawTextWithShadow(
                spriteBatch,
                ModEntry.i18n.Get("ui.itemhover.cdr", new { amount = cdr }),
                font,
                new Vector2(x + 68, y + 28),
                co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus defense
        var defense = (__instance.addedDefense.Value + __instance.Read<float>(DataFields.ResonantDefense)) * 0.1f;
        if ((ModEntry.Config.EnableArsenal && ModEntry.Config.Arsenal.OverhauledDefense && defense != 0f) || defense > 1f)
        {
            co = __instance.hasEnchantmentOfType<TopazEnchantment>() ? new Color(0, 120, 120) : Game1.textColor;
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
            Utility.drawTextWithShadow(
                spriteBatch,
                ModEntry.Config.EnableArsenal && ModEntry.Config.Arsenal.OverhauledDefense
                    ? ModEntry.i18n.Get("ui.itemhover.resist", new { amount = (defense > 0 ? "+" : "-") + $"{defense:0%}" })
                    : Game1.content.LoadString("ItemHover_DefenseBonus", __instance.addedDefense.Value),
                font,
                new Vector2(x + 68, y + 28),
                co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus random forges
        if (__instance.enchantments.Count > 0 && __instance.enchantments[^1] is DiamondEnchantment)
        {
            co = new Color(0, 120, 120);
            var randomForges = __instance.GetMaxForges() - __instance.GetTotalForgeLevels();
            var randomForgeString = randomForges != 1
                ? Game1.content.LoadString("Strings\\UI:ItemHover_DiamondForge_Plural", randomForges)
                : Game1.content.LoadString("Strings\\UI:ItemHover_DiamondForge_Singular", randomForges);
            Utility.drawTextWithShadow(
                spriteBatch,
                randomForgeString,
                font,
                new Vector2(x + 16, y + 28),
                co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        co = new Color(120, 0, 210);

        // write other enchantments
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

        return false; // don't run original logic
    }

    #endregion harmony patches
}
