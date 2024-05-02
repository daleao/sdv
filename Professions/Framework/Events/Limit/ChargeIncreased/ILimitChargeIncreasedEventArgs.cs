namespace DaLion.Professions.Framework.Events.Limit.ChargeIncreased;

/// <summary>Interface for the arguments of an <see cref="LimitChargeIncreasedEvent"/>.</summary>
public interface ILimitChargeIncreasedEventArgs
{
    /// <summary>Gets the player who triggered the event.</summary>
    Farmer Player { get; }

    /// <summary>Gets the previous charge value.</summary>
    double OldValue { get; }

    /// <summary>Gets the new charge value.</summary>
    double NewValue { get; }
}
