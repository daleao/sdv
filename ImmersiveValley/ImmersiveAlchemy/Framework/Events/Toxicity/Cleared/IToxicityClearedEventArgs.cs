namespace DaLion.Stardew.Alchemy.Framework.Events.Toxicity;

#region using directives

using StardewValley;

#endregion using directives

public interface IToxicityClearedEventArgs
{
    /// <summary>The player who triggered the event.</summary>
    Farmer Player { get; }
}