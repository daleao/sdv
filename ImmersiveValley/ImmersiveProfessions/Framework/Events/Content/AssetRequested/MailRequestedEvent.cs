namespace DaLion.Stardew.Professions.Framework.Events.Content;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal class MailRequestedEvent : AssetRequestedEvent
{
    /// <inheritdoc />
    protected override void OnAssetRequestedImpl(object sender, AssetRequestedEventArgs e)
    {
        if (!e.NameWithoutLocale.IsEquivalentTo("Data/mail")) return;

        e.Edit(asset =>
        {
            // patch mail from the Ferngill Revenue Service
            var data = asset.AsDictionary<string, string>().Data;
            var taxBonus = Game1.player.ReadDataAs<float>(DataField.ConservationistActiveTaxBonusPct);
            var key = taxBonus >= ModEntry.Config.TaxDeductionCeiling
                ? "conservationist.mail2"
                : "conservationist.mail1";

            string message = ModEntry.i18n.Get(key,
                new { taxBonus = $"{taxBonus:p0}", farmName = Game1.getFarm().Name });
            data[$"{ModEntry.Manifest.UniqueID}/ConservationistTaxNotice"] = message;
        });
    }
}