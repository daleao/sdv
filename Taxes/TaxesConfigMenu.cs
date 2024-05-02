namespace DaLion.Taxes;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Shared.Extensions;
using DaLion.Shared.Integrations.GMCM;

#endregion using directives

internal class TaxesConfigMenu : GMCMBuilder<TaxesConfigMenu>
{
    /// <summary>Initializes a new instance of the <see cref="TaxesConfigMenu"/> class.</summary>
    internal TaxesConfigMenu()
        : base(ModHelper.Translation, ModHelper.ModRegistry, TaxesMod.Manifest)
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
        Config = new TaxesConfig();
    }

    /// <inheritdoc />
    protected override void SaveAndApply()
    {
        ModHelper.WriteConfig(Config);
    }

    [UsedImplicitly]
    private static void TaxByIncomeBracketOverride()
    {
        Instance!.AddDynamicKeyValuePairListOption(
            I18n.Gmcm_TaxRatePerIncomeBracket_Title,
            I18n.Gmcm_TaxRatePerIncomeBracket_Desc,
            () => Config.TaxRatePerIncomeBracket.Select(pair => new KeyValuePair<string, string>($"{pair.Key}", $"{pair.Value}")).ToList(),
            pairs =>
            {
                var parsedPairs = new List<KeyValuePair<int, float>>();
                for (var i = 0; i < pairs.Count; i++)
                {
                    var pair = pairs[i];
                    if (!int.TryParse(pair.Key, out var bracket))
                    {
                        Log.W(
                            $"Failed to change the tax bracket at position {i / 2}. The key `{pair.Key}` is invalid. Please make sure that it is a valid integer.");
                    }
                    else if (!float.TryParse(pair.Value, out var tax))
                    {
                        Log.W(
                            $"Failed to change the tax rate at position {i / 2}. The value `{pair.Value}` is invalid. Please make sure that it is a valid decimal.");
                    }
                    else
                    {
                        parsedPairs.Add(new KeyValuePair<int, float>(bracket, tax));
                    }
                }

                Config.TaxRatePerIncomeBracket = parsedPairs.ToDictionary(pair => pair.Key, value => value.Value);
            },
            i => i % 2 == 0 ? I18n.Gmcm_IncomeBracket_Title() : I18n.Gmcm_TaxRate_Title(),
            i => i % 2 == 0 ? I18n.Gmcm_IncomeBracket_Desc() : I18n.Gmcm_TaxRate_Desc(),
            enumerateLabels: true,
            id: "TaxRatePerIncomeBracket");
    }

    [UsedImplicitly]
    private static void DeductibleExtrasOverride()
    {
        Instance!.AddDynamicKeyValuePairListOption(
            I18n.Gmcm_DeductibleExtras_Title,
            I18n.Gmcm_DeductibleExtras_Desc,
            () => Config.DeductibleExtras.Select(pair => new KeyValuePair<string, string>(pair.Key.TrimAll(), $"{pair.Value}")).ToList(),
            pairs =>
            {
                var parsedPairs = new List<KeyValuePair<string, float>>();
                foreach (var pair in pairs)
                {
                    if (!float.TryParse(pair.Value, out var deductible))
                    {
                        Log.W(
                            $"Failed to change the deduction rate for item {pair.Key}. The value `{pair.Value}` is invalid. Please make sure that it is a valid decimal.");
                    }
                    else
                    {
                        parsedPairs.Add(new KeyValuePair<string, float>(pair.Key, deductible));
                    }
                }

                Config.DeductibleExtras = parsedPairs.ToDictionary(pair => pair.Key, value => value.Value);
            },
            i => i % 2 == 0 ? I18n.Gmcm_DeductibleExtras_Label_Key() : I18n.Gmcm_DeductibleExtras_Label_Value(),
            id: "DeductibleExtras");
    }
}
