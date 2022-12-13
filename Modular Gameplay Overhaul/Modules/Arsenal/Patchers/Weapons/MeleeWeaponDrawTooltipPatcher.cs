namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Weapons;

#region using directives

using System.Linq;
using System.Reflection;
using DaLion.Overhaul.Modules.Arsenal.VirtualProperties;
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
        if (!ArsenalModule.Config.Weapons.RebalancedStats)
        {
            return true; // run original logic
        }

        try
        {
            // write description
            var descriptionWidth = Reflector
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
                    __instance.Get_MinDamage(),
                    __instance.Get_MaxDamage()),
                font,
                new Vector2(x + 68, y + 28),
                co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);

            // write bonus knockback
            var relativeKnockback = __instance.Get_RelativeKnockback();
            if (relativeKnockback != 0)
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
                    I18n.Get(
                        "ui.itemhover.knockback",
                        new { amount = $"{relativeKnockback:+#%;-#%}" }),
                    font,
                    new Vector2(x + 68, y + 28),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            // write bonus crit rate
            var relativeCritChance = __instance.Get_RelativeCritChance();
            if (relativeCritChance != 0)
            {
                co = __instance.hasEnchantmentOfType<AquamarineEnchantment>()
                    ? new Color(0, 120, 120)
                    : Game1.textColor;
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
                    I18n.Get(
                        "ui.itemhover.crate",
                        new { amount = $"{relativeCritChance:+#%;-#%}" }),
                    font,
                    new Vector2(x + 68, y + 28),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            // write bonus crit power
            var relativeGetCritPower = __instance.Get_RelativeCritPower();
            if (relativeGetCritPower != 0)
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
                    I18n.Get(
                        "ui.itemhover.cpow",
                        new { amount = $"{relativeGetCritPower:+#%;-#%}" }),
                    font,
                    new Vector2(x + 68, y + 28),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            // write bonus swing speed
            var speed = __instance.Get_RelativeSwingSpeed();
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
                    I18n.Get(
                        "ui.itemhover.swingspeed",
                        new { amount = $"{speed:+#%;-#%}" }),
                    font,
                    new Vector2(x + 68, y + 28),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            // write bonus cooldown reduction
            var cooldownReduction = __instance.Get_RelativeCooldownReduction();
            if (cooldownReduction > 0)
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
                    I18n.Get("ui.itemhover.cdr", new { amount = $"-{cooldownReduction:0%}" }),
                    font,
                    new Vector2(x + 68, y + 28),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            // write bonus defense
            var resistance = __instance.Get_RelativeResilience();
            if ((ArsenalModule.Config.OverhauledDefense && resistance != 0f) || resistance > 1f)
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
                    Config.EnableArsenal && ArsenalModule.Config.OverhauledDefense
                        ? I18n.Get("ui.itemhover.resist", new { amount = $"{resistance:+#%;-#%}" })
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
            foreach (var enchantment in __instance.enchantments.Where(e => e.ShouldBeDisplayed()))
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
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
