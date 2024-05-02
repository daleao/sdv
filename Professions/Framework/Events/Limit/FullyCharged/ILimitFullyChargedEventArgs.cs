namespace DaLion.Professions.Framework.Events.Limit.FullyCharged;

/// <summary>Interface for the arguments of a <see cref="LimitFullyChargedEvent"/>.</summary>
public interface ILimitFullyChargedEventArgs
{
    /// <summary>Gets the player who triggered the event.</summary>
    Farmer Player { get; }
}
