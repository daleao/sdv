namespace DaLion.Professions.Framework.Events.Limit.Emptied;

/// <summary>Interface for the arguments of an <see cref="LimitEmptiedEvent"/>.</summary>
public interface ILimitEmptiedEventArgs
{
    /// <summary>Gets the player who triggered the event.</summary>
    Farmer Player { get; }
}
