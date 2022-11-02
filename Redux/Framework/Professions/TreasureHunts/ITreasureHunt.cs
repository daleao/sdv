namespace DaLion.Redux.Framework.Professions.TreasureHunts;

#region using directives

using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Interface for treasure hunts.</summary>
public interface ITreasureHunt
{
    /// <summary>
    ///     Gets whether this instance pertains to a <see cref="Profession.Scavenger"/> or a
    ///     <see cref="Profession.Prospector"/>.
    /// </summary>
    public TreasureHuntType Type { get; }

    /// <summary>Gets a value indicating whether whether the <see cref="TreasureTile"/> is set to a valid target.</summary>
    public bool IsActive { get; }

    /// <summary>Gets the target tile containing treasure.</summary>
    public Vector2? TreasureTile { get; }

    /// <summary>Tries to start a new hunt at the specified <paramref name="location"/>.</summary>
    /// <param name="location">The game location.</param>
    /// <returns><see langword="true"/> if a hunt was started, otherwise <see langword="false"/>.</returns>
    public bool TryStart(GameLocation location);

    /// <summary>Forcefully starts a new hunt at the specified <paramref name="location"/>.</summary>
    /// <param name="location">The game location.</param>
    /// <param name="target">The target treasure tile.</param>
    public void ForceStart(GameLocation location, Vector2 target);

    /// <summary>Ends the active hunt unsuccessfully.</summary>
    public void Fail();
}
