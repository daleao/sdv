namespace DaLion.Redux.Framework.Arsenal.Weapons.Extensions;

#region using directives

using StardewValley.Tools;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
internal class FarmerExtensions
{
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
