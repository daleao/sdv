namespace DaLion.Core;

#region using directives

using DaLion.Shared.Classes;

#endregion using directives

/// <summary>Runtime state schema for the Core mod.</summary>
/// <remarks>This is public to be used by other mods.</remarks>
public sealed class CoreState
{
    /// <summary>Gets a value indicating whether enemies are nearby the player.</summary>
    public bool AreEnemiesNearby { get; internal set; }

    /// <summary>Gets the number of seconds since the last taking or receiving damage.</summary>
    public int SecondsOutOfCombat { get; internal set; } = int.MaxValue;

    /// <summary>Gets a list of <see cref="Timer"/>s managed by the core mod.</summary>
    public List<Timer> Timers { get; } = [];

    internal bool DebugMode { get; set; }

    internal FrameRateCounter? FpsCounter { get; set; }
}
