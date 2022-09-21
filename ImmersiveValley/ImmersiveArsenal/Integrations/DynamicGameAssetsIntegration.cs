namespace DaLion.Stardew.Arsenal.Integrations;

#region using directives

using DaLion.Common.Integrations;
using DaLion.Common.Integrations.DynamicGameAssets;

#endregion using directives

internal sealed class DynamicGameAssetsIntegration : BaseIntegration<IDynamicGameAssetsApi>
{
    /// <summary>Initializes a new instance of the <see cref="DynamicGameAssetsIntegration"/> class.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    public DynamicGameAssetsIntegration(IModRegistry modRegistry)
        : base("DynamicGameAssets", "spacechase0.DynamicGameAssets", "1.4.3", modRegistry)
    {
    }

    /// <summary>Add the Hero Soul item.</summary>
    public void Register()
    {
        this.AssertLoaded();
        ModEntry.DynamicGameAssetsApi = this.ModApi;
        //ModApi.AddEmbeddedPack(ModEntry.Manifest, Path.Combine(ModEntry.ModHelper.DirectoryPath, "assets", "dga"));
    }
}
