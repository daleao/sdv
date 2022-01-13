using System;
using Microsoft.Xna.Framework;
using StardewValley;
using DaLion.Stardew.Professions.Framework.Patches.Foraging;

namespace DaLion.Stardew.Professions.Framework.TreasureHunt;

/// <summary>Base class for treasure hunts.</summary>
internal abstract class TreasureHunt
{
    protected readonly Random random = new(Guid.NewGuid().GetHashCode());

    private double _accumulatedBonus = 1.0;
    protected uint elapsed;
    protected GameLocation huntLocation;
    protected uint timeLimit;
    public bool IsActive => TreasureTile is not null;
    public Vector2? TreasureTile { get; protected set; } = null;

    protected string HuntStartedMessage { get; set; }
    protected string HuntFailedMessage { get; set; }
    protected Rectangle IconSourceRect { get; set; }

    /// <summary>Try to start a new hunt at the specified location.</summary>
    /// <param name="location">The game location.</param>
    public abstract void TryStartNewHunt(GameLocation location);

    /// <summary>Select a random tile and make sure it is a valid treasure target.</summary>
    /// <param name="location">The game location.</param>
    public abstract Vector2? ChooseTreasureTile(GameLocation location);

    /// <summary>Reset treasure tile and unsubscribe treasure hunt update event.</summary>
    public abstract void End();

    /// <summary>Check for completion or failure on every update tick.</summary>
    /// <param name="ticks">The number of ticks elapsed since the game started.</param>
    public void Update(uint ticks)
    {
        if (!Game1ShouldTimePassPatch.Game1ShouldTimePassOriginal(Game1.game1, true)) return;

        if (ticks % 60 == 0 && ++elapsed > timeLimit) Fail();
        else CheckForCompletion();
    }

    /// <summary>Reset the accumulated bonus chance to trigger a new hunt.</summary>
    public void ResetAccumulatedBonus()
    {
        _accumulatedBonus = 1.0;
    }

    /// <summary>Start a new treasure hunt or adjust the odds for the next attempt.</summary>
    protected bool TryStartNewHunt()
    {
        if (random.NextDouble() > ModEntry.Config.ChanceToStartTreasureHunt * _accumulatedBonus)
        {
            _accumulatedBonus *= 1.0 + Game1.player.DailyLuck;
            return false;
        }

        _accumulatedBonus = 1.0;
        return true;
    }

    /// <summary>Check if the player has found the treasure tile.</summary>
    protected abstract void CheckForCompletion();

    /// <summary>End the hunt unsuccessfully.</summary>
    protected abstract void Fail();
}