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
            I18n.Mail_Frs_Intro(
                honorific,
                player.farmName.Value,
                CurrentCulture($"{TaxesModule.Config.IncomeTaxLatenessFine:0.#%}"),
                interest);

        var due = Game1.player.Read<int>(DataKeys.LatestDueIncomeTax);
        data[$"{Manifest.UniqueID}/{Mail.FrsNotice}"] = I18n.Mail_Frs_Notice(honorific, due);

        var outstanding = Game1.player.Read<int>(DataKeys.LatestOutstandingIncomeTax);
        data[$"{Manifest.UniqueID}/{Mail.FrsOutstanding}"] =
            I18n.Mail_Frs_Outstanding(
                honorific,
                due,
                CurrentCulture($"{TaxesModule.Config.IncomeTaxLatenessFine:0.#%}"),
                player.farmName.Value,
                outstanding,
                interest);

        var deductions = Game1.player.Read<float>(DataKeys.LatestTaxDeductions);
        data[$"{Manifest.UniqueID}/{Mail.FrsDeduction}"] = deductions switch
        {
            >= 1f => I18n.Mail_Frs_Deduction_Max(honorific),
            >= 0f => I18n.Mail_Frs_Deduction(honorific, CurrentCulture($"{deductions:0.#%}")),
            _ => string.Empty,
        };

        // county letters
        due = Game1.player.Read<int>(DataKeys.LatestDuePropertyTax);
        var agricultureValue = farm.Read<int>(DataKeys.AgricultureValue);
        var livestockValue = farm.Read<int>(DataKeys.LivestockValue);
        var buildingValue = farm.Read<int>(DataKeys.BuildingValue);
        var valuation = agricultureValue + livestockValue + buildingValue;
        data[$"{Manifest.UniqueID}/{Mail.LewisNotice}"] = I18n.Mail_Lewis_Notice(player.farmName.Value, valuation, due);

        outstanding = Game1.player.Read<int>(DataKeys.LatestOutstandingPropertyTax);
        data[$"{Manifest.UniqueID}/{Mail.LewisOutstanding}"] = I18n.Mail_Lewis_Outstanding(
                player.farmName.Value,
                valuation,
                due,
                CurrentCulture($"{TaxesModule.Config.PropertyTaxLatenessFine:0.#%}"),
                outstanding,
                interest);

        Game1.player.Write(DataKeys.LatestDueIncomeTax, string.Empty);
        Game1.player.Write(DataKeys.LatestOutstandingIncomeTax, string.Empty);
        Game1.player.Write(DataKeys.LatestDuePropertyTax, string.Empty);
        Game1.player.Write(DataKeys.LatestOutstandingPropertyTax, string.Empty);
        Game1.player.Write(DataKeys.LatestTaxDeductions, string.Empty);
    }
}
