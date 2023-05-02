namespace DaLion.Alchemy.Events.Toxicity.Filled;

#region using directives

using StardewValley;

#endregion using directives

/// <summary>Interface for the arguments of a <see cref="ToxicityFilledEvent"/>.</summary>
public interface IToxicityFilledEventArgs
{
    /// <summary>Gets the player who triggered the event.</summary>
    Farmer Player { get; }
}
