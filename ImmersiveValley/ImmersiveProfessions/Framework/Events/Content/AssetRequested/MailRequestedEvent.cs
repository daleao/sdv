namespace DaLion.Stardew.Professions.Framework.Events.Content;

#region using directives

using System;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class MailRequestedEvent : AssetRequestedEvent
{
    /// <inheritdoc />
    protected override void OnAssetRequestedImpl(object sender, AssetRequestedEventArgs e)
    {
        if (!e.NameWithoutLocale.IsEquivalentTo("Data/mail")) return;

        e.Edit(asset =>
        {
            // patch mail from the Ferngill Revenue Service
            var data = asset.AsDictionary<string, string>().Data;
            var taxBonus = Game1.player.ReadDataAs<float>(ModData.ConservationistActiveTaxBonusPct);
            var key = taxBonus >= ModEntry.Config.ConservationistTaxBonusCeiling
                ? "conservationist.mail.max"
                : "conservationist.mail";
            var honorific = ModEntry.i18n.Get("honorific" + (Game1.player.IsMale ? ".male" : ".female"));
            var farm = Game1.getFarm().Name;
            var season = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr
                ? ModEntry.i18n.Get("season." + Game1.currentSeason)
                : Game1.CurrentSeasonDisplayName;

            string message = ModEntry.i18n.Get(key,
                new {honorific, taxBonus = FormattableString.CurrentCulture($"{taxBonus:p0}"), farm, season});
            data[$"{ModEntry.Manifest.UniqueID}/ConservationistTaxNotice"] = message;
        });
    }
}