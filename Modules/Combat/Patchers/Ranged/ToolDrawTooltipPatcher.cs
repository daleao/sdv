namespace DaLion.Overhaul.Modules.Combat.Patchers.Ranged;

#region using directives

using System.Reflection;
using System.Text;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
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
            #region description

            ItemDrawTooltipPatcher.ItemDrawTooltipReverse(
                slingshot,
                spriteBatch,
                ref x,
                ref y,
                font,
                alpha,
                overrideText);

            var bowData = ArcheryIntegration.Instance?.ModApi?.GetWeaponData(Manifest, slingshot);
            if (bowData is not null)
            {
                y += 12; // space out between special move description
            }

            #endregion description

            Color co;

            #region damage

            if (bowData is not null || __instance.attachments?[0] is not null)
            {
                var combinedDamage = (uint)slingshot.Get_DisplayedDamageModifier();
                var maxDamage = combinedDamage >> 16;
                var minDamage = combinedDamage & 0xFFFF;
                co = slingshot.HasRubyBonus() ? new Color(0, 120, 120) : Game1.textColor;
                spriteBatch.DrawAttackIcon(new Vector2(x + 20f, y + 20f));
                Utility.drawTextWithShadow(
                    spriteBatch,
                    Game1.content.LoadString(
                        "Strings\\UI:ItemHover_Damage",
                        minDamage,
                        maxDamage),
                    font,
                    new Vector2(x + 68f, y + 28f),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }
            else if (slingshot.Get_DisplayedDamageModifier() is var damageMod && damageMod != 0)
            {
                co = slingshot.HasRubyBonus() ? new Color(0, 120, 120) : Game1.textColor;
                spriteBatch.DrawAttackIcon(new Vector2(x + 20f, y + 20f));
                Utility.drawTextWithShadow(
                    spriteBatch,
                    I18n.Ui_ItemHover_Damage($"+{damageMod:#.#%}"),
                    font,
                    new Vector2(x + 68f, y + 28f),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            #endregion damage

            #region knockback

            if (slingshot.Get_DisplayedKnockback() is var knockback && knockback != 0f)
            {
                co = slingshot.HasAmethystBonus() ? new Color(0, 120, 120) : Game1.textColor;
                spriteBatch.DrawWeightIcon(new Vector2(x + 20f, y + 20f));
                Utility.drawTextWithShadow(
                    spriteBatch,
                    I18n.Ui_ItemHover_Knockback($"{knockback:+#.#%;-#.#%}"),
                    font,
                    new Vector2(x + 68f, y + 28f),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            #endregion knockback

            #region crit chance

            if (slingshot.Get_DisplayedCritChance() is var critChance && critChance != 0f)
            {
                co = slingshot.HasAquamarineBonus() ? new Color(0, 120, 120) : Game1.textColor;
                spriteBatch.DrawCritChanceIcon(new Vector2(x + 20f, y + 20f));
                Utility.drawTextWithShadow(
                    spriteBatch,
                    I18n.Ui_ItemHover_CRate($"{critChance:+#.#%;-#.#%}"),
                    font,
                    new Vector2(x + 68f, y + 28f),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            #endregion crit chance

            #region crit power

            if (slingshot.Get_DisplayedCritPower() is var critPower && critPower != 0f)
            {
                co = slingshot.HasJadeBonus() ? new Color(0, 120, 120) : Game1.textColor;
                spriteBatch.DrawCritPowerIcon(new Vector2(x + 20f, y + 20f));
                Utility.drawTextWithShadow(
                    spriteBatch,
                    I18n.Ui_ItemHover_CPow($"{critPower:+#.#%;-#.#%}"),
                    font,
                    new Vector2(x + 68f, y + 28f),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            #endregion crit power

            #region firing speed

            if (slingshot.Get_DisplayedFireSpeed() is var speedModifier && speedModifier > 0f)
            {
                co = new Color(0, 120, 120);
                spriteBatch.DrawSpeedIcon(new Vector2(x + 20f, y + 20f));
                Utility.drawTextWithShadow(
                    spriteBatch,
                    I18n.Ui_ItemHover_FireSpeed($"{speedModifier:+#.#%;-#.#%}"),
                    font,
                    new Vector2(x + 68f, y + 28f),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            #endregion firing speed

            #region cooldown reduction

            if (slingshot.Get_DisplayedCooldownModifier() is var cooldownModifier && cooldownModifier > 0f)
            {
                co = new Color(0, 120, 120);
                spriteBatch.DrawCooldownIcon(new Vector2(x + 20f, y + 20f));
                Utility.drawTextWithShadow(
                    spriteBatch,
                    I18n.Ui_ItemHover_Cdr($"-{cooldownModifier:#.#%}"),
                    font,
                    new Vector2(x + 68f, y + 28f),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            #endregion cooldown reduction

            #region resilience

            if (slingshot.Get_DisplayedResilience() is var resilience && resilience > 0f)
            {
                co = new Color(0, 120, 120);
                var amount = CombatModule.ShouldEnable && CombatModule.Config.NewResistanceFormula
                    ? $"+{resilience:#.#%}"
                    : $"{(int)(resilience * 100f)}";

                spriteBatch.DrawDefenseIcon(new Vector2(x + 20f, y + 20f));
                Utility.drawTextWithShadow(
                    spriteBatch,
                    CombatModule.ShouldEnable && CombatModule.Config.NewResistanceFormula
                        ? I18n.Ui_ItemHover_Resist(amount)
                        : Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", amount),
                    font,
                    new Vector2(x + 68f, y + 28f),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            #endregion resilience

            #region prismatic enchantments

            co = new Color(120, 0, 210);
            for (var i = 0; i < __instance.enchantments.Count; i++)
            {
                var enchantment = __instance.enchantments[i];
                if (!enchantment.ShouldBeDisplayed())
                {
                    continue;
                }

                spriteBatch.DrawEnchantmentIcon(new Vector2(x + 20f, y + 20f));
                Utility.drawTextWithShadow(
                    spriteBatch,
                    BaseEnchantment.hideEnchantmentName ? "???" : enchantment.GetDisplayName(),
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
