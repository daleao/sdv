namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using DaLion.Stardew.Professions.Framework;
using StardewValley.Tools;

#endregion using directives

/// <summary>Extensions for the <see cref="Slingshot"/> class.</summary>
public static class SlingshotExtensions
{
    /// <summary>Determines the extra power of shots fired by <see cref="Profession.Desperado"/>.</summary>
    /// <param name="slingshot">The <see cref="Slingshot"/>.</param>
    /// <param name="who">The <see cref="Farmer"/> using the <paramref name="slingshot"/>.</param>
    /// <returns>A percentage between 0 and 1.</returns>
    public static float GetOvercharge(this Slingshot slingshot, Farmer who)
    {
        if (slingshot.pullStartTime < 0.0 || slingshot.CanAutoFire())
        {
            return 0f;
        }

        // divides number of seconds elapsed since pull and required charged time to obtain `units of required charge time`,
        // from which we subtract 1 to account for the initial charge before the overcharge began, and finally divide by twice the number of units we want to impose (3)
        // to account for Desperado's halving of required charge time
        var overcharge = Math.Clamp(
            (float)(((Game1.currentGameTime.TotalGameTime.TotalSeconds - slingshot.pullStartTime) /
                     slingshot.GetRequiredChargeTime()) - 1f) /
            (who.HasProfession(Profession.Desperado, true) ? 4f : 6f),
            0f,
            1f);

        return overcharge;
    }
}
