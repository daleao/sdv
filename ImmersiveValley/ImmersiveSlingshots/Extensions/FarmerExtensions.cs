namespace DaLion.Stardew.Slingshots.Extensions;

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
public static class FarmerExtensions
{
    /// <summary>Whether the farmer is stepping on a snowy tile.</summary>
    public static bool IsSteppingOnSnow(this Farmer farmer) => farmer.FarmerSprite.currentStep == "snowyStep";
}