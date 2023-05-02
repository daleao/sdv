namespace DaLion.Alchemy.Events.Toxicity.Changed;

#region using directives

using StardewValley;

#endregion using directives

/// <summary>Interface for the arguments of a <see cref="ToxicityChangedEvent"/>.</summary>
public interface IToxicityChangedEventArgs
{
    /// <summary>Gets the player who triggered the event.</summary>
    Farmer Player { get; }

    /// <summary>Gets the old toxicity value.</summary>
    double OldValue { get; }

    /// <summary>Gets the new toxicity value.</summary>
    double NewValue { get; }
}
