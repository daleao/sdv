namespace DaLion.Stardew.Taxes.Framework.Events;

#region using directives

using Common.Events;
using Common.Extensions.Stardew;
using StardewModdingAPI.Events;
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

            var due = ModEntry.LatestAmountDue.Value.ToString();
            var deductible = Game1.player.Read<float>("DeductionPct");
            var outstanding = Game1.player.Read("DebtOutstanding");
            var honorific = ModEntry.i18n.Get("honorific" + (Game1.player.IsMale ? ".male" : ".female"));
            var farm = Game1.getFarm().Name;
            var interest = CurrentCulture($"{ModEntry.Config.AnnualInterest:p0}");

            data[$"{ModEntry.Manifest.UniqueID}/TaxIntro"] =
                ModEntry.i18n.Get("tax.intro", new { honorific, farm = Game1.getFarm().Name, interest });
            data[$"{ModEntry.Manifest.UniqueID}/TaxNotice"] = ModEntry.i18n.Get("tax.notice", new { honorific, due });
            data[$"{ModEntry.Manifest.UniqueID}/TaxOutstanding"] =
                ModEntry.i18n.Get("tax.outstanding", new { honorific, due, outstanding, farm, interest, });
            data[$"{ModEntry.Manifest.UniqueID}/TaxDeduction"] = deductible switch
            {
                >= 1f => ModEntry.i18n.Get("tax.deduction.max", new { honorific }),
                >= 0f => ModEntry.i18n.Get("tax.deduction",
                    new { honorific, deductible = CurrentCulture($"{deductible:p0}") }),
                _ => string.Empty
            };
        });
    }
}