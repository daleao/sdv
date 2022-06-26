namespace DaLion.Stardew.Alchemy.Framework.Events.Toxicity;

#region using directives

using StardewValley;

#endregion using directives


/// <summary>Interface for the arguments of a <see cref="ToxicityChangedEvent"/>.</summary>
public interface IToxicityChangedEventArgs
{
    /// <summary>The player who triggered the event.</summary>
    Farmer Player { get; }

    /// <summary>The old toxicity value.</summary>
    double OldValue { get; }

    /// <summary>The new toxicity value.</summary>
    double NewValue { get; }
}