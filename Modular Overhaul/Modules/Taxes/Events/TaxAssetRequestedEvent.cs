namespace DaLion.Overhaul.Modules.Taxes.Events;

#region using directives

using DaLion.Shared.Content;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using static System.FormattableString;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class TaxAssetRequestedEvent : AssetRequestedEvent
{
    /// <summary>Initializes a new instance of the <see cref="TaxAssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal TaxAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        this.Edit("Data/mail", new AssetEditor(EditMailData));
    }

    private static void EditMailData(IAssetData asset)
    {
        // patch mail from the Ferngill Revenue Service
        var data = asset.AsDictionary<string, string>().Data;

        // FRS letters
        string honorific = _I18n.Get("honorific" + (Game1.player.IsMale ? ".male" : ".female"));
        var player = Game1.player;
        var farm = Game1.getFarm();
        var interest = CurrentCulture($"{TaxesModule.Config.AnnualInterest:0.#%}");

        data[$"{Manifest.UniqueID}/{Mail.FrsIntro}"] =
            I18n.Frs_Intro(
                honorific,
                player.farmName.Value,
                CurrentCulture($"{TaxesModule.Config.IncomeTaxLatenessFine:0.#%}"),
                interest);

        var due = TaxesModule.State.LatestDueIncomeTax;
        data[$"{Manifest.UniqueID}/{Mail.FrsNotice}"] = I18n.Frs_Notice(honorific, due);

        var outstanding = TaxesModule.State.LatestOutstandingIncomeTax;
        data[$"{Manifest.UniqueID}/{Mail.FrsOutstanding}"] =
            I18n.Frs_Outstanding(
                honorific,
                due,
                CurrentCulture($"{TaxesModule.Config.IncomeTaxLatenessFine:0.#%}"),
                player.farmName.Value,
                outstanding,
                interest);

        var deductions = TaxesModule.State.LatestTaxDeductions;
        data[$"{Manifest.UniqueID}/{Mail.FrsDeduction}"] = deductions switch
        {
            >= 1f => I18n.Frs_Deduction_Max(honorific),
            >= 0f => I18n.Frs_Deduction(honorific, CurrentCulture($"{deductions:0.#%}")),
            _ => string.Empty,
        };

        // county letters
        due = TaxesModule.State.LatestDuePropertyTax;
        var agricultureValue = farm.Read<int>(DataKeys.AgricultureValue);
        var livestockValue = farm.Read<int>(DataKeys.LivestockValue);
        var buildingValue = farm.Read<int>(DataKeys.BuildingValue);
        var valuation = agricultureValue + livestockValue + buildingValue;
        data[$"{Manifest.UniqueID}/{Mail.LewisNotice}"] = I18n.Lewis_Notice(player.farmName.Value, valuation, due);

        outstanding = TaxesModule.State.LatestOutstandingPropertyTax;
        data[$"{Manifest.UniqueID}/{Mail.LewisOutstanding}"] = I18n.Lewis_Outstanding(
                player.farmName.Value,
                valuation,
                due,
                CurrentCulture($"{TaxesModule.Config.PropertyTaxLatenessFine:0.#%}"),
                outstanding,
                interest);
    }
}
