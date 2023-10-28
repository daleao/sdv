namespace DaLion.Overhaul.Modules.Combat.Patchers.Melee;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Constants;
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
    internal MeleeWeaponDrawTooltipPatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.drawTooltip));
    }

    #region harmony patches

    /// <summary>Make weapon stats human-readable..</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponDrawTooltipPrefix(
        MeleeWeapon __instance, SpriteBatch spriteBatch, ref int x, ref int y, SpriteFont font, float alpha)
    {
        if (!CombatModule.Config.EnableWeaponOverhaul ||
            CombatModule.Config.WeaponTooltipStyle == CombatConfig.TooltipStyle.Vanilla || __instance.isScythe())
        {
            return true; // run original logic
        }

        try
        {
            #region description

            var descriptionWidth = Reflector
                .GetUnboundMethodDelegate<Func<Item, int>>(__instance, "getDescriptionWidth")
                .Invoke(__instance);
            Utility.drawTextWithShadow(
                spriteBatch,
                Game1.parseText(__instance.description, Game1.smallFont, descriptionWidth),
                font,
                new Vector2(x + 16f, y + 20f),
                Game1.textColor);
            y += (int)font.MeasureString(Game1.parseText(__instance.description, Game1.smallFont, descriptionWidth)).Y;

            #endregion description

            var co = Game1.textColor;

            #region damage

            if (__instance.hasEnchantmentOfType<RubyEnchantment>())
            {
                co = new Color(0, 120, 120);
            }

            spriteBatch.DrawAttackIcon(new Vector2(x + 20f, y + 20f));

            var text = Game1.content.LoadString(
                "Strings\\UI:ItemHover_Damage",
                __instance.Get_MinDamage(),
                __instance.Get_MaxDamage());
            Utility.drawTextWithShadow(
                spriteBatch,
                text,
                font,
                new Vector2(x + 68f, y + 28f),
                co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);

            #endregion damage

            #region knockback

            if (__instance.Get_DisplayedKnockback() is var knockback && knockback != 0)
            {
                co = __instance.hasEnchantmentOfType<AmethystEnchantment>() ? new Color(0, 120, 120) : Game1.textColor;
                spriteBatch.DrawWeightIcon(new Vector2(x + 20f, y + 20f));

                text = I18n.Ui_ItemHover_Knockback($"{knockback:+#.#%;-#.#%}");
                Utility.drawTextWithShadow(
                    spriteBatch,
                    text,
                    font,
                    new Vector2(x + 68f, y + 28f),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            #endregion knockback

            #region crit rate

            if (__instance.Get_DisplayedCritChance() is var critChance && critChance != 0)
            {
                co = __instance.hasEnchantmentOfType<AquamarineEnchantment>()
                    ? new Color(0, 120, 120)
                    : Game1.textColor;
                spriteBatch.DrawCritChanceIcon(new Vector2(x + 20f, y + 20f));

                text = Game1.parseText(I18n.Ui_ItemHover_CRate($"{critChance:+#.#%;-#.#%}"), Game1.smallFont, descriptionWidth);
                Utility.drawTextWithShadow(
                    spriteBatch,
                    text,
                    font,
                    new Vector2(x + 68f, y + 28f),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            #endregion crit rate

            #region crit power

            if (__instance.Get_DisplayedCritPower() is var critPower && critPower != 0)
            {
                co = __instance.hasEnchantmentOfType<JadeEnchantment>() ? new Color(0, 120, 120) : Game1.textColor;
                spriteBatch.DrawCritPowerIcon(new Vector2(x + 20f, y + 20f));

                text = I18n.Ui_ItemHover_CPow($"{critPower:+#.#%;-#.#%}");
                Utility.drawTextWithShadow(
                    spriteBatch,
                    text,
                    font,
                    new Vector2(x + 68f, y + 28f),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            #endregion crit power

            #region attack speed

            if (__instance.Get_DisplayedSwingSpeed() is var speed && speed != 0)
            {
                co = __instance.hasEnchantmentOfType<EmeraldEnchantment>() ? new Color(0, 120, 120) : Game1.textColor;
                spriteBatch.DrawSpeedIcon(new Vector2(x + 20f, y + 20f));

                text = Game1.parseText(I18n.Ui_ItemHover_SwingSpeed($"{speed:+#.#%;-#.#%}"), Game1.smallFont, descriptionWidth);
                Utility.drawTextWithShadow(
                    spriteBatch,
                    text,
                    font,
                    new Vector2(x + 68f, y + 28f),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            #endregion attack speed

            #region cooldown reduction

            if (__instance.Get_DisplayedCooldownReduction() is var cooldownReduction && cooldownReduction > 0)
            {
                co = new Color(0, 120, 120);
                spriteBatch.DrawCooldownIcon(new Vector2(x + 20f, y + 20f));

                text = I18n.Ui_ItemHover_Cdr($"-{cooldownReduction:#.#%}");
                Utility.drawTextWithShadow(
                    spriteBatch,
                    text,
                    font,
                    new Vector2(x + 68f, y + 28f),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            #endregion cooldown reduction

            #region resistance

            if (__instance.Get_DisplayedResilience() is var resistance && resistance != 0f)
            {
                co = __instance.hasEnchantmentOfType<TopazEnchantment>() ? new Color(0, 120, 120) : Game1.textColor;
                spriteBatch.DrawDefenseIcon(new Vector2(x + 20f, y + 20f));

                text = CombatModule.Config.NewResistanceFormula
                    ? I18n.Ui_ItemHover_Resist($"{resistance:+#.#%;-#.#%}")
                    : Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", __instance.addedDefense.Value)
                        .Replace("+", string.Empty);
                Utility.drawTextWithShadow(
                    spriteBatch,
                    text,
                    font,
                    new Vector2(x + 68f, y + 28f),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            #endregion resistance

            #region light emittance

            if (__instance.InitialParentTileIndex == WeaponIds.HolyBlade || __instance.IsInfinityWeapon())
            {
                co = __instance.IsInfinityWeapon()
                    ? Color.DeepPink
                    : Game1.textColor;
                spriteBatch.DrawLightIcon(new Vector2(x + 20f, y + 20f));

                text = I18n.Ui_ItemHover_Light();
                Utility.drawTextWithShadow(spriteBatch, text, font, new Vector2(x + 68f, y + 28f), co * 0.9f * alpha);
                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            #endregion light emittance

            #region random forges

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
                    new Vector2(x + 16f, y + 28f),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            #endregion random forges

            co = new Color(120, 0, 210);

            #region prismatic enchantments

            for (var i = 0; i < __instance.enchantments.Count; i++)
            {
                var enchantment = __instance.enchantments[i];
                if (!enchantment.ShouldBeDisplayed())
                {
                    continue;
                }

                spriteBatch.DrawEnchantmentIcon(new Vector2(x + 20f, y + 20f));

                text = BaseEnchantment.hideEnchantmentName ? "???" : enchantment.GetDisplayName();
                Utility.drawTextWithShadow(
                    spriteBatch,
                    text,
                    font,
                    new Vector2(x + 68f, y + 28f),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            #endregion prismatic enchantments

            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
