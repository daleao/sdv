namespace DaLion.Enchantments;

#region using directives

using DaLion.Shared.Integrations.GMCM;

#endregion using directives

internal sealed class EnchantmentsConfigMenu : GMCMBuilder<EnchantmentsConfigMenu>
{
    /// <summary>Initializes a new instance of the <see cref="EnchantmentsConfigMenu"/> class.</summary>
    internal EnchantmentsConfigMenu()
        : base(ModHelper.Translation, ModHelper.ModRegistry, EnchantmentsMod.Manifest)
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
        Config = new EnchantmentsConfig();
    }

    /// <inheritdoc />
    protected override void SaveAndApply()
    {
        ModHelper.WriteConfig(Config);
    }
}
