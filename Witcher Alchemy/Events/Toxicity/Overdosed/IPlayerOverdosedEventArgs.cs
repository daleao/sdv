namespace DaLion.Alchemy.Events.Toxicity.Overdosed;

#region using directives

using StardewValley;

#endregion using directives

/// <summary>Interface for the arguments of a <see cref="PlayerOverdosedEvent"/>.</summary>
public interface IPlayerOverdosedEventArgs
{
    /// <summary>Gets the player who triggered the event.</summary>
    Farmer Player { get; }
}
