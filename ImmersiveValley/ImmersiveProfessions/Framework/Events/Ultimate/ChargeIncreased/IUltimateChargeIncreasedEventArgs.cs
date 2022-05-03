namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using StardewValley;

#endregion using directives

public interface IUltimateChargeIncreasedEventArgs
{
    /// <summary>The player who triggered the event.</summary>
    Farmer Player { get; }

    /// <summary>The previous charge value.</summary>
    double OldValue { get; }

    /// <summary>The new charge value.</summary>
    double NewValue { get; }
}