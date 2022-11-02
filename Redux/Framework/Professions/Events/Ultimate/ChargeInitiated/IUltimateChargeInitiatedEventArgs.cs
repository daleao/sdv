namespace DaLion.Redux.Framework.Professions.Events.Ultimate;

/// <summary>Interface for the arguments of an <see cref="UltimateChargeInitiatedEvent"/>.</summary>
public interface IUltimateChargeInitiatedEventArgs
{
    /// <summary>Gets the player who triggered the event.</summary>
    Farmer Player { get; }

    /// <summary>Gets the new charge value.</summary>
    double NewValue { get; }
}
