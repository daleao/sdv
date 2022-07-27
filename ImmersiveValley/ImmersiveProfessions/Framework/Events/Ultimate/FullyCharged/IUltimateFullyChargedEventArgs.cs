namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

/// <summary>Interface for the arguments of a <see cref="UltimateFullyChargedEvent"/>.</summary>
public interface IUltimateFullyChargedEventArgs
{
    /// <summary>The player who triggered the event.</summary>
    Farmer Player { get; }
}