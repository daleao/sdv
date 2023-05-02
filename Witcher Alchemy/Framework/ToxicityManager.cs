namespace DaLion.Stardew.Alchemy.Framework;

#region using directives

using StardewValley;
using System;
using DaLion.Alchemy.Events.Toxicity.Changed;
using DaLion.Alchemy.Events.Toxicity.Cleared;
using DaLion.Alchemy.Events.Toxicity.Filled;
using DaLion.Alchemy.Events.Toxicity.Overdosed;

#endregion using directives

internal class ToxicityManager
{
    public const int BASE_TOLERANCE_I = 100;

    public static int MaxTolerance { get; }

    public static int OverdoseThreshold { get; }

    private static int _ToxicityValueValue;

    public static int ToxicityValue
    {
        get => _ToxicityValueValue;
        set => _ToxicityValueValue = value;
    }

    #region event handlers

    /// <inheritdoc cref="OnChanged"/>
    internal static event EventHandler<IToxicityChangedEventArgs>? Changed;

    /// <inheritdoc cref="OnCleared"/>
    internal static event EventHandler<IToxicityClearedEventArgs>? Cleared;

    /// <inheritdoc cref="OnFilled"/>
    internal static event EventHandler<IToxicityFilledEventArgs>? Filled;

    /// <inheritdoc cref="OnOverdosed"/>
    internal static event EventHandler<IPlayerOverdosedEventArgs>? Overdosed;

    #endregion event handlers

    #region event callbacks

    /// <summary>Raised when a player's Toxicity value changes.</summary>
    /// <param name="oldValue">The old toxicity value.</param>
    /// <param name="newValue">The old charge value.</param>
    protected void OnChanged(double oldValue, double newValue)
    {
        Changed?.Invoke(this, new ToxicityChangedEventArgs(Game1.player, oldValue, newValue));
    }

    /// <summary>Raised when a player's Toxicity value drops back to zero.</summary>
    protected void OnCleared()
    {
        Cleared?.Invoke(this, new ToxicityClearedEventArgs(Game1.player));
    }

    /// <summary>Raised when a player's Toxicity value reaches the maximum value.</summary>
    protected void OnFilled()
    {
        Filled?.Invoke(this, new ToxicityFilledEventArgs(Game1.player));
    }

    /// <summary>Raised when a player's Toxicity value crosses the overdose threshold.</summary>
    protected void OnOverdosed()
    {
        Overdosed?.Invoke(this, new PlayerOverdosedEventArgs(Game1.player));
    }

    #endregion event callbacks
}