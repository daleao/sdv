namespace DaLion.Overhaul.Modules.Tools.Integrations;

#region using directives

using DaLion.Shared.Integrations;

#endregion using directives

internal sealed class MoonMisadventuresIntegration : BaseIntegration
{
    /// <summary>Initializes a new instance of the <see cref="MoonMisadventuresIntegration"/> class.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    internal MoonMisadventuresIntegration(IModRegistry modRegistry)
        : base("MoonMisadventures", "spacechase0.MoonMisadventures", null, modRegistry)
    {
        ModEntry.Integrations[this.ModName] = this;
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
