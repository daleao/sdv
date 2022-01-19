namespace DaLion.Stardew.Professions.Framework.Utility;

#region using directives

using StardewValley;
using SuperMode;

#endregion using directives

public static class Localization
{
    /// <summary>Get the localized pronoun for the currently registered Super Mode buff.</summary>
    public static string GetBuffPronoun()
    {
        switch (LocalizedContentManager.CurrentLanguageCode)
        {
            case LocalizedContentManager.LanguageCode.es:
                return ModEntry.ModHelper.Translation.Get("pronoun.definite.female");

            case LocalizedContentManager.LanguageCode.fr:
            case LocalizedContentManager.LanguageCode.pt:
                return ModEntry.ModHelper.Translation.Get("pronoun.definite" +
                                                          (ModEntry.State.Value.SuperMode.Index ==
                                                           SuperModeIndex.Poacher
                                                              ? ".male"
                                                              : ".female"));

            default:
                return string.Empty;
        }
    }
}