namespace DaLion.Professions.Framework.Events.Limit.Deactivated;

/// <summary>Interface for the arguments of an <see cref="LimitDeactivatedEvent"/>.</summary>
public interface ILimitDeactivatedEventArgs
{
    /// <summary>Gets the player who triggered the event.</summary>
    Farmer Player { get; }
}
