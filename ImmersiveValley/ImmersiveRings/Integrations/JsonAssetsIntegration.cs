namespace DaLion.Stardew.Rings.Integrations;

#region using directives

using System.IO;
using DaLion.Common.Integrations;
using DaLion.Common.Integrations.JsonAssets;

#endregion using directives

internal sealed class JsonAssetsIntegration : BaseIntegration<IJsonAssetsApi>
{
    /// <summary>Initializes a new instance of the <see cref="JsonAssetsIntegration"/> class.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    public JsonAssetsIntegration(IModRegistry modRegistry)
        : base("JsonAssets", "spacechase0.JsonAssets", "1.10.7", modRegistry)
    {
    }

    /// <summary>Register the Garnet and Garnet Ring items.</summary>
    public void Register()
    {
        this.AssertLoaded();
        ModEntry.JsonAssetsApi = this.ModApi;
        this.ModApi.LoadAssets(Path.Combine(ModEntry.ModHelper.DirectoryPath, "assets", "json-assets"), ModEntry.i18n);
        this.ModApi.IdsAssigned += this.OnIdsAssigned;
    }

    /// <summary>Get assigned IDs.</summary>
    private void OnIdsAssigned(object? sender, EventArgs e)
    {
        if (this.ModApi is null)
        {
            return;
        }

        ModEntry.GarnetIndex = this.ModApi.GetObjectId("Garnet");
        ModEntry.GarnetRingIndex = this.ModApi.GetObjectId("Garnet Ring");
        ModEntry.InfinityBandIndex = this.ModApi.GetObjectId("Infinity Band");
    }
}
