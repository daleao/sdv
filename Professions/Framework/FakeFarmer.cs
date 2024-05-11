namespace DaLion.Professions.Framework;

/// <summary>A wrapper for fake <see cref="Farmer"/> instances used for faking typically farmer-only interactions.</summary>
internal sealed class FakeFarmer : Farmer
{
    /// <summary>Initializes a new instance of the <see cref="FakeFarmer"/> class.</summary>
    internal FakeFarmer()
    {
    }

    /// <summary>Gets or sets a value indicating whether the <see cref="FakeFarmer"/> is an enemy to be targeted.</summary>
    internal bool IsEnemy { get; set; } = true;
}
