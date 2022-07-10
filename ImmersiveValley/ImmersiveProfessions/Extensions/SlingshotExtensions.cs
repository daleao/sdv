namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using StardewValley;
using StardewValley.Tools;
using System;

#endregion using directives

/// <summary>Extensions for the <see cref="Slingshot"/> class.</summary>
public static class SlingshotExtensions
{
    /// <summary>Determines the extra power of Desperado shots.</summary>
    /// <param name="who">The player who is firing the instance.</param>
    /// <returns>A percentage between 0 and 1.</returns>
    public static float GetDesperadoOvercharge(this Slingshot slingshot, Farmer who)
    {
        if (slingshot.pullStartTime < 0.0) return 0f;

        // divides number of seconds elapsed since pull and divide by required charged time to obtain `units of required charge time`,
        // from which we subtract 1 to account for the initially charge before the overcharge began, and finally divide by twice the number of units we want to impose (3)
        // to account for Desperado's halving of required charge time
        return Math.Clamp(
            (float)((Game1.currentGameTime.TotalGameTime.TotalSeconds - slingshot.pullStartTime) /
                slingshot.GetRequiredChargeTime() - 1f) / 6f, 0f, 1f);
    }
}