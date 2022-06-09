namespace DaLion.Stardew.Alchemy.Framework.Events.Toxicity;

#region using directives

using StardewValley;

#endregion using directives

public interface IToxicityChangedEventArgs
{
    /// <summary>The player who triggered the event.</summary>
    Farmer Player { get; }

    /// <summary>The old toxicity value.</summary>
    double OldValue { get; }

    /// <summary>The new toxicity value.</summary>
    double NewValue { get; }
}