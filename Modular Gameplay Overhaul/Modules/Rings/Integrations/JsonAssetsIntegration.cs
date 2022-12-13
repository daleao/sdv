namespace DaLion.Overhaul.Modules.Rings.Integrations;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.IO;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.JsonAssets;

#endregion using directives

internal sealed class JsonAssetsIntegration : BaseIntegration<IJsonAssetsApi>
{
    /// <summary>Initializes a new instance of the <see cref="JsonAssetsIntegration"/> class.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    internal JsonAssetsIntegration(IModRegistry modRegistry)
        : base("JsonAssets", "spacechase0.JsonAssets", "1.10.7", modRegistry)
    {
        ModEntry.Integrations[this.ModName] = this;
    }

    /// <summary>Gets the <see cref="IJsonAssetsApi"/>.</summary>
    internal static IJsonAssetsApi? Api { get; private set; }

    /// <inheritdoc />
    [MemberNotNull(nameof(Api))]
    protected override void RegisterImpl()
    {
        this.AssertLoaded();
        Api = this.ModApi;

        var subFolder = VanillaTweaksIntegration.RingsCategoryEnabled
            ? "VanillaTweaks"
            : BetterRingsIntegration.IsLoaded
                ? "BetterRings" : "Vanilla";
        this.ModApi.LoadAssets(Path.Combine(ModHelper.DirectoryPath, "assets", "json-assets", "Rings", subFolder), I18n);
        this.ModApi.IdsAssigned += this.OnIdsAssigned;
    }

    /// <summary>Gets assigned IDs.</summary>
    private void OnIdsAssigned(object? sender, EventArgs e)
    {
        if (this.ModApi is null)
        {
            return;
        }

        Globals.GarnetIndex = this.ModApi.GetObjectId("Garnet");
        Globals.GarnetRingIndex = this.ModApi.GetObjectId("Garnet Ring");
        Globals.InfinityBandIndex = this.ModApi.GetObjectId("Infinity Band");
    }
}
