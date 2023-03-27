namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenuCore
{
    /// <summary>Register the Ponds menu.</summary>
    private void RegisterPonds()
    {
        this
            .AddPage(OverhaulModule.Ponds.Namespace, () => "Pond Settings")

            .AddNumberField(
                () => "Days Until Algae Spawn",
                () => "The number of days until an empty pond will begin spawning algae.",
                config => (int)config.Ponds.DaysUntilAlgaeSpawn,
                (config, value) => config.Ponds.DaysUntilAlgaeSpawn = (uint)value,
                1,
                5)
            .AddNumberField(
                () => "Roe Production Chance Multiplier",
                () => "Multiplies a fish's base chance to produce roe each day.",
                config => config.Ponds.RoeProductionChanceMultiplier,
                (config, value) => config.Ponds.RoeProductionChanceMultiplier = value,
                0.1f,
                2f)
            .AddCheckbox(
            () => "Roe Quality Always Same As Fish",
            () =>
                "If true, then the quality of produced roe is always the same as the quality of the producing fish. If false, then the quality will be a random value less than or equal to that of the producing fish.",
            config => config.Ponds.RoeAlwaysSameQualityAsFish,
            (config, value) => config.Ponds.RoeAlwaysSameQualityAsFish = value);
    }
}
