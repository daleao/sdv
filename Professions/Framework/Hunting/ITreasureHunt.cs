namespace DaLion.Professions.Framework.Hunting;

#region using directives

using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Interface for treasure hunts.</summary>
public interface ITreasureHunt
{
    /// <summary>
    ///     Gets whether this instance pertains to a <see cref="Profession.Scavenger"/> or a
    ///     <see cref="Profession.Prospector"/>.
    /// </summary>
    public TreasureHuntProfession Profession { get; }

    /// <summary>Gets the active hunt's <see cref="GameLocation"/>.</summary>
    public GameLocation? Location { get; }

    /// <summary>Gets the target tile containing treasure.</summary>
    public Vector2? TargetTile { get; }

    /// <summary>Gets a value indicating whether the <see cref="TargetTile"/> is set to a valid target.</summary>
    public bool IsActive { get; }

    /// <summary>Gets the elapsed time of the active hunt, in seconds.</summary>
    public int Elapsed { get; }

    /// <summary>Gets the time limit of the active hunt, in seconds.</summary>
    public int TimeLimit { get; }

    /// <summary>Tries to start a new hunt at a random tile.</summary>
    /// <param name="location">The current <see cref="GameLocation"/>.</param>
    /// <returns><see langword="true"/> if a hunt was started, otherwise <see langword="false"/>.</returns>
    [MemberNotNullWhen(true, nameof(Location), nameof(TargetTile))]
    public bool TryStart(GameLocation location);

    /// <summary>Ends the active hunt successfully.</summary>
    public void Complete();

    /// <summary>Ends the active hunt unsuccessfully.</summary>
    public void Fail();
}
