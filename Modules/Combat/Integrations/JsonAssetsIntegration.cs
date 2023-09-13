namespace DaLion.Overhaul.Modules.Combat.Integrations;

#region using directives

using System.IO;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.JsonAssets;

#endregion using directives

[ModRequirement("spacechase0.JsonAssets", "Json Assets", "1.10.7")]
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
        if (!this.IsLoaded)
        {
            return false;
        }

        var directory = Path.Combine(ModHelper.DirectoryPath, "assets", "json-assets", "base");
        if (Directory.Exists(directory))
        {
            this.ModApi.LoadAssets(directory, _I18n);
            this.ModApi.IdsAssigned += this.AssignBaseIds;
        }
        else
        {
            Log.W("Json Assets are missing for base Combat module items. `Dwarven Legacy` and `Hero Quest` will be disabled.");
            CombatModule.Config.DwarvenLegacy = false;
            CombatModule.Config.EnableHeroQuest = false;
            ModHelper.WriteConfig(ModEntry.Config);
        }

        directory = Path.Combine(ModHelper.DirectoryPath, "assets", "json-assets", "rings");
        if (Directory.Exists(directory))
        {
            var subDir = VanillaTweaksIntegration.Instance?.RingsCategoryEnabled == true
                ? "VanillaTweaks"
                : BetterRingsIntegration.Instance?.IsLoaded == true
                    ? "BetterRings" : "Vanilla";
            this.ModApi.LoadAssets(Path.Combine(directory, subDir), _I18n);
            this.ModApi.IdsAssigned += this.AssignRingIds;
        }
        else
        {
            Log.W("JSON Assets are missing for Rings.");
            CombatModule.Config.EnableInfinityBand = false;
            ModHelper.WriteConfig(ModEntry.Config);
        }

        return true;
    }

    /// <summary>Gets assigned IDs.</summary>
    private void AssignBaseIds(object? sender, EventArgs e)
    {
        this.AssertLoaded();
        Globals.HeroSoulIndex = this.ModApi.GetObjectId("Hero Soul");
        if (Globals.HeroSoulIndex == -1)
        {
            Log.W("[CMBT]: Failed to get ID for Hero Soul from Json Assets.");
        }
        else
        {
            Log.D($"[CMBT]: Json Assets ID {Globals.HeroSoulIndex} has been assigned to Hero Soul.");
        }

        Globals.DwarvenScrapIndex = this.ModApi.GetObjectId("Dwarven Scrap");
        if (Globals.DwarvenScrapIndex == -1)
        {
            Log.W("[CMBT]: Failed to get ID for Dwarven Scrap from Json Assets.");
        }
        else
        {
            Log.D($"[CMBT]: Json Assets ID {Globals.DwarvenScrapIndex} has been assigned to Dwarven Scrap.");
        }

        Globals.ElderwoodIndex = this.ModApi.GetObjectId("Elderwood");
        if (Globals.ElderwoodIndex == -1)
        {
            Log.W("[CMBT]: Failed to get ID for Elderwood from Json Assets.");
        }
        else
        {
            Log.D($"[CMBT]: Json Assets ID {Globals.ElderwoodIndex} has been assigned to Elderwood.");
        }

        Globals.DwarvishBlueprintIndex = this.ModApi.GetObjectId("Dwarvish Blueprint");
        if (Globals.DwarvishBlueprintIndex == -1)
        {
            Log.W("[CMBT]: Failed to get ID for Dwarvish Blueprint from Json Assets.");
        }
        else
        {
            Log.D($"[CMBT]: Json Assets ID {Globals.DwarvishBlueprintIndex} has been assigned to Dwarvish Blueprint.");
        }

        Globals.GarnetIndex = this.ModApi.GetObjectId("Garnet");
        if (Globals.GarnetIndex == -1)
        {
            Log.W("[CMBT]: Failed to get ID for Garnet from Json Assets.");
        }
        else
        {
            Log.D($"[CMBT]: Json Assets ID {Globals.GarnetIndex} has been assigned to Garnet.");
        }

        // reload the monsters data so that Dwarven Scrap Metal is added to Dwarven Sentinel's drop list
        ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Monsters");
    }

    /// <summary>Gets assigned IDs.</summary>
    private void AssignRingIds(object? sender, EventArgs e)
    {
        this.AssertLoaded();
        Globals.GarnetRingIndex = this.ModApi.GetObjectId("Garnet Ring");
        if (Globals.GarnetRingIndex == -1)
        {
            Log.W("[CMBT]: Failed to get ID for Garnet Ring from Json Assets.");
        }
        else
        {
            Log.D($"[CMBT]: Json Assets ID {Globals.GarnetRingIndex} has been assigned to Garnet Ring.");
        }

        Globals.InfinityBandIndex = this.ModApi.GetObjectId("Infinity Band");
        if (Globals.InfinityBandIndex == -1)
        {
            Log.W("[CMBT]: Failed to get ID for Infinity Band from Json Assets.");
        }
        else
        {
            Log.D($"[CMBT]: Json Assets ID {Globals.InfinityBandIndex} has been assigned to Infinity Band.");
        }
    }
}
