namespace DaLion.Stardew.Professions.Framework.TreasureHunt;

#region using directives

using Microsoft.Xna.Framework;
using StardewValley;

#endregion using directives

/// <summary>Interface for treasure hunts.</summary>
internal interface ITreasureHunt
{
    public bool IsActive { get; }

    public Vector2? TreasureTile { get; }

    /// <summary>End the hunt unsuccessfully.</summary>
    public void Fail();

    /// <summary>Reset the accumulated bonus chance to trigger a new hunt.</summary>
    public void ResetAccumulatedBonus();

    /// <summary>Try to start a new hunt at the specified location.</summary>
    /// <param name="location">The game location.</param>
    public void TryStartNewHunt(GameLocation location);

    /// <summary>Check for completion or failure.</summary>
    /// <param name="ticks">The number of ticks elapsed since the game started.</param>
    public void Update(uint ticks);
}