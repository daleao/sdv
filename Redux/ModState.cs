namespace DaLion.Redux;

#region using directives

using StardewValley.Menus;

#endregion using directives

/// <summary>Holds the runtime state variables of all the modules.</summary>
internal sealed class ModState
{
    /// <summary>Gets or sets the Arsenal module runtime state.</summary>
    internal Arsenal.State Arsenal { get; set; } = new();

    /// <summary>Gets or sets the Professions module runtime state.</summary>
    internal Professions.State Professions { get; set; } = new();

    /// <summary>Gets or sets the Rings module runtime state.</summary>
    internal Rings.State Rings { get; set; } = new();

    /// <summary>Gets or sets the Taxes module runtime state.</summary>
    internal Taxes.State Taxes { get; set; } = new();

    /// <summary>Gets or sets the Tools module runtime state.</summary>
    internal Tools.State Tools { get; set; } = new();
}
