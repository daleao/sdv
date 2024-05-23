namespace DaLion.Ponds;

#region using directives

using DaLion.Shared.Integrations.GMCM;

#endregion using directives

internal sealed class PondsConfigMenu : GMCMBuilder<PondsConfigMenu>
{
    /// <summary>Initializes a new instance of the <see cref="PondsConfigMenu"/> class.</summary>
    internal PondsConfigMenu()
        : base(ModHelper.Translation, ModHelper.ModRegistry, PondsMod.Manifest)
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
        Config = new PondsConfig();
    }

    /// <inheritdoc />
    protected override void SaveAndApply()
    {
        ModHelper.WriteConfig(Config);
    }
}
