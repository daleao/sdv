namespace DaLion.Ligo.Modules.Arsenal.Integrations;

#region using directives

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
    }

    /// <summary>Gets a value indicating whether the integration has been registered.</summary>
    internal static bool Registered { get; private set; }

    /// <summary>Registers the Garnet and Garnet Ring items.</summary>
    internal void Register()
    {
        this.AssertLoaded();
        Ligo.Integrations.JsonAssetsApi = this.ModApi;
        this.ModApi.LoadAssets(Path.Combine(ModHelper.DirectoryPath, "assets", "json-assets", "Arsenal"), i18n);
        this.ModApi.IdsAssigned += this.OnIdsAssigned;
        Registered = true;
    }

    /// <summary>Gets assigned IDs.</summary>
    private void OnIdsAssigned(object? sender, EventArgs e)
    {
        if (this.ModApi is null)
        {
            return;
        }

        Globals.HeroSoulIndex = this.ModApi.GetObjectId("Hero Soul");
        Globals.DwarvenScrapIndex = this.ModApi.GetObjectId("Dwarven Scrap");
        Globals.ElderwoodIndex = this.ModApi.GetObjectId("Elderwood");
        Globals.DwarvishBlueprintIndex = this.ModApi.GetObjectId("Dwarvish Blueprint");
        ModHelper.GameContent.InvalidateCache("Data/Monsters");
    }
}
