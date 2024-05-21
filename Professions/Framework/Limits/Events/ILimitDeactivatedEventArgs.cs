namespace DaLion.Professions.Framework.Limits.Events;

/// <summary>Interface for the arguments of a <see cref="LimitDeactivatedEvent"/>.</summary>
public interface ILimitDeactivatedEventArgs
{
    /// <summary>Gets the player who triggered the event.</summary>
    Farmer Player { get; }
}
