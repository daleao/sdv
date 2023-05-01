namespace DaLion.Stardew.Alchemy.Framework.Events.Toxicity;

#region using directives

using StardewValley;

#endregion using directives

/// <summary>Interface for the arguments of a <see cref="PlayerOverdosedEvent"/>.</summary>
public interface IPlayerOverdosedEventArgs
{
    /// <summary>The player who triggered the event.</summary>
    Farmer Player { get; }
}