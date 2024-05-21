namespace DaLion.Professions.Framework.Limits.Events;

/// <summary>Interface for the arguments of a <see cref="LimitChargeInitiatedEvent"/>.</summary>
public interface ILimitChargeInitiatedEventArgs
{
    /// <summary>Gets the player who triggered the event.</summary>
    Farmer Player { get; }

    /// <summary>Gets the new charge value.</summary>
    double NewValue { get; }
}
