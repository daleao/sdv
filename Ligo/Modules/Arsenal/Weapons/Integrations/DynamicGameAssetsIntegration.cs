namespace DaLion.Ligo.Modules.Arsenal.Weapons.Integrations;

#region using directives

using System.IO;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.DynamicGameAssets;

#endregion using directives

internal sealed class DynamicGameAssetsIntegration : BaseIntegration<IDynamicGameAssetsApi>
{
    /// <summary>Initializes a new instance of the <see cref="DynamicGameAssetsIntegration"/> class.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    internal DynamicGameAssetsIntegration(IModRegistry modRegistry)
        : base("DynamicGameAssets", "spacechase0.DynamicGameAssets", "1.4.3", modRegistry)
    {
    }

    /// <summary>Add the Hero Soul item.</summary>
    internal void Register()
    {
        this.AssertLoaded();
        Ligo.Integrations.DynamicGameAssetsApi = this.ModApi;
        this.ModApi.AddEmbeddedPack(
            ModEntry.Manifest,
            Path.Combine(ModEntry.ModHelper.DirectoryPath, "assets", "dga"));
    }
}
