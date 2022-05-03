namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using StardewValley;

#endregion using directives

public interface IUltimateActivatedEventArgs
{
    /// <summary>The player who triggered the event.</summary>
    Farmer Player { get; }
}