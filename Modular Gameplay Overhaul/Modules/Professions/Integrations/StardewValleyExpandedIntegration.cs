namespace DaLion.Overhaul.Modules.Professions.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Integrations;

#endregion using directives

[RequiresMod("FlashShifter.StardewValleyExpandedCP", "StardewValleyExpanded")]
internal sealed class StardewValleyExpandedIntegration : ModIntegration<StardewValleyExpandedIntegration>
{
    private StardewValleyExpandedIntegration()
        : base(
            "FlashShifter.StardewValleyExpandedCP",
            "StardewValleyExpanded",
            null,
            ModHelper.ModRegistry)
    {
    }

    /// <summary>Gets a value indicating whether the <c>DisableGaldoranTheme</c> config setting is enabled.</summary>
    internal bool DisabeGaldoranTheme => this.IsLoaded && ModHelper
        .ReadContentPackConfig("FlashShifter.StardewValleyExpandedCP")
        ?.Value<bool?>("DisableGaldoranTheme") == true;

    /// <summary>Gets a value indicating whether the <c>UseGaldoranThemeAllTimes</c> config setting is enabled.</summary>
    internal bool UseGaldoranThemeAllTimes => this.IsLoaded && ModHelper
        .ReadContentPackConfig("FlashShifter.StardewValleyExpandedCP")
        ?.Value<bool?>("UseGaldoranThemeAllTimes") == true;
}
