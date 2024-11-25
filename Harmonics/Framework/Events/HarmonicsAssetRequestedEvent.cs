namespace DaLion.Harmonics.Framework.Events;

#region using directives

using DaLion.Shared.Constants;
using DaLion.Shared.Content;
using DaLion.Shared.Enums;
using DaLion.Shared.Events;
using ItemExtensions.Models;
using ItemExtensions.Models.Contained;
using ItemExtensions.Models.Enums;
using ItemExtensions.Models.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley.GameData.Objects;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="HarmonicsAssetRequestedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class HarmonicsAssetRequestedEvent(EventManager? manager = null)
    : AssetRequestedEvent(manager ?? HarmonicsMod.EventManager)
{
    /// <inheritdoc />
    protected override void Initialize()
    {
        this.Edit("Data/CraftingRecipes", new AssetEditor(EditCraftingRecipesData));
        this.Edit("Data/Objects", new AssetEditor(EditObjectsData, AssetEditPriority.Late));
        this.Edit("Maps/springobjects", new AssetEditor(EditSpringObjectsSpritesheet, AssetEditPriority.Late));
        this.Edit("Mods/mistyspring.ItemExtensions/Resources", new AssetEditor(EditItemExtensionsData));

        this.Provide(
            $"{Manifest.UniqueID}/Garnet",
            new ModTextureProvider(ProvideGarnetTexture));
        this.Provide(
            $"{Manifest.UniqueID}/Rings",
            new ModTextureProvider(ProvideRingsTextures));
    }

    #region editor callback

    /// <summary>Edits crafting recipes with new ring recipes.</summary>
    private static void EditCraftingRecipesData(IAssetData asset)
    {
        var data = asset.AsDictionary<string, string>().Data;

        var fields = data["Iridium Band"].Split('/');
        fields[0] = "337 5 768 100 769 100";
        data["Iridium Band"] = string.Join('/', fields);

        if (!Config.CraftableGemstoneRings)
        {
            return;
        }

        data["Emerald Ring"] = "60 1 336 5/Home/533/Ring/s Combat 6";
        data["Aquamarine Ring"] = "62 1 335 5/Home/531/Ring/s Combat 4";
        data["Ruby Ring"] = "64 1 336 5/Home/534/Ring/s Combat 6";
        data["Amethyst Ring"] = "66 1 334 5/Home/529/Ring/s Combat 2";
        data["Topaz Ring"] = "68 1 334 5/Home/530/Ring/s Combat 2";
        data["Jade Ring"] = "70 1 335 5/Home/532/Ring/s Combat 4";
        data["Garnet Ring"] = $"{GarnetStoneId} 1 336 5/Home/{GarnetRingId}/Ring/s Combat 7";
    }

    /// <summary>Patches new Garnet gemstone object.</summary>
    private static void EditObjectsData(IAssetData asset)
    {
        var data = asset.AsDictionary<string, ObjectData>().Data;
        data[GarnetStoneId] = new ObjectData
        {
            Name = "Garnet Gemstone",
            DisplayName = I18n.Objects_Garnet_Name(),
            Description = I18n.Objects_Garnet_Desc(),
            Type = "Minerals",
            Category = (int)ObjectCategory.Gems,
            Price = 300,
            Texture = $"{UniqueId}/Garnet",
            SpriteIndex = 0,
            Edibility = -300,
            ContextTags =
            [
                "color_red",
            ],
        };

        var ringSpriteIndex = 6;
        if (RingTextureStyle == TextureStyle.BetterRings)
        {
            ringSpriteIndex += 9;
        }
        else if (RingTextureStyle == TextureStyle.VanillaTweaks)
        {
            ringSpriteIndex += 18;
        }

        data[GarnetRingId] = new ObjectData
        {
            Name = "Garnet Ring",
            DisplayName = I18n.Rings_Garnet_Name(),
            Description = I18n.Rings_Garnet_Desc(),
            Type = "Ring",
            Category = (int)ObjectCategory.None,
            Price = 800,
            Texture = $"{UniqueId}/Rings",
            SpriteIndex = ringSpriteIndex,
            Edibility = -300,
            ContextTags =
            [
                "color_red",
                "ring_item",
            ],
        };

        ringSpriteIndex = 8;
        if (RingTextureStyle == TextureStyle.BetterRings)
        {
            ringSpriteIndex += 9;
        }
        else if (RingTextureStyle == TextureStyle.VanillaTweaks)
        {
            ringSpriteIndex += 18;
        }

        data[InfinityBandId] = new ObjectData
        {
            Name = "Infinity Band",
            DisplayName = I18n.Rings_Infinity_Name(),
            Description = I18n.Rings_Infinity_Desc(),
            Type = "Ring",
            Category = (int)ObjectCategory.None,
            Price = 800,
            Texture = $"{UniqueId}/Rings",
            SpriteIndex = ringSpriteIndex,
            Edibility = -300,
            ContextTags =
            [
                "color_red",
                "ring_item",
            ],
        };

        data[QualifiedObjectIds.IridiumBand.Split(")")[1]].Description = I18n.Rings_Iridium_Ring_Desc();
    }

    private static void EditSpringObjectsSpritesheet(IAssetData asset)
    {
        var editor = asset.AsImage();
        var targetArea = new Rectangle(16, 352, 96, 16);
        var sourceArea = new Rectangle(0, 0, 96, 16);
        editor.PatchImage(ModHelper.ModContent.Load<Texture2D>("assets/rings"), sourceArea, targetArea);

        targetArea = new Rectangle(368, 336, 16, 16);
        sourceArea = new Rectangle(112, 0, 16, 16);
        editor.PatchImage(ModHelper.ModContent.Load<Texture2D>("assets/rings"), sourceArea, targetArea);
    }

    private static void EditItemExtensionsData(IAssetData asset)
    {
        var data = asset.AsDictionary<string, ResourceData>().Data;
        data[$"{UniqueId}/GarnetNode"] = new ResourceData
        {
            Name = "UniqueId}/GarnetNode",
            DisplayName = I18n.Node_Garnet_Name(),
            Description = I18n.Node_Garnet_Desc(),
            Texture = $"{UniqueId}/Garnet",
            SpriteIndex = 1,
            Width = 1,
            Height = 1,
            Health = 3,
            ItemDropped = QualifiedObjectIds.Stone,
            MinDrops = 1,
            MaxDrops = 2,
            ExtraItems =
            [
                new ExtraSpawn
                {
                    ItemId = $"(O){GarnetStoneId}", Chance = 1, MinStack = 1, MaxStack = 2,
                },
                new ExtraSpawn
                {
                    ItemId = $"(O){GarnetStoneId}",
                    Chance = 0.5,
                    Condition =
                        $"PLAYER_HAS_PROFESSION Target {(ModHelper.ModRegistry.IsLoaded("DaLion.Professions") ? Farmer.gemologist : Farmer.geologist)}",
                    MinStack = 1,
                    MaxStack = 2,
                },
            ],
            Debris = "stone",
            FailSounds = ["clubhit", "clank"],
            BreakingSound = "stoneCrack",
            Sound = "hammer",
            Tool = "Pickaxe",
            Exp = 18,
            Skill = "mining",
            ContextTags = null,
            MineSpawns =
            [
                new MineSpawn
                {
                    Floors = "80/77376", SpawnFrequency = 0.009, AdditionalChancePerLevel = 0.00005, Type = MineType.All,
                },
            ],
        };
    }

    #endregion editor callbacks

    #region provider callbacks

    /// <summary>Provides garnet gemstone texture.</summary>
    private static string ProvideGarnetTexture()
    {
        return "assets/garnet.png";
    }

    /// <summary>Provides new ring textures.</summary>
    private static string ProvideRingsTextures()
    {
        return "assets/rings.png";
    }

    #endregion provider callbacks
}
