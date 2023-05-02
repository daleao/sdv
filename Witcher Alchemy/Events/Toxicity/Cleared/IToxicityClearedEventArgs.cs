namespace DaLion.Alchemy.Events.Toxicity.Cleared;

#region using directives

using StardewValley;

#endregion using directives

/// <summary>Interface for the arguments of a <see cref="ToxicityClearedEvent"/>.</summary>
public interface IToxicityClearedEventArgs
{
    /// <summary>Gets the player who triggered the event.</summary>
    Farmer Player { get; }
}
