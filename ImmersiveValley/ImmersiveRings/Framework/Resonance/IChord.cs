namespace DaLion.Stardew.Rings.Framework.Resonance;

/// <summary>A harmonic set of <see cref="Gemstone"/> wavelengths.</summary>
/// <remarks>
///     The interference of vibration patterns between neighboring <see cref="Gemstone"/>s may amplify, dampen or
///     even create new overtones.
/// </remarks>
public interface IChord
{
    /// <summary>Adds resonance stat bonuses to the farmer.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="who">The <see cref="Farmer"/>.</param>
    void OnEquip(GameLocation location, Farmer who);

    /// <summary>Removes resonating stat bonuses from the farmer.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="who">The <see cref="Farmer"/>.</param>
    void OnUnequip(GameLocation location, Farmer who);

    /// <summary>Adds resonance effects to the new <paramref name="location"/>.</summary>
    /// <param name="location">The new location.</param>
    void OnNewLocation(GameLocation location);

    /// <summary>Removes resonance effects from the old <paramref name="location"/>.</summary>
    /// <param name="location">The left location.</param>
    void OnLeaveLocation(GameLocation location);

    /// <summary>Updates resonance effects.</summary>
    /// <param name="who">The <see cref="Farmer"/>.</param>
    void Update(Farmer who);

    /// <summary>Adds the total resonance stat bonuses to the <paramref name="buffer"/>.</summary>
    /// <param name="buffer">A <see cref="StatBuffer"/> for aggregating stat bonuses.</param>
    void Buffer(StatBuffer buffer);
}
