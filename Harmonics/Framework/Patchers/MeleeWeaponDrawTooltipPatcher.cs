namespace DaLion.Harmonics.Framework.Patchers;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Core.Framework.Extensions;
using DaLion.Harmonics.Framework.VirtualProperties;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Enchantments;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDrawTooltipPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponDrawTooltipPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal MeleeWeaponDrawTooltipPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.drawTooltip));
    }

    #region harmony patches

    /// <summary>Display cooldown effects in tooltip.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? MeleeWeaponDrawTooltipTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .PatternMatch(
                    [
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(Tool).RequireField(nameof(Tool.enchantments))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(NetList<BaseEnchantment, NetRef<BaseEnchantment>>).RequirePropertyGetter("Count")),
                    ],
                    ILHelper.SearchOption.Last)
                .StripLabels(out var labels)
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Ldarg_3, (byte)3),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(OpCodes.Ldarg_2, (byte)2),
                        new CodeInstruction(OpCodes.Ldind_I4), // de-reference the ref x param
                        new CodeInstruction(OpCodes.Ldarg_3, (byte)3),
                        new CodeInstruction(OpCodes.Ldind_I4), // de-reference the ref y param
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)4),
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)5),
                        new CodeInstruction(OpCodes.Call, typeof(MeleeWeaponDrawTooltipPatcher).RequireMethod(nameof(DrawCooldown))),
                        new CodeInstruction(OpCodes.Ldarg_3, (byte)3),
                        new CodeInstruction(OpCodes.Ldind_I4),
                        new CodeInstruction(OpCodes.Add),
                        new CodeInstruction(OpCodes.Stind_I4),
                    ],
                    labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting cooldown draw.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected

    private static int DrawCooldown(MeleeWeapon weapon, SpriteBatch b, int x, int y, SpriteFont font, float alpha)
    {
        if (!weapon.hasEnchantmentOfType<GarnetEnchantment>() || weapon.Get_CooldownReduction().Value is not (var cdr and > 0f))
        {
            return 0;
        }

        b.DrawCooldownIcon(new Vector2(x + 16 + 4, y + 16 + 4));
        Utility.drawTextWithShadow(
            b,
            I18n.Ui_ItemHover_Cdr($"-{cdr:#.#%}"),
            font,
            new Vector2(x + 16 + 52, y + 16 + 12),
            new Color(0, 120, 120) * 0.9f * alpha);
        return (int)Math.Max(font.MeasureString("TT").Y, 48f);
    }

    #endregion injected
}
