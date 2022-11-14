namespace DaLion.Ligo.Modules.Arsenal.Slingshots.Extensions;

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
internal static class FarmerExtensions
{
    /// <summary>Determines whether the <paramref name="farmer"/> is stepping on a snowy tile.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if the corresponding <see cref="FarmerSprite"/> is using snowy step sounds, otherwise <see langword="false"/>.</returns>
    internal static bool IsSteppingOnSnow(this Farmer farmer)
    {
        return farmer.FarmerSprite.currentStep == "snowyStep";
    }
}
