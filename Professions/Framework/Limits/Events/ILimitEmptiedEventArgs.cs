namespace DaLion.Professions.Framework.Limits.Events;

/// <summary>Interface for the arguments of a <see cref="LimitEmptiedEvent"/>.</summary>
public interface ILimitEmptiedEventArgs
{
    /// <summary>Gets the player who triggered the event.</summary>
    Farmer Player { get; }
}
