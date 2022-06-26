namespace DaLion.Stardew.Taxes.Framework.Events;

#region using directives

using Common.Data;
using Common.Events;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;
using static System.FormattableString;

#endregion using directives

[UsedImplicitly]
internal sealed class TaxAssetRequestedEvent : AssetRequestedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal TaxAssetRequestedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnAssetRequestedImpl(object? sender, AssetRequestedEventArgs e)
    {
        if (!e.NameWithoutLocale.IsEquivalentTo("Data/mail")) return;

        e.Edit(asset =>
        {
            // patch mail from the Ferngill Revenue Service
            var data = asset.AsDictionary<string, string>().Data;
            data[$"{ModEntry.Manifest.UniqueID}/TaxIntro"] = ModEntry.i18n.Get("tax.intro");

            var due = ModEntry.LatestAmountDue.Value.ToString();
            var deductible = ModDataIO.ReadDataAs<float>(Game1.player, ModData.DeductionPct.ToString());
            var outstanding = ModDataIO.ReadDataAs<int>(Game1.player, ModData.DebtOutstanding.ToString()).ToString();
            var honorific = ModEntry.i18n.Get("honorific" + (Game1.player.IsMale ? ".male" : ".female"));
            var farm = Game1.getFarm().Name;
            var interest = CurrentCulture($"{ModEntry.Config.AnnualInterest:p0}");

            data[$"{ModEntry.Manifest.UniqueID}/TaxNotice"] = ModEntry.i18n.Get("tax.notice", new { honorific, due });
            data[$"{ModEntry.Manifest.UniqueID}/TaxOutstanding"] =
                ModEntry.i18n.Get("tax.outstanding", new { honorific, due, outstanding, farm, interest, });
#pragma warning disable CS8509
            data[$"{ModEntry.Manifest.UniqueID}/TaxDeduction"] = deductible switch
#pragma warning restore CS8509
            {
                >= 1f => ModEntry.i18n.Get("tax.deduction.max", new { honorific }),
                >= 0f => ModEntry.i18n.Get("tax.deduction",
                    new { honorific, deductible = CurrentCulture($"{deductible:p0}") })
            };
        });
    }
}