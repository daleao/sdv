namespace DaLion.Stardew.Arsenal.Framework.AssetEditors;

#region using directives

using System;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Edits all game assets required for the truly legendary galaxy sword.</summary>
public class TrulyLegendaryGalaxyEditor : IAssetEditor
{
    /// <inheritdoc />
    public bool CanEdit<T>(IAssetInfo asset)
    {
        return asset.AssetNameEquals(PathUtilities.NormalizeAssetName("Data/Quests")) ||
               asset.AssetNameEquals(PathUtilities.NormalizeAssetName("Data/mail")) ||
               asset.AssetNameEquals(PathUtilities.NormalizeAssetName("Strings/Locations")) ||
               asset.AssetNameEquals(PathUtilities.NormalizeAssetName("String/StringsFromCSFiles"));
    }

    /// <inheritdoc />
    public void Edit<T>(IAssetData asset)
    {
        if (!ModEntry.Config.TrulyLegendaryGalaxySword) return;

        if (asset.AssetNameEquals(PathUtilities.NormalizeAssetName("Data/Quests")))
        {
            var data = asset.AsDictionary<int, string>().Data;
            var title = ModEntry.ModHelper.Translation.Get("quests.QiChallengeFinal.title");
            var description = ModEntry.ModHelper.Translation.Get("quests.QiChallengeFinal.desc");
            var objective = ModEntry.ModHelper.Translation.Get("quests.QiChallengeFinal.obj");
            data[ModEntry.QiChallengeFinalQuestId] = $"Basic/{title}/{description}/{objective}/-1/-1/0/-1/false";
        }
        else if (asset.AssetNameEquals(PathUtilities.NormalizeAssetName("Data/mail")))
        {
            var data = asset.AsDictionary<string, string>().Data;
            data["MeleeWeapon"] = ModEntry.ModHelper.Translation.Get("mail.skullCave");
            data["QiChallengeFirst"] = ModEntry.ModHelper.Translation.Get("mail.QiChallengeFirst", new {ModEntry.QiChallengeFinalQuestId});
            data["QiChallengeComplete"] = ModEntry.ModHelper.Translation.Get("mail.QiChallengeComplete");
        }
        else if (asset.AssetNameEquals(PathUtilities.NormalizeAssetName("Strings/Locations")))
        {
            var data = asset.AsDictionary<string, string>().Data;
            data["Town_DwarfGrave_Translated"] = ModEntry.ModHelper.Translation.Get("locations.Town_DwarfGrave_Translated");
        }
        else if (asset.AssetNameEquals(PathUtilities.NormalizeAssetName("Strings/StringsFromCSFiles")))
        {
            var data = asset.AsDictionary<string, string>().Data;
            data["MeleeWeapon.cs.14122"] = ModEntry.ModHelper.Translation.Get("csfiles.MeleeWeapon.cs.14122");
        }
        else
        {
            throw new InvalidOperationException($"Unexpected asset {asset.AssetName}.");
        }
    }
}