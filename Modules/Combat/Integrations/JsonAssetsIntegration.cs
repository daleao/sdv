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
        : base(ModHelper.ModRegistry)
    {
    }

    /// <summary>Gets <see cref="Item"/> index of the Garnet gemstone.</summary>
    internal static int? GarnetIndex { get; private set; }

    /// <summary>Gets <see cref="Item"/> index of the Garnet Ring.</summary>
    internal static int? GarnetRingIndex { get; private set; }

    /// <summary>Gets <see cref="Item"/> index of the Infinity Band.</summary>
    internal static int? InfinityBandIndex { get; private set; }

    /// <summary>Gets <see cref="Item"/> index of the Hero Soul.</summary>
    internal static int? HeroSoulIndex { get; private set; }

    /// <summary>Gets <see cref="Item"/> index of Dwarven Scrap.</summary>
    internal static int? DwarvenScrapIndex { get; private set; }

    /// <summary>Gets <see cref="Item"/> index of Elderwood.</summary>
    internal static int? ElderwoodIndex { get; private set; }

    /// <summary>Gets <see cref="Item"/> index of Dwarvish weapon blueprints.</summary>
    internal static int? DwarvishBlueprintIndex { get; private set; }

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

        Log.D("[CMBT]: Registered the Json Assets integration.");
        return true;
    }

    /// <summary>Gets assigned IDs.</summary>
    private void AssignBaseIds(object? sender, EventArgs e)
    {
        this.AssertLoaded();
        var index = this.ModApi.GetObjectId("Hero Soul");
        if (index == -1)
        {
            Log.W("[CMBT]: Failed to get ID for Hero Soul from Json Assets.");
        }
        else
        {
            HeroSoulIndex = index;
            Log.D($"[CMBT]: Json Assets ID {HeroSoulIndex} has been assigned to Hero Soul.");
        }

        index = this.ModApi.GetObjectId("Dwarven Scrap");
        if (index == -1)
        {
            Log.W("[CMBT]: Failed to get ID for Dwarven Scrap from Json Assets.");
        }
        else
        {
            DwarvenScrapIndex = index;
            Log.D($"[CMBT]: Json Assets ID {DwarvenScrapIndex} has been assigned to Dwarven Scrap.");
        }

        index = this.ModApi.GetObjectId("Elderwood");
        if (index == -1)
        {
            Log.W("[CMBT]: Failed to get ID for Elderwood from Json Assets.");
        }
        else
        {
            ElderwoodIndex = index;
            Log.D($"[CMBT]: Json Assets ID {ElderwoodIndex} has been assigned to Elderwood.");
        }

        index = this.ModApi.GetObjectId("Dwarvish Blueprint");
        if (index == -1)
        {
            Log.W("[CMBT]: Failed to get ID for Dwarvish Blueprint from Json Assets.");
        }
        else
        {
            DwarvishBlueprintIndex = index;
            Log.D($"[CMBT]: Json Assets ID {DwarvishBlueprintIndex} has been assigned to Dwarvish Blueprint.");
        }

        index = this.ModApi.GetObjectId("Garnet");
        if (index == -1)
        {
            Log.W("[CMBT]: Failed to get ID for Garnet from Json Assets.");
        }
        else
        {
            GarnetIndex = index;
            Log.D($"[CMBT]: Json Assets ID {GarnetIndex} has been assigned to Garnet.");
        }

        // reload the monsters data so that Dwarven Scrap Metal is added to Dwarven Sentinel's drop list
        ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Monsters");
    }

    /// <summary>Gets assigned IDs.</summary>
    private void AssignRingIds(object? sender, EventArgs e)
    {
        this.AssertLoaded();
        var index = this.ModApi.GetObjectId("Garnet Ring");
        if (index == -1)
        {
            Log.W("[CMBT]: Failed to get ID for Garnet Ring from Json Assets.");
        }
        else
        {
            GarnetRingIndex = index;
            Log.D($"[CMBT]: Json Assets ID {GarnetRingIndex} has been assigned to Garnet Ring.");
        }

        index = this.ModApi.GetObjectId("Infinity Band");
        if (index == -1)
        {
            Log.W("[CMBT]: Failed to get ID for Infinity Band from Json Assets.");
        }
        else
        {
            InfinityBandIndex = index;
            Log.D($"[CMBT]: Json Assets ID {InfinityBandIndex} has been assigned to Infinity Band.");
        }
    }
}
