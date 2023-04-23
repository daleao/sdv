namespace DaLion.Overhaul.Modules.Enchantments;

/// <summary>The runtime state variables for ENCH.</summary>
internal sealed class State
{
    internal bool DidArtfulParry { get; set; }

    internal bool GatlingModeEngaged { get; set; }

    internal int DoublePressTimer { get; set; }
}
