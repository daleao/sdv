namespace DaLion.Professions.Framework.Limits.Events;

/// <summary>Interface for the arguments of a <see cref="LimitChargeChangedEvent"/>.</summary>
public interface ILimitChargeChangedEventArgs
{
    /// <summary>Gets the player who triggered the event.</summary>
    Farmer Player { get; }

    /// <summary>Gets the previous charge value.</summary>
    double OldValue { get; }

    /// <summary>Gets the new charge value.</summary>
    double NewValue { get; }
}
