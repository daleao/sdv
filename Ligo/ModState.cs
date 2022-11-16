namespace DaLion.Ligo;

/// <summary>Holds the runtime state variables of all the modules.</summary>
internal sealed class ModState
{
    /// <summary>Gets or sets the Arsenal module runtime state.</summary>
    internal Modules.Arsenal.State Arsenal { get; set; } = new();

    /// <summary>Gets or sets the Professions module runtime state.</summary>
    internal Modules.Professions.State Professions { get; set; } = new();

    /// <summary>Gets or sets the Rings module runtime state.</summary>
    internal Modules.Rings.State Rings { get; set; } = new();

    /// <summary>Gets or sets the Taxes module runtime state.</summary>
    internal Modules.Taxes.State Taxes { get; set; } = new();

    /// <summary>Gets or sets the Tools module runtime state.</summary>
    internal Modules.Tools.State Tools { get; set; } = new();
}
