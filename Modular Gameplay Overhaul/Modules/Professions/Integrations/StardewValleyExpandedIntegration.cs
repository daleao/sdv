namespace DaLion.Overhaul.Modules.Professions.Integrations;

#region using directives

using System.Diagnostics.CodeAnalysis;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Integrations;

#endregion using directives

internal sealed class StardewValleyExpandedIntegration : BaseIntegration
{
    /// <summary>Initializes a new instance of the <see cref="StardewValleyExpandedIntegration"/> class.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    internal StardewValleyExpandedIntegration(IModRegistry modRegistry)
        : base("StardewValleyExpanded", "FlashShifter.StardewValleyExpandedCP", null, modRegistry)
    {
    }

    /// <inheritdoc cref="BaseIntegration.IsLoaded"/>
    internal static new bool IsLoaded { get; private set; }

    /// <summary>Gets the value of the <c>DisableGaldoranTheme</c> config setting.</summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:Property summary documentation should match accessors", Justification = "Doesn't make sense in this context.")]
    internal static bool DisabeGaldoranTheme => ModHelper
        .ReadContentPackConfig("FlashShifter.StardewValleyExpandedCP")
        ?.Value<bool?>("DisableGaldoranTheme") == true;

    /// <summary>Gets the value of the <c>UseGaldoranThemeAllTimes</c> config setting.</summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:Property summary documentation should match accessors", Justification = "Doesn't make sense in this context.")]
    internal static bool UseGaldoranThemeAllTimes => ModHelper
        .ReadContentPackConfig("FlashShifter.StardewValleyExpandedCP")
        ?.Value<bool?>("UseGaldoranThemeAllTimes") == true;

    /// <inheritdoc />
    protected override void RegisterImpl()
    {
        this.AssertLoaded();
        IsLoaded = true;
    }
}
