namespace DaLion.Redux.Framework.Professions.Events.Ultimate;

/// <summary>Interface for the arguments of an <see cref="UltimateEmptiedEvent"/>.</summary>
public interface IUltimateEmptiedEventArgs
{
    /// <summary>Gets the player who triggered the event.</summary>
    Farmer Player { get; }
}
