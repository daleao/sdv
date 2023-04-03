namespace DaLion.Overhaul.Modules.Taxes.Events;

#region using directives

using DaLion.Shared.Content;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
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
        this.Edit("Data/mail", new AssetEditor(EditMailData, AssetEditPriority.Default));
    }

    private static void EditMailData(IAssetData asset)
    {
        // patch mail from the Ferngill Revenue Service
        var data = asset.AsDictionary<string, string>().Data;

        // FRS letters
        string honorific = I18n.Get("honorific" + (Game1.player.IsMale ? ".male" : ".female"));
        var player = Game1.player;
        var farm = Game1.getFarm();
        var interest = CurrentCulture($"{TaxesModule.Config.AnnualInterest:0.#%}");

        data[$"{Manifest.UniqueID}/{Mail.FrsIntro}"] =
            I18n.Get("frs.intro", new
            {
                honorific,
                farm = player.farmName,
                fine = CurrentCulture($"{TaxesModule.Config.IncomeTaxLatenessFine:0.#%}"),
                interest,
            });

        var due = TaxesModule.State.LatestDueIncomeTax;
        data[$"{Manifest.UniqueID}/{Mail.FrsNotice}"] = I18n.Get("frs.notice", new { honorific, due });

        var outstanding = TaxesModule.State.LatestOutstandingIncomeTax;
        data[$"{Manifest.UniqueID}/{Mail.FrsOutstanding}"] =
            I18n.Get("frs.outstanding", new
            {
                honorific,
                due,
                fine = CurrentCulture($"{TaxesModule.Config.IncomeTaxLatenessFine:0.#%}"),
                farm = player.farmName,
                outstanding,
                interest,
            });

        var deductions = TaxesModule.State.LatestTaxDeductions;
        data[$"{Manifest.UniqueID}/{Mail.FrsDeduction}"] = deductions switch
        {
            >= 1f => I18n.Get("frs.deduction.max", new { honorific }),
            >= 0f => I18n.Get(
                "frs.deduction",
                new { honorific, deductible = CurrentCulture($"{deductions:0.#%}") }),
            _ => string.Empty,
        };

        // county letters
        due = TaxesModule.State.LatestDuePropertyTax;
        var agricultureValue = farm.Read<int>(DataKeys.AgricultureValue);
        var livestockValue = farm.Read<int>(DataKeys.LivestockValue);
        var buildingValue = farm.Read<int>(DataKeys.BuildingValue);
        var valuation = agricultureValue + livestockValue + buildingValue;
        data[$"{Manifest.UniqueID}/{Mail.LewisNotice}"] =
            I18n.Get("lewis.notice", new { farm = player.farmName, valuation, due });

        outstanding = TaxesModule.State.LatestOutstandingPropertyTax;
        data[$"{Manifest.UniqueID}/{Mail.LewisOutstanding}"] = I18n.Get("lewis.outstanding", new
            {
                farm = player.farmName,
                valuation,
                due,
                fine = CurrentCulture($"{TaxesModule.Config.PropertyTaxLatenessFine:0.#%}"),
                outstanding,
                interest,
            });
    }
}
