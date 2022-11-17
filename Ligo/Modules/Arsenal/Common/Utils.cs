namespace DaLion.Ligo.Modules.Arsenal.Common;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Common.Enchantments;
using StardewValley.Tools;

#endregion using directives

internal static class Utils
{
    /// <summary>Adds hidden weapon enchantments related to Infinity +1.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    internal static void AddEnchantments(MeleeWeapon weapon)
    {
        switch (weapon.InitialParentTileIndex)
        {
            case Constants.DarkSwordIndex:
                weapon.enchantments.Add(new CursedEnchantment());
                break;
            case Constants.HolyBladeIndex:
                weapon.enchantments.Add(new BlessedEnchantment());
                break;
            case Constants.InfinityBladeIndex:
            case Constants.InfinityDaggerIndex:
            case Constants.InfinityClubIndex:
                weapon.enchantments.Add(new InfinityEnchantment());
                break;
        }
    }

    /// <summary>Transforms the currently held weapon into the Holy Blade.</summary>
    internal static void GetHolyBlade()
    {
        Game1.flashAlpha = 1f;
        Game1.player.holdUpItemThenMessage(new MeleeWeapon(Constants.HolyBladeIndex));
        ((MeleeWeapon)Game1.player.CurrentTool).transform(Constants.HolyBladeIndex);
        Game1.player.mailReceived.Add("holyBlade");
        Game1.player.jitterStrength = 0f;
        Game1.screenGlowHold = false;
    }
}
