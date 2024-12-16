namespace DaLion.Combat;

#region using directives

using DaLion.Shared.Integrations.GMCM;

#endregion using directives

internal sealed class CombatConfigMenu : GMCMBuilder<CombatConfigMenu>
{
    /// <summary>Initializes a new instance of the <see cref="CombatConfigMenu"/> class.</summary>
    internal CombatConfigMenu()
        : base(ModHelper.Translation, ModHelper.ModRegistry, CombatMod.Manifest)
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
        Config = new CombatConfig();
    }

    /// <inheritdoc />
    protected override void SaveAndApply()
    {
        ModHelper.WriteConfig(Config);
    }
}
