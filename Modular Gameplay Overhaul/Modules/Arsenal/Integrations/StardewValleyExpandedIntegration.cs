namespace DaLion.Overhaul.Modules.Arsenal.Integrations;

#region using directives

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

    /// <inheritdoc />
    protected override void RegisterImpl()
    {
        this.AssertLoaded();
        IsLoaded = true;
    }
}
