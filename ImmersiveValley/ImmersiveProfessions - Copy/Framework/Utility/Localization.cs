namespace DaLion.Stardew.Professions.Framework.Utility;

#region using directives

using StardewValley;

using Ultimates;

#endregion using directives

internal static class Localization
{
    /// <summary>Get the localized pronoun for the currently registered Ultimate buff.</summary>
    internal static string GetBuffPronoun()
    {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (LocalizedContentManager.CurrentLanguageCode)
        {
            case LocalizedContentManager.LanguageCode.es:
                return ModEntry.i18n.Get("pronoun.definite.female");

            case LocalizedContentManager.LanguageCode.fr:
            case LocalizedContentManager.LanguageCode.pt:
                return ModEntry.i18n.Get("pronoun.definite" +
                                                          (ModEntry.Player.RegisteredUltimate is Ambush
                                                              ? ".male"
                                                              : ".female"));

            default:
                return string.Empty;
        }
    }
}