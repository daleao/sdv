namespace DaLion.Professions.Framework.Events.Limit.ChargeInitiated;

/// <summary>Interface for the arguments of an <see cref="LimitChargeInitiatedEvent"/>.</summary>
public interface ILimitChargeInitiatedEventArgs
{
    /// <summary>Gets the player who triggered the event.</summary>
    Farmer Player { get; }

    /// <summary>Gets the new charge value.</summary>
    double NewValue { get; }
}
