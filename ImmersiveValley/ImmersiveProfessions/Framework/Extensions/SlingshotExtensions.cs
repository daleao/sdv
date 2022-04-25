namespace DaLion.Stardew.Professions.Framework.Extensions;

#region using directives

using System;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

/// <summary>Extensions for the <see cref="Slingshot"/> class.</summary>
public static class SlingshotExtensions
{
    /// <summary>Determines the extra power of Desperado shots.</summary>
    /// <param name="who">The player who is firing the instance.</param>
    /// <returns>Returns a percentage between 0 and 1.</returns>
    public static float GetDesperadoOvercharge(this Slingshot slingshot, Farmer who)
    {
        if (slingshot.pullStartTime < 0.0) return 0f;

        return Math.Clamp((float) ((Game1.currentGameTime.TotalGameTime.TotalSeconds - slingshot.pullStartTime) / 0.3f - 1f) /
                          (who.HasProfession(Profession.Desperado, true) ? 6f : 3f), 0f, 1f);
    }
}