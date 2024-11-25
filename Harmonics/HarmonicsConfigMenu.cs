namespace DaLion.Harmonics;

#region using directives

using DaLion.Shared.Integrations.GMCM;

#endregion using directives

internal sealed class HarmonicsConfigMenu : GMCMBuilder<HarmonicsConfigMenu>
{
    /// <summary>Initializes a new instance of the <see cref="HarmonicsConfigMenu"/> class.</summary>
    internal HarmonicsConfigMenu()
        : base(ModHelper.Translation, ModHelper.ModRegistry, HarmonicsMod.Manifest)
    {
    }

    /// <inheritdoc />
    protected override void BuildMenu()
    {
        this.BuildImplicitly(() => Config);
    }

    /// <inheritdoc />
    protected override void ResetConfig()
    {
        Config = new HarmonicsConfig();
    }

    /// <inheritdoc />
    protected override void SaveAndApply()
    {
        ModHelper.WriteConfig(Config);
    }
}
