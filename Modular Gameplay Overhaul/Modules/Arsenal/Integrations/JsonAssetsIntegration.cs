namespace DaLion.Overhaul.Modules.Arsenal.Integrations;

#region using directives

using System.IO;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.JsonAssets;

#endregion using directives

[RequiresMod("spacechase0.JsonAssets", "Json Assets", "1.10.7")]
internal sealed class JsonAssetsIntegration : ModIntegration<JsonAssetsIntegration, IJsonAssetsApi>
{
    private JsonAssetsIntegration()
        : base("spacechase0.JsonAssets", "Json Assets", "1.10.7", ModHelper.ModRegistry)
    {
    }

    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        if (this.IsLoaded)
        {
            this.ModApi.LoadAssets(Path.Combine(ModHelper.DirectoryPath, "assets", "json-assets", "Arsenal"), I18n);
            this.ModApi.IdsAssigned += this.OnIdsAssigned;
            return true;
        }

        ArsenalModule.Config.DwarvishCrafting = false;
        ArsenalModule.Config.InfinityPlusOne = false;
        ModHelper.WriteConfig(ModEntry.Config);
        return false;
    }

    /// <summary>Gets assigned IDs.</summary>
    private void OnIdsAssigned(object? sender, EventArgs e)
    {
        this.AssertLoaded();
        Globals.HeroSoulIndex = this.ModApi.GetObjectId("Hero Soul");
        Globals.DwarvenScrapIndex = this.ModApi.GetObjectId("Dwarven Scrap");
        Globals.ElderwoodIndex = this.ModApi.GetObjectId("Elderwood");
        Globals.DwarvishBlueprintIndex = this.ModApi.GetObjectId("Dwarvish Blueprint");
        Log.T("The IDs for custom items in the Arsenal module have been assigned.");

        // reload the monsters data so that Dwarven Scrap Metal is added to Dwarven Sentinel's drop list
        ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Monsters");
    }
}
