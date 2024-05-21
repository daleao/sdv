namespace DaLion.Professions.Framework.Limits.Events;

/// <summary>Interface for the arguments of a <see cref="LimitActivatedEvent"/>.</summary>
public interface ILimitActivatedEventArgs
{
    /// <summary>Gets the player who triggered the event.</summary>
    Farmer Player { get; }
}
