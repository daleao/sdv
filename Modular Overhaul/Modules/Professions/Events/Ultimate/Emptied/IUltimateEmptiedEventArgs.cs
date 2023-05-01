namespace DaLion.Overhaul.Modules.Professions.Events.Ultimate;

/// <summary>Interface for the arguments of an <see cref="UltimateEmptiedEvent"/>.</summary>
public interface IUltimateEmptiedEventArgs
{
    /// <summary>Gets the player who triggered the event.</summary>
    Farmer Player { get; }
}
