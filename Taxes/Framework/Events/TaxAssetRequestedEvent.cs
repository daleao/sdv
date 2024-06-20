namespace DaLion.Taxes.Framework.Events;

#region using directives

using DaLion.Shared.Content;
using DaLion.Shared.Events;
using static System.FormattableString;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="TaxAssetRequestedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class TaxAssetRequestedEvent(EventManager? manager = null)
    : AssetRequestedEvent(manager ?? TaxesMod.EventManager)
{
    /// <inheritdoc />
    protected override void Initialize()
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

        var due = Data.ReadAs<int>(player, DataKeys.AccruedIncomeTax);
        var when = Config.IncomeTaxDay == 1
            ? I18n.When_Tonight()
            : I18n.When_Day(toDayOfMonthString(Config.IncomeTaxDay));
        data[$"{UniqueId}/{Mail.FrsNotice}"] = I18n.Mail_Frs_Notice(honorific, due, when);

        var deductions = Data.ReadAs<float>(player, DataKeys.PercentDeductions);
        data[$"{UniqueId}/{Mail.FrsDeduction}"] = deductions >= 1f
            ? I18n.Mail_Frs_Deduction_Max(honorific, due)
            : I18n.Mail_Frs_Deduction(
                honorific,
                due,
                CurrentCulture($"{deductions:0.#%}"),
                (int)(due * (1f - deductions)),
                when);

        var outstanding = Data.ReadAs<int>(player, DataKeys.OutstandingIncomeTax);
        data[$"{UniqueId}/{Mail.FrsOutstanding}"] =
            I18n.Mail_Frs_Outstanding(
                honorific,
                due,
                CurrentCulture($"{Config.IncomeTaxLatenessFine:0.#%}"),
                player.farmName.Value,
                outstanding,
                interest);

        // county letters
        due = Data.ReadAs<int>(player, DataKeys.AccruedPropertyTax);
        var agricultureValue = Data.ReadAs<int>(farm, DataKeys.AgricultureValue);
        var livestockValue = Data.ReadAs<int>(farm, DataKeys.LivestockValue);
        var buildingValue = Data.ReadAs<int>(farm, DataKeys.BuildingValue);
        var valuation = agricultureValue + livestockValue + buildingValue;
        data[$"{UniqueId}/{Mail.LewisNotice}"] = I18n.Mail_Lewis_Notice(
            player.farmName.Value,
            valuation,
            due,
            toDayOfMonthString(Config.PropertyTaxDay));

        outstanding = Data.ReadAs<int>(player, DataKeys.OutstandingPropertyTax);
        data[$"{UniqueId}/{Mail.LewisOutstanding}"] =
            I18n.Mail_Lewis_Outstanding(
                CurrentCulture($"{Config.PropertyTaxLatenessFine:0.#%}"),
                outstanding,
                interest);

        string toDayOfMonthString(int dayNumber)
        {
            return dayNumber switch
            {
                1 => "1st",
                2 => "2nd",
                3 => "3rd",
                _ => $"{dayNumber}th",
            };
        }
    }
}
