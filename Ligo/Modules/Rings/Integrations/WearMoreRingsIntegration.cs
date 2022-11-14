namespace DaLion.Ligo.Modules.Rings.Integrations;

#region using directives

using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.WearMoreRings;

#endregion using directives

internal sealed class WearMoreRingsIntegration : BaseIntegration<IWearMoreRingsApi>
{
    /// <summary>Initializes a new instance of the <see cref="WearMoreRingsIntegration"/> class.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    internal WearMoreRingsIntegration(IModRegistry modRegistry)
        : base("WearMoreRings", "bcmpinc.WearMoreRings", "5.1", modRegistry)
    {
    }

    /// <summary>Caches the API.</summary>
    internal void Register()
    {
        this.AssertLoaded();
        Ligo.Integrations.WearMoreRingsApi = this.ModApi;
    }
}
