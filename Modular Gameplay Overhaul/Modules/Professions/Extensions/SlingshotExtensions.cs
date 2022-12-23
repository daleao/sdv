namespace DaLion.Overhaul.Modules.Professions.Extensions;

#region using directives

using StardewValley.Tools;

#endregion using directives

/// <summary>Extensions for the <see cref="Slingshot"/> class.</summary>
internal static class SlingshotExtensions
{
    /// <summary>Determines the extra power of shots fired by <see cref="Profession.Desperado"/>.</summary>
    /// <param name="slingshot">The <see cref="Slingshot"/>.</param>
    /// <returns>A value between 1 and 2.</returns>
    internal static float GetOvercharge(this Slingshot slingshot)
    {
        if (Game1.options.useLegacySlingshotFiring || slingshot.pullStartTime < 0.0 || slingshot.CanAutoFire())
        {
            return 1f;
        }

        // divides number of seconds elapsed since pull and required charged time to obtain `units of required charge time`,
        // from which we subtract 1 to account for the initial charge before the overcharge began, and finally divide by twice the number of units we want to impose (3)
        var overcharge = Math.Clamp(
            (float)(((Game1.currentGameTime.TotalGameTime.TotalSeconds - slingshot.pullStartTime) /
                     slingshot.GetRequiredChargeTime()) - 1f) / 6f,
            0f,
            1f);

        return overcharge + 1f;
    }
}
