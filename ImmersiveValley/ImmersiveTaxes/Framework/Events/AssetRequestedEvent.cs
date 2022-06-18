namespace DaLion.Stardew.Taxes.Framework.Events;

#region using directives

using static System.FormattableString;
using System;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

using Extensions;

#endregion using directives

/// <summary>Wrapper for <see cref="IContentEvents.AssetRequested"/> that can be hooked or unhooked.</summary>
[UsedImplicitly]
internal sealed class AssetRequestedEvent : IEvent
{
    /// <inheritdoc />
    public void Hook()
    {
        ModEntry.ModHelper.Events.Content.AssetRequested += OnAssetRequested;
        Log.D("[Taxes] Hooked AssetRequested event.");
    }

    /// <inheritdoc />
    public void Unhook()
    {
        ModEntry.ModHelper.Events.Content.AssetRequested -= OnAssetRequested;
        Log.D("[Taxes] Unhooked AssetRequested event.");
    }

    /// <inheritdoc cref="IContentEvents.AssetRequested"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
    {
        if (!e.NameWithoutLocale.IsEquivalentTo("Data/mail")) return;

        e.Edit(asset =>
        {
            // patch mail from the Ferngill Revenue Service
            var data = asset.AsDictionary<string, string>().Data;
            data[$"{ModEntry.Manifest.UniqueID}/TaxIntro"] = ModEntry.i18n.Get("tax.intro");

            var due = ModEntry.LatestAmountDue.Value.ToString();
            var deductible = Game1.player.ReadDataAs<float>(ModData.DeductionPct);
            var outstanding = Game1.player.ReadDataAs<int>(ModData.DebtOutstanding).ToString();
            var honorific = ModEntry.i18n.Get("honorific" + (Game1.player.IsMale ? ".male" : ".female"));
            var farm = Game1.getFarm().Name;
            var interest = CurrentCulture($"{ModEntry.Config.AnnualInterest:p0}");

            data[$"{ModEntry.Manifest.UniqueID}/TaxNotice"] = ModEntry.i18n.Get("tax.notice", new {honorific, due});
            data[$"{ModEntry.Manifest.UniqueID}/TaxOutstanding"] =
                ModEntry.i18n.Get("tax.outstanding", new {honorific, due, outstanding, farm, interest,});
#pragma warning disable CS8509
            data[$"{ModEntry.Manifest.UniqueID}/TaxDeduction"] = deductible switch
#pragma warning restore CS8509
            {
                >= 1f => ModEntry.i18n.Get("tax.deduction.max", new {honorific}),
                >= 0f => ModEntry.i18n.Get("tax.deduction",
                    new {honorific, deductible = CurrentCulture($"{deductible:p0}")})
            };
        });
    }
}