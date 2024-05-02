namespace DaLion.Chargeable;

#region using directives

using DaLion.Shared.Integrations.GMCM;

#endregion using directives

internal class ChargeableConfigMenu : GMCMBuilder<ChargeableConfigMenu>
{
    /// <summary>Initializes a new instance of the <see cref="ChargeableConfigMenu"/> class.</summary>
    internal ChargeableConfigMenu()
        : base(ModHelper.Translation, ModHelper.ModRegistry, ChargeableMod.Manifest)
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
        Config = new ChargeableConfig();
    }

    /// <inheritdoc />
    protected override void SaveAndApply()
    {
        ModHelper.WriteConfig(Config);
    }
}
