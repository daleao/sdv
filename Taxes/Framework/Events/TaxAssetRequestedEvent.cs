namespace DaLion.Taxes.Framework.Events;

#region using directives

using DaLion.Shared.Content;
using DaLion.Shared.Events;
using static System.FormattableString;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class TaxAssetRequestedEvent : AssetRequestedEvent
{
    /// <summary>Initializes a new instance of the <see cref="TaxAssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal TaxAssetRequestedEvent(EventManager? manager = null)
        : base(manager ?? TaxesMod.EventManager)
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
        var interest = CurrentCulture($"{Config.AnnualInterest:0.#%}");

        data[$"{UniqueId}/{Mail.FrsIntro}"] =
            I18n.Mail_Frs_Intro(
                honorific,
                player.farmName.Value,
                CurrentCulture($"{Config.IncomeTaxLatenessFine:0.#%}"),
                interest);

        var due = Data.ReadAs<int>(player, DataKeys.LatestDueIncomeTax);
        data[$"{UniqueId}/{Mail.FrsNotice}"] = I18n.Mail_Frs_Notice(honorific, due);

        var outstanding = Data.ReadAs<int>(player, DataKeys.LatestOutstandingIncomeTax);
        data[$"{UniqueId}/{Mail.FrsOutstanding}"] =
            I18n.Mail_Frs_Outstanding(
                honorific,
                due,
                CurrentCulture($"{Config.IncomeTaxLatenessFine:0.#%}"),
                player.farmName.Value,
                outstanding,
                interest);

        var deductions = Data.ReadAs<float>(player, DataKeys.LatestTaxDeductions);
        data[$"{UniqueId}/{Mail.FrsDeduction}"] = deductions switch
        {
            >= 1f => I18n.Mail_Frs_Deduction_Max(honorific),
            >= 0f => I18n.Mail_Frs_Deduction(honorific, CurrentCulture($"{deductions:0.#%}")),
            _ => string.Empty,
        };

        // county letters
        due = Data.ReadAs<int>(player, DataKeys.LatestDuePropertyTax);
        var agricultureValue = Data.ReadAs<int>(farm, DataKeys.AgricultureValue);
        var livestockValue = Data.ReadAs<int>(farm, DataKeys.LivestockValue);
        var buildingValue = Data.ReadAs<int>(farm, DataKeys.BuildingValue);
        var valuation = agricultureValue + livestockValue + buildingValue;
        data[$"{UniqueId}/{Mail.LewisNotice}"] = I18n.Mail_Lewis_Notice(player.farmName.Value, valuation, due);

        outstanding = Data.ReadAs<int>(player, DataKeys.LatestOutstandingPropertyTax);
        data[$"{UniqueId}/{Mail.LewisOutstanding}"] = I18n.Mail_Lewis_Outstanding(
                player.farmName.Value,
                valuation,
                due,
                CurrentCulture($"{Config.PropertyTaxLatenessFine:0.#%}"),
                outstanding,
                interest);

        Data.Write(player, DataKeys.LatestDueIncomeTax, string.Empty);
        Data.Write(player, DataKeys.LatestOutstandingIncomeTax, string.Empty);
        Data.Write(player, DataKeys.LatestDuePropertyTax, string.Empty);
        Data.Write(player, DataKeys.LatestOutstandingPropertyTax, string.Empty);
        Data.Write(player, DataKeys.LatestTaxDeductions, string.Empty);
    }
}
