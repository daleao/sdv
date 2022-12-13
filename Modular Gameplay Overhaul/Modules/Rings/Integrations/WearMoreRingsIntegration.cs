namespace DaLion.Overhaul.Modules.Rings.Integrations;

#region using directives

using System.Diagnostics.CodeAnalysis;
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
        ModEntry.Integrations[this.ModName] = this;
    }

    /// <summary>Gets the <see cref="IWearMoreRingsApi"/>.</summary>
    internal static IWearMoreRingsApi? Api { get; private set; }

    /// <inheritdoc />
    [MemberNotNull(nameof(Api))]
    protected override void RegisterImpl()
    {
        this.AssertLoaded();
        Api = this.ModApi;
    }
}
