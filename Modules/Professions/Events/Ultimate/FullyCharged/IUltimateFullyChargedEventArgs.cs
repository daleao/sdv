namespace DaLion.Overhaul.Modules.Professions.Events.Ultimate.FullyCharged;

/// <summary>Interface for the arguments of a <see cref="UltimateFullyChargedEvent"/>.</summary>
public interface IUltimateFullyChargedEventArgs
{
    /// <summary>Gets the player who triggered the event.</summary>
    Farmer Player { get; }
}
