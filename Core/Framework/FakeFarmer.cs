namespace DaLion.Core.Framework;

/// <summary>A wrapper for fake <see cref="Farmer"/> instances used for faking typically farmer-only interactions.</summary>
public sealed class FakeFarmer : Farmer
{
    /// <summary>Gets or sets a value indicating whether the <see cref="FakeFarmer"/> is an enemy to be targeted.</summary>
    public bool IsEnemy { get; set; } = true;
}
