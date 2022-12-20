namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenuForOverhaul
{
    /// <summary>Register the Ponds menu.</summary>
    private void RegisterPonds()
    {
        this
            .AddPage(OverhaulModule.Ponds.Namespace, () => "Pond Settings")
            .AddNumberField(
                () => "Roe Production Chance Multiplier",
                () => "Multiplies a fish's base chance to produce roe each day.",
                config => config.Ponds.RoeProductionChanceMultiplier,
                (config, value) => config.Ponds.RoeProductionChanceMultiplier = value,
                0.1f,
                2f);
    }
}
