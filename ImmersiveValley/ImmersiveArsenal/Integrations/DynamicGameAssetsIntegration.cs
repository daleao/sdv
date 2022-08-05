namespace DaLion.Stardew.Arsenal.Integrations;

#region using directives

using Common.Integrations;
using Common.Integrations.DynamicGameAssets;

#endregion using directives

internal sealed class DynamicGameAssetsIntegration : BaseIntegration<IDynamicGameAssetsAPI>
{
    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    public DynamicGameAssetsIntegration(IModRegistry modRegistry)
        : base("DynamicGameAssets", "spacechase0.DynamicGameAssets", "1.4.3", modRegistry) { }

    /// <summary>Add the Hero Soul item.</summary>
    public void Register()
    {
        AssertLoaded();
        ModEntry.DynamicGameAssetsApi = ModApi;
        //ModApi.AddEmbeddedPack(ModEntry.Manifest, Path.Combine(ModEntry.ModHelper.DirectoryPath, "assets", "dga"));
    }
}