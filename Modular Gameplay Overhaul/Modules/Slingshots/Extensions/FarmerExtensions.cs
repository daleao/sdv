namespace DaLion.Overhaul.Modules.Slingshots.Extensions;

#region using directives

using DaLion.Overhaul.Modules.Slingshots.VirtualProperties;
using StardewValley.Tools;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
internal static class FarmerExtensions
{
    /// <summary>Gets the total firing speed modifier for the <paramref name="farmer"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="slingshot">The <paramref name="farmer"/>'s slingshot.</param>
    /// <returns>The total firing speed modifier, a number between 0 and 1.</returns>
    internal static float GetTotalFiringSpeedModifier(this Farmer farmer, Slingshot? slingshot = null)
    {
        var modifier = 10f / (10f + farmer.weaponSpeedModifier);
        slingshot ??= farmer.CurrentTool as Slingshot;
        if (slingshot is not null)
        {
            modifier *= slingshot.Get_EmeraldFireSpeed();
        }

        return modifier;
    }

    /// <summary>Determines whether the <paramref name="farmer"/> is stepping on a snowy tile.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if the corresponding <see cref="FarmerSprite"/> is using snowy step sounds, otherwise <see langword="false"/>.</returns>
    internal static bool IsSteppingOnSnow(this Farmer farmer)
    {
        return farmer.FarmerSprite.currentStep == "snowyStep";
    }
}
