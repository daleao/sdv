namespace DaLion.Arsenal;

#region using directives

using DaLion.Arsenal.Framework.Enums;
using DaLion.Shared.Integrations.GMCM;

#endregion using directives

internal sealed class ArsenalConfigMenu : GMCMBuilder<ArsenalConfigMenu>
{
    /// <summary>Initializes a new instance of the <see cref="ArsenalConfigMenu"/> class.</summary>
    internal ArsenalConfigMenu()
        : base(ModHelper.Translation, ModHelper.ModRegistry, ArsenalMod.Manifest)
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
        Config = new ArsenalConfig();
    }

    /// <inheritdoc />
    protected override void SaveAndApply()
    {
        ModHelper.WriteConfig(Config);
    }

    [UsedImplicitly]
    private static void CombatConfigColorByTierOverride()
    {
        for (var i = 0; i < Config.ColorByTier.Length; i++)
        {
            var tier = (WeaponTier)i;
            var @default = Config.ColorByTier[i];
            Instance!.AddColorPicker(
                () => Instance!._I18n.Get($"gmcm.color_by_tier.{tier.Name.ToLower()}.title"),
                () => Instance!._I18n.Get($"gmcm.color_by_tier.{tier.Name.ToLower()}.desc"),
                config => config.ColorByTier[tier],
                (config, value) => config.ColorByTier[tier] = value,
                () => Config,
                @default,
                false,
                (uint)IGenericModConfigMenuOptionsApi.ColorPickerStyle.RGBSliders,
                "ColorByTier." + tier.Name);
        }
    }
}
