namespace DaLion.Stardew.Professions.Framework.TreasureHunt;

#region using directives

using System;
using Microsoft.Xna.Framework;
using StardewValley;

#endregion using directives

/// <summary>Base class for treasure hunts.</summary>
internal abstract class TreasureHunt : ITreasureHunt
{
    public bool IsActive => TreasureTile is not null;
    public Vector2? TreasureTile { get; protected set; } = null;
    
    protected uint elapsed;
    protected uint timeLimit;
    protected string huntStartedMessage;
    protected string huntFailedMessage;
    protected Rectangle iconSourceRect;
    protected GameLocation huntLocation;
    protected readonly Random random = new(Guid.NewGuid().GetHashCode());
    
    private double _accumulatedBonus = 1.0;

    #region public methods

    /// <inheritdoc />
    public void ResetAccumulatedBonus()
    {
        _accumulatedBonus = 1.0;
    }

    /// <inheritdoc />
    public void Update(uint ticks)
    {
        if (!Game1.game1.IsActive || !Game1.shouldTimePass()) return;

        if (ticks % 60 == 0 && ++elapsed > timeLimit) Fail();
        else CheckForCompletion();
    }

    /// <inheritdoc />
    public abstract void Fail();

    /// <inheritdoc />
    public abstract void TryStartNewHunt(GameLocation location);

    #endregion public methods

    #region protected methods

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

    /// <summary>Select a random tile and make sure it is a valid treasure target.</summary>
    /// <param name="location">The game location.</param>
    protected abstract Vector2? ChooseTreasureTile(GameLocation location);

    /// <summary>Check if the player has found the treasure tile.</summary>
    protected abstract void CheckForCompletion();

    /// <summary>Reset treasure tile and release treasure hunt update event.</summary>
    protected abstract void End(bool successful);

    #endregion protected methods
}