namespace DaLion.Core;

#region using directives

using DaLion.Shared.Integrations.GMCM;

#endregion using directives

internal sealed class CoreConfigMenu : GMCMBuilder<CoreConfigMenu>
{
    /// <summary>Initializes a new instance of the <see cref="CoreConfigMenu"/> class.</summary>
    internal CoreConfigMenu()
        : base(ModHelper.Translation, ModHelper.ModRegistry, CoreMod.Manifest)
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
        Config = new CoreConfig();
    }

    /// <inheritdoc />
    protected override void SaveAndApply()
    {
        ModHelper.WriteConfig(Config);
    }
}
