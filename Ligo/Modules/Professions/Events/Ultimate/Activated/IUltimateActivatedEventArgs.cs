namespace DaLion.Ligo.Modules.Professions.Events.Ultimate;

/// <summary>Interface for the arguments of an <see cref="UltimateActivatedEvent"/>.</summary>
public interface IUltimateActivatedEventArgs
{
    /// <summary>Gets the player who triggered the event.</summary>
    Farmer Player { get; }
}
