namespace DaLion.Overhaul.Modules.Combat.Patchers.Ranged;

#region using directives

using System.Reflection;
using System.Text;
using DaLion.Overhaul.Modules.Combat.Enchantments;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Constants;
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
        this.Target = this.RequireMethod<Tool>(nameof(Tool.drawTooltip));
    }

    #region harmony patches

    /// <summary>Draw Slingshot enchantment effects in tooltip.</summary>
    [HarmonyPrefix]
    private static bool ToolDrawTooltipPrefix(
        Tool __instance,
        SpriteBatch spriteBatch,
        ref int x,
        ref int y,
        SpriteFont font,
        float alpha,
        StringBuilder? overrideText)
    {
        if (__instance is not Slingshot slingshot)
        {
            return true; // run original logic
        }

        try
        {
            // write description
            ItemDrawTooltipPatcher.ItemDrawTooltipReverse(
                slingshot,
                spriteBatch,
                ref x,
                ref y,
                font,
                alpha,
                overrideText);

            Color co;

            var bowData = ArcheryIntegration.Instance?.ModApi?.GetWeaponData(Manifest, slingshot);
            if (bowData is not null)
            {
                y += 12; // space out between special move description
            }

            // draw damage
            if (bowData is not null || __instance.attachments?[0] is not null)
            {
                var combinedDamage = (uint)slingshot.Get_DisplayedDamageModifier();
                var maxDamage = combinedDamage >> 16;
                var minDamage = combinedDamage & 0xFFFF;
                co = slingshot.Get_EffectiveDamageModifier() > 1f ? new Color(0, 120, 120) : Game1.textColor;

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
                        minDamage,
                        maxDamage),
                    font,
                    new Vector2(x + 68, y + 28),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }
            else if (__instance.InitialParentTileIndex != WeaponIds.BasicSlingshot)
            {
                co = Game1.textColor;

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
                    I18n.Ui_ItemHover_Damage($"+{slingshot.Get_DisplayedDamageModifier():#.#%}"),
                    font,
                    new Vector2(x + 68, y + 28),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            // draw knockback
            var knockback = slingshot.Get_DisplayedKnockback();
            if (knockback != 0f)
            {
                co = slingshot.Get_EffectiveKnockback() > 0 ? new Color(0, 120, 120) : Game1.textColor;

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
                    I18n.Ui_ItemHover_Knockback($"{knockback:+#.#%;-#.#%}"),
                    font,
                    new Vector2(x + 68, y + 28),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            // draw crit. chance
            var critChance = slingshot.Get_DisplayedCritChance();
            if (critChance != 0f)
            {
                co = slingshot.Get_EffectiveCritChance() > 0 ? new Color(0, 120, 120) : Game1.textColor;

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
                    I18n.Ui_ItemHover_Crate($"{critChance:+#.#%;-#.#%}"),
                    font,
                    new Vector2(x + 68, y + 28),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            // draw crit. damage
            var critPower = slingshot.Get_DisplayedCritPower();
            if (critPower != 0f)
            {
                co = slingshot.Get_EffectiveCritPower() > 0 ? new Color(0, 120, 120) : Game1.textColor;

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
                    I18n.Ui_ItemHover_Cpow($"{critPower:+#.#%;-#.#%}"),
                    font,
                    new Vector2(x + 68, y + 28),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            // draw fire speed
            var speedModifier = slingshot.Get_DisplayedFireSpeed();
            if (speedModifier > 0f)
            {
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

                Utility.drawTextWithShadow(
                    spriteBatch,
                    I18n.Ui_ItemHover_Firespeed($"{speedModifier:+#.#%;-#.#%}"),
                    font,
                    new Vector2(x + 68, y + 28),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            // draw cdr
            var cooldownModifier = slingshot.Get_DisplayedCooldownModifier();
            if (cooldownModifier > 0f)
            {
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
                    I18n.Ui_ItemHover_Cdr($"-{cooldownModifier:#.#%}"),
                    font,
                    new Vector2(x + 68, y + 28),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            // draw resilience
            var resilience = slingshot.Get_DisplayedResilience();
            if (resilience > 0f)
            {
                co = new Color(0, 120, 120);
                var amount = CombatModule.ShouldEnable && CombatModule.Config.NewResistanceFormula
                    ? $"+{resilience:#.#%}"
                    : $"{(int)(resilience * 100f)}";

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
                    CombatModule.ShouldEnable && CombatModule.Config.NewResistanceFormula
                        ? I18n.Ui_ItemHover_Resist(amount)
                        : Game1.content.LoadString("ItemHover_DefenseBonus", amount),
                    font,
                    new Vector2(x + 68, y + 28),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            // draw other enchantments
            co = new Color(120, 0, 210);
            for (var i = 0; i < __instance.enchantments.Count; i++)
            {
                var enchantment = __instance.enchantments[i];
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

            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches

    #region deprecated

    private static void DrawAsSlingshot(
        Slingshot slingshot,
        SpriteBatch spriteBatch,
        ref int x,
        ref int y,
        SpriteFont font,
        float alpha)
    {
        Color co;

        // write bonus damage
        var hasRubyEnchant = slingshot.hasEnchantmentOfType<RubyEnchantment>();
        if (slingshot.InitialParentTileIndex != WeaponIds.BasicSlingshot || hasRubyEnchant)
        {
            co = hasRubyEnchant ? new Color(0, 120, 120) : Game1.textColor;
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
                I18n.Ui_ItemHover_Damage($"+{slingshot.Get_DisplayedDamageModifier():#.#%}"),
                font,
                new Vector2(x + 68, y + 28),
                co * 0.9f * alpha);

            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus knockback
        var hasAmethystEnchant = slingshot.hasEnchantmentOfType<AmethystEnchantment>();
        if (slingshot.InitialParentTileIndex != WeaponIds.BasicSlingshot || hasAmethystEnchant)
        {
            co = hasAmethystEnchant ? new Color(0, 120, 120) : Game1.textColor;
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
                I18n.Ui_ItemHover_Knockback($"+{slingshot.Get_DisplayedKnockback():#.#%}"),
                font,
                new Vector2(x + 68, y + 28),
                co * 0.9f * alpha);

            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus crit rate
        if (slingshot.hasEnchantmentOfType<AquamarineEnchantment>())
        {
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

            Utility.drawTextWithShadow(
                spriteBatch,
                I18n.Ui_ItemHover_Crate($"{slingshot.Get_DisplayedCritChance():#.#%}"),
                font,
                new Vector2(x + 68, y + 28),
                co * 0.9f * alpha);

            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write crit power
        if (slingshot.hasEnchantmentOfType<JadeEnchantment>())
        {
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

            Utility.drawTextWithShadow(
                spriteBatch,
                I18n.Ui_ItemHover_Cpow($"{slingshot.Get_DisplayedCritPower():#.#%}"),
                font,
                new Vector2(x + 16 + 44, y + 16 + 12),
                co * 0.9f * alpha);

            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus fire speed
        if (slingshot.hasEnchantmentOfType<EmeraldEnchantment>())
        {
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

            Utility.drawTextWithShadow(
                spriteBatch,
                I18n.Ui_ItemHover_Firespeed($"+{slingshot.Get_DisplayedFireSpeed():#.#%}"),
                font,
                new Vector2(x + 68, y + 28),
                co * 0.9f * alpha);

            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus cooldown reduction
        if (slingshot.hasEnchantmentOfType<GarnetEnchantment>())
        {
            var amount = $"-{slingshot.Get_DisplayedCooldownModifier():#.#%}";
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
                I18n.Ui_ItemHover_Cdr(amount),
                font,
                new Vector2(x + 68, y + 28),
                co * 0.9f * alpha);

            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus defense
        if (slingshot.hasEnchantmentOfType<TopazEnchantment>() && CombatModule.Config.RebalancedGemstones)
        {
            var amount = CombatModule.ShouldEnable && CombatModule.Config.NewResistanceFormula
                ? $"+{slingshot.Get_DisplayedResilience():#.#%}"
                : slingshot.GetEnchantmentLevel<TopazEnchantment>().ToString();
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

            Utility.drawTextWithShadow(
                spriteBatch,
                CombatModule.ShouldEnable && CombatModule.Config.NewResistanceFormula
                    ? I18n.Ui_ItemHover_Resist(amount)
                    : Game1.content.LoadString("ItemHover_DefenseBonus", amount),
                font,
                new Vector2(x + 68, y + 28),
                co * 0.9f * alpha);

            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus random forge
        if (slingshot.enchantments.Count > 0 && slingshot.enchantments[^1] is DiamondEnchantment)
        {
            co = new Color(0, 120, 120);
            var randomForges = slingshot.GetMaxForges() - slingshot.GetTotalForgeLevels();
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

        // write other enchantments
        co = new Color(120, 0, 210);
        for (var i = 0; i < slingshot.enchantments.Count; i++)
        {
            var enchantment = slingshot.enchantments[i];
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

    #endregion deprecated
}
