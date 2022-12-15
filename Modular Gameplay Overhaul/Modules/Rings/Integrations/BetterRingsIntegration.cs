namespace DaLion.Overhaul.Modules.Rings.Integrations;

#region using directives

using DaLion.Shared.Integrations;

#endregion using directives

internal sealed class BetterRingsIntegration : BaseIntegration
{
    /// <summary>Initializes a new instance of the <see cref="BetterRingsIntegration"/> class.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    internal BetterRingsIntegration(IModRegistry modRegistry)
        : base("BetterRings", "BBR.BetterRings", null, modRegistry)
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
        ModHelper.GameContent.InvalidateCache("Maps/springobjects");
    }
}
