namespace DaLion.Overhaul.Modules.Arsenal.Integrations;

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

        this.ModApi.LoadAssets(Path.Combine(ModHelper.DirectoryPath, "assets", "json-assets", "Arsenal"), I18n);
        this.ModApi.IdsAssigned += this.OnIdsAssigned;
    }

    /// <summary>Gets assigned IDs.</summary>
    private void OnIdsAssigned(object? sender, EventArgs e)
    {
        this.AssertLoaded();
        Globals.HeroSoulIndex = this.ModApi.GetObjectId("Hero Soul");
        Globals.DwarvenScrapIndex = this.ModApi.GetObjectId("Dwarven Scrap");
        Globals.ElderwoodIndex = this.ModApi.GetObjectId("Elderwood");
        Globals.DwarvishBlueprintIndex = this.ModApi.GetObjectId("Dwarvish Blueprint");
        ModHelper.GameContent.InvalidateCache("Data/Monsters");
    }
}
