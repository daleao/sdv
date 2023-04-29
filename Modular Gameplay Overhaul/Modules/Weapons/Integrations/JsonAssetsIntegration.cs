namespace DaLion.Overhaul.Modules.Weapons.Integrations;

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
    /// <summary>Initializes a new instance of the <see cref="JsonAssetsIntegration"/> class.</summary>
    internal JsonAssetsIntegration()
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

        WeaponsModule.Config.DwarvenLegacy = false;
        WeaponsModule.Config.InfinityPlusOne = false;
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
        Log.T("[WPNZ]: The IDs for custom items in the Weapons module have been assigned.");

        // reload the monsters data so that Dwarven Scrap Metal is added to Dwarven Sentinel's drop list
        ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Monsters");
    }
}
