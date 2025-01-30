namespace DaLion.Enchantments.Framework.Patchers;

#region using directives

using System.Text;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponGetExtraSpaceNeededForTooltipSpecialIconsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponGetExtraSpaceNeededForTooltipSpecialIconsPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal MeleeWeaponGetExtraSpaceNeededForTooltipSpecialIconsPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.getExtraSpaceNeededForTooltipSpecialIcons));
    }

    #region harmony patches

    /// <summary>Display enchanted Scythe tooltip.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void MeleeWeaponGetExtraSpaceNeededForTooltipSpecialIconsPostfix(
        MeleeWeapon __instance,
        ref Point __result,
        SpriteFont font,
        int minWidth,
        int horizontalBuffer,
        int startingHeight,
        StringBuilder descriptionText,
        string boldTitleText,
        int moneyAmountToDisplayAtBottom)
    {
        if (__instance.isScythe())
        {
            __result.Y -= LocalizedContentManager.CurrentLanguageCode switch
            {
                LocalizedContentManager.LanguageCode.ja => 28,
                LocalizedContentManager.LanguageCode.ko => 48,
                LocalizedContentManager.LanguageCode.zh => 30,
                _ => 34,
            };
        }
    }

    #endregion harmony patches
}
