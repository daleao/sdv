namespace DaLion.Overhaul.Modules.Combat.Patchers.Ranged;

#region using directives

using System.Reflection;
using System.Text;
using DaLion.Overhaul.Modules.Combat.Enchantments;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using Extensions;
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
                co = slingshot.Get_EffectiveDamageModifier() > 1f ? new Color(0, 120, 120) : Game1.textColor;
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
            else if (__instance.InitialParentTileIndex != WeaponIds.BasicSlingshot)
            {
                co = Game1.textColor;
                spriteBatch.DrawAttackIcon(new Vector2(x + 20f, y + 20f));
                Utility.drawTextWithShadow(
                    spriteBatch,
                    I18n.Ui_ItemHover_Damage($"+{slingshot.Get_DisplayedDamageModifier():#.#%}"),
                    font,
                    new Vector2(x + 68f, y + 28f),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            #endregion damage

            #region knockback

            var knockback = slingshot.Get_DisplayedKnockback();
            if (knockback != 0f)
            {
                co = slingshot.Get_EffectiveKnockback() > 0 ? new Color(0, 120, 120) : Game1.textColor;
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

            var critChance = slingshot.Get_DisplayedCritChance();
            if (critChance != 0f)
            {
                co = slingshot.Get_EffectiveCritChance() > 0 ? new Color(0, 120, 120) : Game1.textColor;
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

            var critPower = slingshot.Get_DisplayedCritPower();
            if (critPower != 0f)
            {
                co = slingshot.Get_EffectiveCritPower() > 0 ? new Color(0, 120, 120) : Game1.textColor;
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

            var speedModifier = slingshot.Get_DisplayedFireSpeed();
            if (speedModifier > 0f)
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

            var cooldownModifier = slingshot.Get_DisplayedCooldownModifier();
            if (cooldownModifier > 0f)
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

            var resilience = slingshot.Get_DisplayedResilience();
            if (resilience > 0f)
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
