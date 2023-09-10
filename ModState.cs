#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
namespace DaLion.Overhaul;

/// <summary>The collection of state for each module.</summary>
internal sealed class ModState
{
    internal Modules.Professions.State Professions { get; set; } = new();

    internal Modules.Combat.State Combat { get; set; } = new();

    internal Modules.Tools.State Tools { get; set; } = new();

    internal bool DebugMode { get; set; }
}
#pragma warning restore CS1591
#pragma warning restore SA1600
