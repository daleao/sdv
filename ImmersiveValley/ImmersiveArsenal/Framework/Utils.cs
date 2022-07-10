namespace DaLion.Stardew.Arsenal.Framework;

#region using directives

using StardewValley;
using StardewValley.Tools;

#endregion using directives

internal static class Utils
{
    internal static void GetHolyBlade()
    {
        Game1.flashAlpha = 1f;
        Game1.player.holdUpItemThenMessage(new MeleeWeapon(Constants.HOLY_BLADE_INDEX_I));
        ((MeleeWeapon)Game1.player.CurrentTool).transform(Constants.HOLY_BLADE_INDEX_I);
        Game1.player.mailReceived.Add("holyBlade");
        Game1.player.jitterStrength = 0f;
        Game1.screenGlowHold = false;
    }
}