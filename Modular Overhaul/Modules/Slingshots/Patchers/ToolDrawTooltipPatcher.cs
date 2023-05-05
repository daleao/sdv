namespace DaLion.Overhaul.Modules.Slingshots.Patchers;

#region using directives

using System.Reflection;
using System.Text;
using DaLion.Overhaul.Modules.Enchantments.Gemstone;
using DaLion.Overhaul.Modules.Slingshots.VirtualProperties;
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
        if (__instance is not Slingshot slingshot || !SlingshotsModule.Config.EnableRebalance)
        {
            return true; // run original logic
        }

        try
        {
            // write description
            ItemDrawTooltipPatcher.ItemDrawTooltipReverse(
                __instance,
                spriteBatch,
                ref x,
                ref y,
                font,
                alpha,
                overrideText);

            Color co;

            // write bonus damage
            var hasRubyEnchant = slingshot.hasEnchantmentOfType<RubyEnchantment>();
            if (slingshot.InitialParentTileIndex != ItemIDs.BasicSlingshot || hasRubyEnchant)
            {
                var amount = $"+{slingshot.Get_RelativeDamageModifier():#.#%}";
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
                    I18n.Ui_Itemhover_Damage(amount),
                    font,
                    new Vector2(x + 68, y + 28),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            // write bonus knockback
            var hasAmethystEnchant = __instance.hasEnchantmentOfType<AmethystEnchantment>();
            if (slingshot.InitialParentTileIndex != ItemIDs.BasicSlingshot || hasAmethystEnchant)
            {
                var amount = $"+{slingshot.Get_RelativeKnockbackModifer():#.#%}";
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
                    I18n.Ui_Itemhover_Knockback(amount),
                    font,
                    new Vector2(x + 68, y + 28),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            // write bonus crit rate
            if (__instance.hasEnchantmentOfType<AquamarineEnchantment>())
            {
                var amount = $"{slingshot.Get_RelativeCritChanceModifier():#.#%}";
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
                    Game1.content.LoadString("Strings\\UI:ItemHover_CritChanceBonus", amount),
                    font,
                    new Vector2(x + 68, y + 28),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            // write crit power
            if (__instance.hasEnchantmentOfType<JadeEnchantment>())
            {
                var amount = $"{slingshot.Get_RelativeCritPowerModifier():#.#%}";
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
                    Game1.content.LoadString("Strings\\UI:ItemHover_CritPowerBonus", amount),
                    font,
                    new Vector2(x + 16 + 44, y + 16 + 12),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            // write bonus fire speed
            if (__instance.hasEnchantmentOfType<EmeraldEnchantment>())
            {
                var amount = $"+{slingshot.Get_RelativeFireSpeed():#.#%}";
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
                    I18n.Ui_Itemhover_Firespeed(amount),
                    font,
                    new Vector2(x + 68, y + 28),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            // write bonus cooldown reduction
            if (__instance.hasEnchantmentOfType<GarnetEnchantment>())
            {
                var amount = $"-{slingshot.Get_RelativeCooldownReduction():#.#%}";
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
                    I18n.Ui_Itemhover_Cdr(amount),
                    font,
                    new Vector2(x + 68, y + 28),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            // write bonus defense
            if (__instance.hasEnchantmentOfType<TopazEnchantment>() && EnchantmentsModule.Config.RebalancedForges)
            {
                var amount = CombatModule.ShouldEnable && CombatModule.Config.OverhauledDefense
                    ? $"+{slingshot.Get_RelativeResilience():#.#%}"
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
                    CombatModule.ShouldEnable && CombatModule.Config.OverhauledDefense
                        ? I18n.Ui_Itemhover_Resist(amount)
                        : Game1.content.LoadString("ItemHover_DefenseBonus", amount),
                    font,
                    new Vector2(x + 68, y + 28),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            // write bonus random forge
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

            // write other enchantments
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
}
