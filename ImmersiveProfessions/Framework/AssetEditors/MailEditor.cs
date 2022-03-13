namespace DaLion.Stardew.Professions.Framework.AssetEditors;

#region using directives

using System;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;

using Extensions;

#endregion using directives

/// <summary>Edits <c>Data/mail</c> with Conservationist taxation notice.</summary>
public class MailEditor : IAssetEditor
{
    /// <inheritdoc />
    public bool CanEdit<T>(IAssetInfo asset)
    {
        return asset.AssetNameEquals(PathUtilities.NormalizeAssetName("Data/mail"));
    }

    /// <inheritdoc />
    public void Edit<T>(IAssetData asset)
    {
        if (!asset.AssetNameEquals(PathUtilities.NormalizeAssetName("Data/mail")))
            throw new InvalidOperationException($"Unexpected asset {asset.AssetName}.");

        // patch mail from the Ferngill Revenue Service
        var data = asset.AsDictionary<string, string>().Data;
        var taxBonus = Game1.player.ReadDataAs<float>(DataField.ConservationistActiveTaxBonusPct);
        var key = taxBonus >= ModEntry.Config.TaxDeductionCeiling
            ? "conservationist.mail2"
            : "conservationist.mail1";

        string message = ModEntry.ModHelper.Translation.Get(key,
            new {taxBonus = $"{taxBonus:p0}", farmName = Game1.getFarm().Name});
        data[$"{ModEntry.Manifest.UniqueID}/ConservationistTaxNotice"] = message;
    }
}