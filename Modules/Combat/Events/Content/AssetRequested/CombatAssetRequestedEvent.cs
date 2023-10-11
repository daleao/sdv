namespace DaLion.Overhaul.Modules.Combat.Events.Content.AssetRequested;

#region using directives

using System.Globalization;
using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Shared.Constants;
using DaLion.Shared.Content;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class CombatAssetRequestedEvent : AssetRequestedEvent
{
    #region weapon data fields

    private const int Name = 0;
    private const int Description = 1;
    private const int MinDamage = 2;
    private const int MaxDamage = 3;
    private const int Knockback = 4;
    private const int Speed = 5;
    private const int Precision = 6;
    private const int Defense = 7;
    private const int Type = 8;
    private const int BaseDropLevel = 9;
    private const int MinDropLevel = 10;
    private const int Aoe = 11;
    private const int CritChance = 12;
    private const int CritPower = 13;

    #endregion weapon data fields

    /// <summary>Initializes a new instance of the <see cref="CombatAssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal CombatAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        this.Edit("Characters/Dialogue/Gil", new AssetEditor(EditGilDialogue, AssetEditPriority.Late));
        this.Edit("Data/CraftingRecipes", new AssetEditor(EditCraftingRecipesData));
        this.Edit("Data/Events/AdventureGuild", new AssetEditor(EditSveEventsData, AssetEditPriority.Late));
        this.Edit("Data/Events/Blacksmith", new AssetEditor(EditBlacksmithEventsData, AssetEditPriority.Late));
        this.Edit("Data/Events/WizardHouse", new AssetEditor(EditWizardEventsData, AssetEditPriority.Late));
        this.Edit("Data/mail", new AssetEditor(EditMailData, AssetEditPriority.Late));
        this.Edit("Data/Monsters", new AssetEditor(EditMonstersData, AssetEditPriority.Late));
        this.Edit("Data/ObjectInformation", new AssetEditor(EditObjectInformationData, AssetEditPriority.Late));
        this.Edit("Data/Quests", new AssetEditor(EditQuestsData, AssetEditPriority.Late));
        this.Edit("Data/weapons", new AssetEditor(EditWeaponsData, AssetEditPriority.Late));
        this.Edit("Maps/springobjects", new AssetEditor(EditSpringObjectsMaps, AssetEditPriority.Late));
        this.Edit("Strings/Locations", new AssetEditor(EditLocationsStrings));
        this.Edit("Strings/StringsFromCSFiles", new AssetEditor(EditStringsFromCsFiles, AssetEditPriority.Late));
        this.Edit("TileSheets/BuffsIcons", new AssetEditor(EditBuffsIconsTileSheet));
        this.Edit("TileSheets/Projectiles", new AssetEditor(EditProjectilesTileSheet));
        this.Edit("TileSheets/weapons", new AssetEditor(EditWeaponsTileSheetEarly, AssetEditPriority.Early));
        this.Edit("TileSheets/weapons", new AssetEditor(EditWeaponsTileSheetLate, AssetEditPriority.Late));
        this.Edit("aedenthorn.CustomOreNodes/dict", new AssetEditor(EditCustomOreNodes));

        this.Provide("Data/Events/Blacksmith", new DictionaryProvider<string, string>(null, AssetLoadPriority.Low));
        this.Provide(
            $"{Manifest.UniqueID}/GarnetNode",
            new ModTextureProvider(() => "assets/sprites/garnet_node.png"));
        this.Provide(
            $"{Manifest.UniqueID}/DwarvishBlueprint",
            new ModTextureProvider(() => "assets/sprites/blueprint.png"));
        this.Provide(
            $"{Manifest.UniqueID}/BleedAnimation",
            new ModTextureProvider(() => "assets/sprites/bleed.png"));
        this.Provide(
            $"{Manifest.UniqueID}/SlowAnimation",
            new ModTextureProvider(() => "assets/sprites/slow.png"));
        this.Provide(
            $"{Manifest.UniqueID}/StunAnimation",
            new ModTextureProvider(() => "assets/sprites/stun.png"));
        this.Provide(
            $"{Manifest.UniqueID}/GemstoneSockets",
            new ModTextureProvider(ProvideGemSockets));
        this.Provide(
            $"{Manifest.UniqueID}/SnowballCollisionAnimation",
            new ModTextureProvider(() => "assets/sprites/snowball.png"));
        //this.Provide(
        //    $"{Manifest.UniqueID}/BeamCollisionAnimation",
        //    new ModTextureProvider(() => "assets/sprites/beam.png", AssetLoadPriority.Medium));
        //this.Provide(
        //    $"{Manifest.UniqueID}/InfinityCollisionAnimation",
        //    new ModTextureProvider(() => "assets/sprites/infinity.png", AssetLoadPriority.Medium));
        //this.Provide(
        //    $"{Manifest.UniqueID}/QuincyCollisionAnimation",
        //    new ModTextureProvider(() => "assets/sprites/quincy.png"));
    }

    #region editor callbacks

    /// <summary>Patches buffs icons with energized buff icon.</summary>
    private static void EditBuffsIconsTileSheet(IAssetData asset)
    {
        if (ProfessionsModule.ShouldEnable)
        {
            return;
        }

        var editor = asset.AsImage();
        editor.ExtendImage(192, 80);

        var sourceArea = new Rectangle(64, 16, 32, 16);
        var targetArea = new Rectangle(64, 64, 32, 16);
        editor.PatchImage(
            ModHelper.ModContent.Load<Texture2D>("assets/sprites/buffs"),
            sourceArea,
            targetArea);
    }

    /// <summary>Edits events data with custom Dwarvish Blueprint introduction event.</summary>
    private static void EditBlacksmithEventsData(IAssetData asset)
    {
        if (!Context.IsWorldReady || !CombatModule.Config.DwarvenLegacy ||
            string.IsNullOrEmpty(Game1.player.Read(DataKeys.BlueprintsFound)) || !Game1.player.canUnderstandDwarves)
        {
            return;
        }

        var data = asset.AsDictionary<string, string>().Data;
        data["144701/f Clint 1500/p Clint"] = I18n.Events_Forge_Intro();
    }

    /// <summary>Patches custom Gil dialogue.</summary>
    private static void EditGilDialogue(IAssetData asset)
    {
        var data = asset.AsDictionary<string, string>().Data;
        data[StardewValleyExpandedIntegration.Instance?.IsLoaded == true
                ? "Snoring"
                : "ComeBackLater"] = I18n.Dialogue_Gil_Virtues();
    }

    /// <summary>Edits location string data with custom legendary sword rhyme.</summary>
    private static void EditLocationsStrings(IAssetData asset)
    {
        if (!CombatModule.Config.EnableHeroQuest)
        {
            return;
        }

        var data = asset.AsDictionary<string, string>().Data;
        data["Town_DwarfGrave_Translated"] = I18n.Locations_Town_DwarfGrave_Translated();
        data["SeedShop_Yoba"] = I18n.Locations_SeedShop_Yoba();
    }

    /// <summary>Patches mail data with mail from the Ferngill Revenue Service.</summary>
    private static void EditMailData(IAssetData asset)
    {
        var data = asset.AsDictionary<string, string>().Data;
        data["viegoCurse"] = ModHelper.ModRegistry.IsLoaded("Nom0ri.RomRas")
            ? I18n.Mail_Curse_Intro_Witch()
            : I18n.Mail_Curse_Intro();
    }

    /// <summary>Edits monsters data for ancient weapon crafting materials.</summary>
    private static void EditMonstersData(IAssetData asset)
    {
        if (!CombatModule.Config.DwarvenLegacy)
        {
            return;
        }

        var data = asset.AsDictionary<string, string>().Data;
        var fields = data["Lava Lurk"].Split('/');
        var drops = fields[6].Split(' ');
        drops[^1] = ".05";
        fields[6] = string.Join(' ', drops);
        data["Lava Lurk"] = string.Join('/', fields);
        if (!JsonAssetsIntegration.DwarvenScrapIndex.HasValue)
        {
            return;
        }

        fields = data["Dwarvish Sentry"].Split('/');
        drops = fields[6].Split(' ');
        drops = drops.AddRangeToArray(new[] { JsonAssetsIntegration.DwarvenScrapIndex.Value.ToString(), ".05" });
        fields[6] = string.Join(' ', drops);
        data["Dwarvish Sentry"] = string.Join('/', fields);
    }

    /// <summary>Edits galaxy soul description.</summary>
    private static void EditObjectInformationData(IAssetData asset)
    {
        var data = asset.AsDictionary<int, string>().Data;
        string[] fields;

        if (CombatModule.Config.RebalancedRings)
        {
            fields = data[ObjectIds.TopazRing].Split('/');
            fields[5] = CombatModule.ShouldEnable && CombatModule.Config.NewResistanceFormula
                ? I18n.Rings_Topaz_Desc_Resist()
                : I18n.Rings_Topaz_Desc_Defense();
            data[ObjectIds.TopazRing] = string.Join('/', fields);

            fields = data[ObjectIds.JadeRing].Split('/');
            fields[5] = I18n.Rings_Jade_Desc();
            data[ObjectIds.JadeRing] = string.Join('/', fields);

            fields = data[ObjectIds.WarriorRing].Split('/');
            fields[5] = I18n.Rings_Warrior_Desc();
            data[ObjectIds.WarriorRing] = string.Join('/', fields);

            fields = data[ObjectIds.RingOfYoba].Split('/');
            fields[5] = I18n.Rings_Yoba_Desc();
            data[ObjectIds.RingOfYoba] = string.Join('/', fields);

            fields = data[ObjectIds.ImmunityRing].Split('/');
            fields[5] += I18n.Rings_Immunity_ExtraDesc();
            data[ObjectIds.ImmunityRing] = string.Join('/', fields);

            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en)
            {
                fields = data[ObjectIds.ThornsRing].Split('/');
                if (!ModHelper.ModRegistry.IsLoaded("Rafseazz.RidgesideVillage"))
                {
                    fields[0] = "Ring of Thorns";
                }

                data[ObjectIds.ThornsRing] = string.Join('/', fields);
            }
        }

        if (CombatModule.Config.EnableInfinityBand)
        {
            fields = data[ObjectIds.IridiumBand].Split('/');
            fields[5] = I18n.Rings_Iridium_Desc();
            data[ObjectIds.IridiumBand] = string.Join('/', fields);
        }

        if (CombatModule.Config.EnableHeroQuest)
        {
            // edit galaxy soul description
            fields = data[ObjectIds.GalaxySoul].Split('/');
            fields[5] = I18n.Objects_GalaxySoul_Desc();
            data[ObjectIds.GalaxySoul] = string.Join('/', fields);
        }
    }

    /// <summary>Adds the infinity enchantment projectile.</summary>
    private static void EditProjectilesTileSheet(IAssetData asset)
    {
        var editor = asset.AsImage();
        var sourceArea = new Rectangle(16, 0, 16, 16);
        var targetArea = new Rectangle(64, 16, 16, 16);
        editor.PatchImage(
            ModHelper.ModContent.Load<Texture2D>("assets/sprites/projectiles"),
            sourceArea,
            targetArea);

        if (!CombatModule.Config.EnableHeroQuest)
        {
            return;
        }

        editor = asset.AsImage();
        sourceArea = new Rectangle(0, 0, 16, 16);
        targetArea = new Rectangle(112, 16, 16, 16);
        editor.PatchImage(
            ModHelper.ModContent.Load<Texture2D>("assets/sprites/projectiles"),
            sourceArea,
            targetArea);
    }

    /// <summary>Edits quests data with custom Dwarvish Blueprint introduction quest.</summary>
    private static void EditQuestsData(IAssetData asset)
    {
        if (!CombatModule.Config.DwarvenLegacy)
        {
            return;
        }

        var data = asset.AsDictionary<int, string>().Data;
        data[(int)QuestId.ForgeIntro] = I18n.Quests_Forge_Intro();
        data[(int)QuestId.CurseIntro] = I18n.Quests_Hero_Curse();
        data[(int)QuestId.HeroReward] = I18n.Quests_Hero_Reward();
    }

    /// <summary>Edits Marlon's Galaxy Sword event in SVE, removing references to purchasable Galaxy weapons.</summary>
    private static void EditSveEventsData(IAssetData asset)
    {
        var data = asset.AsDictionary<string, string>().Data;
        if (data.ContainsKey("1337098") && CombatModule.Config.EnableHeroQuest)
        {
            data["1337098"] = I18n.Events_1337098_NoPurchase();
        }
    }

    /// <summary>Edits weapons data with rebalanced stats.</summary>
    private static void EditWeaponsData(IAssetData asset)
    {
        if (CombatModule.Config.EnableWeaponOverhaul || CombatModule.Config.DwarvenLegacy ||
            CombatModule.Config.EnableHeroQuest)
        {
            var data = asset.AsDictionary<int, string>().Data;
            var keys = data.Keys;
            foreach (var key in keys)
            {
                var fields = data[key].Split('/');

                if (CombatModule.Config.EnableWeaponOverhaul)
                {
                    EditSingleWeapon(key, fields);
                }

                if (CombatModule.Config.DwarvenLegacy)
                {
                    if (fields[Name].Contains("Dwarf"))
                    {
                        fields[Name] = fields[Name].Replace("Dwarf", "Dwarven");
                    }
                    else if (key is WeaponIds.ElfBlade or WeaponIds.ForestSword)
                    {
                        fields[Name] = fields[Name].Replace(key == WeaponIds.ElfBlade ? "Elf" : "Forest", "Elven");
                    }
                }

                if (CombatModule.Config.EnableHeroQuest)
                {
                    switch (key)
                    {
                        case WeaponIds.DarkSword:
                            fields[Name] = I18n.Weapons_DarkSword_Name();
                            fields[Description] = I18n.Weapons_DarkSword_Desc();
                            break;
                        case WeaponIds.HolyBlade:
                            fields[Name] = I18n.Weapons_HolyBlade_Name();
                            fields[Description] = I18n.Weapons_HolyBlade_Desc();
                            break;
                    }
                }

                data[key] = string.Join('/', fields);
            }
        }

        if (CombatModule.Config.EnableInfinitySlingshot)
        {
            var data = asset.AsDictionary<int, string>().Data;
            data[WeaponIds.InfinitySlingshot] = string.Format(
                "Infinity Slingshot/{0}/1/3/1/308/0/0/4/-1/-1/0/.02/3/{1}",
                I18n.Slingshots_Infinity_Desc(),
                I18n.Slingshots_Infinity_Name());
        }
    }

    /// <summary>Edits weapons tilesheet with touched up textures.</summary>
    private static void EditWeaponsTileSheetEarly(IAssetData asset)
    {
        if (CombatModule.Config.EnableWeaponOverhaul)
        {
            var editor = asset.AsImage();
            editor.PatchImage(ModHelper.ModContent.Load<Texture2D>("assets/sprites/weapons"));
        }

        if (CombatModule.Config.EnableInfinitySlingshot)
        {
            var editor = asset.AsImage();
            var targetArea = new Rectangle(16, 128, 16, 16);
            editor.PatchImage(
                ModHelper.ModContent.Load<Texture2D>("assets/sprites/InfinitySlingshot"),
                targetArea: targetArea);
        }
    }

    /// <summary>Edits weapons tilesheet with touched up textures.</summary>
    private static void EditWeaponsTileSheetLate(IAssetData asset)
    {
        if (VanillaTweaksIntegration.Instance?.WeaponsCategoryEnabled != true &&
            SimpleWeaponsIntegration.Instance?.IsLoaded != true)
        {
            return;
        }

        var editor = asset.AsImage();
        var sourceTx = VanillaTweaksIntegration.Instance?.WeaponsCategoryEnabled == true
            ? ModHelper.ModContent.Load<Texture2D>("assets/sprites/weapons_vanillatweaks.png")
            : SimpleWeaponsIntegration.Instance?.IsLoaded == true
                ? ModHelper.ModContent.Load<Texture2D>("assets/sprites/weapons_simple.png")
                : Tool.weaponsTexture;
        Rectangle sourceArea, targetArea;
        if (CombatModule.Config.EnableHeroQuest)
        {
            sourceArea = new Rectangle(0, 0, 32, 16);
            targetArea = new Rectangle(32, 0, 32, 16);
            editor.PatchImage(sourceTx, sourceArea, targetArea);
        }

        if (CombatModule.Config.DwarvenLegacy)
        {
            sourceArea = new Rectangle(32, 0, 16, 16);
            targetArea = new Rectangle(112, 16, 16, 16);
            editor.PatchImage(sourceTx, sourceArea, targetArea);

            sourceArea = new Rectangle(48, 0, 16, 16);
            targetArea = new Rectangle(64, 32, 16, 16);
            editor.PatchImage(sourceTx, sourceArea, targetArea);
        }
    }

    /// <summary>Edits events data with custom Blade of Ruin introduction event.</summary>
    private static void EditWizardEventsData(IAssetData asset)
    {
        if (!CombatModule.Config.EnableHeroQuest)
        {
            return;
        }

        var data = asset.AsDictionary<string, string>().Data;
        data["144703/n viegoCurse/p Wizard"] = StardewValleyExpandedIntegration.Instance?.IsLoaded == true
                ? I18n.Events_Curse_Intro_Sve()
                : I18n.Events_Curse_Intro();
    }

    /// <summary>Adjust Jinxed debuff description.</summary>
    private static void EditStringsFromCsFiles(IAssetData asset)
    {
        if (!CombatModule.Config.NewResistanceFormula)
        {
            return;
        }

        var data = asset.AsDictionary<string, string>().Data;
        data["Buff.cs.465"] = I18n.Ui_Buffs_Jinxed();
    }

    /// <summary>Edits crafting recipes with new ring recipes.</summary>
    private static void EditCraftingRecipesData(IAssetData asset)
    {
        var data = asset.AsDictionary<string, string>().Data;

        string[] fields;
        if (CombatModule.Config.RebalancedRings)
        {
            data["Ring of Yoba"] = "336 5 335 5 72 1 768 20/Home/524/false/Combat 8";
        }

        if (CombatModule.Config.CraftableGemstoneRings)
        {
            data["Emerald Ring"] = "60 1 336 5/Home/533/Ring/Combat 6";
            data["Aquamarine Ring"] = "62 1 335 5/Home/531/Ring/Combat 4";
            data["Ruby Ring"] = "64 1 336 5/Home/534/Ring/Combat 6";
            data["Amethyst Ring"] = "66 1 334 5/Home/529/Ring/Combat 2";
            data["Topaz Ring"] = "68 1 334 5/Home/530/Ring/Combat 2";
            data["Jade Ring"] = "70 1 335 5/Home/532/Ring/Combat 4";
        }

        if (CombatModule.Config.EnableInfinityBand)
        {
            fields = data["Iridium Band"].Split('/');
            fields[0] = "337 5 768 100 769 100";
            data["Iridium Band"] = string.Join('/', fields);
        }
    }

    /// <summary>Edits spring objects with new and custom rings.</summary>
    private static void EditSpringObjectsMaps(IAssetData asset)
    {
        var editor = asset.AsImage();
        Rectangle sourceArea, targetArea;

        var sourceY = VanillaTweaksIntegration.Instance?.RingsCategoryEnabled == true
            ? 32 : BetterRingsIntegration.Instance?.IsLoaded == true
                ? 16 : 0;
        if (CombatModule.Config.CraftableGemstoneRings)
        {
            sourceArea = new Rectangle(16, sourceY, 96, 16);
            targetArea = new Rectangle(16, 352, 96, 16);
            editor.PatchImage(Textures.RingsTx, sourceArea, targetArea);
        }

        if (CombatModule.Config.EnableInfinityBand)
        {
            sourceArea = new Rectangle(0, sourceY, 16, 16);
            targetArea = new Rectangle(368, 336, 16, 16);
            editor.PatchImage(Textures.RingsTx, sourceArea, targetArea);
        }
    }

    /// <summary>Adds Garnet Node to Custom Ore Nodes dictionary.</summary>
    private static void EditCustomOreNodes(IAssetData asset)
    {
        if (!JsonAssetsIntegration.GarnetIndex.HasValue)
        {
            Log.W("[CMBT]: Failed to add Garnet Ore Node because the Garnet gemstone was not loaded.");
            return;
        }

        var data = asset.AsDictionary<string, object>().Data;
        data["DaLion.Overhaul/GarnetNode"] = new
        {
            parentSheetIndex = 0,
            dropItems = new[]
                {
                    new
                    {
                        itemIdOrName = JsonAssetsIntegration.GarnetIndex.Value.ToString(),
                        dropChance = 100.0,
                        minAmount = 1,
                        maxAmount = 2,
                        luckyAmount = 1,
                        minerAmount = 1,
                    },
                    new
                    {
                        itemIdOrName = ObjectIds.Stone.ToString(),
                        dropChance = 75.0,
                        minAmount = 2,
                        maxAmount = 5,
                        luckyAmount = 1,
                        minerAmount = 1,
                    },
                },
            oreLevelRanges = new[]
                {
                    new
                    {
                        minLevel = 80,
                        maxLevel = int.MaxValue,
                        spawnChanceMult = 0.5,
                        expMult = 1.0,
                        dropChanceMult = 1.0,
                        dropMult = 1.0,
                    },
                },
            nodeDesc = "A stone rich with Garnet.",
            spritePath = $"{Manifest.UniqueID}/GarnetNode",
            spriteType = "game",
            spriteX = 0,
            spriteY = 0,
            spriteW = 16,
            spriteH = 16,
            spawnChance = 1,
            durability = 8,
            exp = 20,
        };
    }

    #endregion editor callbacks

    #region provider callbacks

    /// <summary>Provides the correct gemstone socket texture path.</summary>
    private static string ProvideGemSockets()
    {
        var path = "assets/sprites/GemSocket_" + CombatModule.Config.SocketStyle;
        if (ModHelper.ModRegistry.IsLoaded("ManaKirel.VMI") ||
            ModHelper.ModRegistry.IsLoaded("ManaKirel.VintageInterface2"))
        {
            path += "_Vintage";
        }

        return path + ".png";
    }

    #endregion provider callbacks

    #region helpers

    private static void EditSingleWeapon(int key, string[] fields)
    {
        switch (key)
        {
            #region swords

            case 0: // rusty sword (removed)
                fields[MinDamage] = 2.ToString();
                fields[MaxDamage] = 5.ToString();
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-1).ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 0.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                break;

            // BASIC SWORDS
            case WeaponIds.WoodenBlade:
                fields[MinDamage] = 2.ToString();
                fields[MaxDamage] = 5.ToString();
                fields[Knockback] = 0.4.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 1.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 0.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.SteelSmallsword:
                fields[MinDamage] = 8.ToString();
                fields[MaxDamage] = 12.ToString();
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 1.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = 10.ToString();
                fields[MinDropLevel] = 1.ToString();
                fields[Aoe] = 0.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.Cutlass:
                fields[MinDamage] = 20.ToString();
                fields[MaxDamage] = 26.ToString();
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = 25.ToString();
                fields[MinDropLevel] = 10.ToString();
                fields[Aoe] = 4.ToString();
                fields[CritChance] = 0.06.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.Rapier:
                fields[MinDamage] = 30.ToString();
                fields[MaxDamage] = 40.ToString();
                fields[Knockback] = 0.35.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 2.ToString();
                fields[Precision] = 2.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = 50.ToString();
                fields[MinDropLevel] = 25.ToString();
                fields[Aoe] = (-8).ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.SteelFalchion:
                fields[MinDamage] = 46.ToString();
                fields[MaxDamage] = 58.ToString();
                fields[Knockback] = 0.45.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 1.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = 80.ToString();
                fields[MinDropLevel] = 50.ToString();
                fields[Aoe] = 0.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.4.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.SilverSaber:
                fields[MinDamage] = 7.ToString();
                fields[MaxDamage] = 13.ToString();
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 1.ToString();
                fields[BaseDropLevel] = 10.ToString();
                fields[MinDropLevel] = 1.ToString();
                fields[Aoe] = 0.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.IronEdge:
                fields[MinDamage] = 18.ToString();
                fields[MaxDamage] = 28.ToString();
                fields[Knockback] = 0.55.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-1).ToString();
                fields[Defense] = 2.ToString();
                fields[BaseDropLevel] = 25.ToString();
                fields[MinDropLevel] = 10.ToString();
                fields[Aoe] = 8.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.Claymore:
                fields[MinDamage] = 28.ToString();
                fields[MaxDamage] = 44.ToString();
                fields[Knockback] = 0.95.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-3).ToString();
                fields[Defense] = 3.ToString();
                fields[BaseDropLevel] = 50.ToString();
                fields[MinDropLevel] = 25.ToString();
                fields[Aoe] = 12.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.5.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.TemperedBroadsword:
                fields[MinDamage] = 36.ToString();
                fields[MaxDamage] = 54.ToString();
                fields[Knockback] = 0.65.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-2).ToString();
                fields[Defense] = 3.ToString();
                fields[BaseDropLevel] = 80.ToString();
                fields[MinDropLevel] = 50.ToString();
                fields[Aoe] = 8.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.TemplarsBlade:
                fields[MinDamage] = 60.ToString();
                fields[MaxDamage] = 80.ToString();
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Defense] = 2.ToString();
                fields[Precision] = 0.ToString();
                fields[BaseDropLevel] = 150.ToString();
                fields[MinDropLevel] = 120.ToString();
                fields[Aoe] = 4.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.5.ToString(CultureInfo.InvariantCulture);
                break;

            case WeaponIds.ForestSword:
                if (CombatModule.Config.DwarvenLegacy)
                {
                    fields[MinDamage] = 85.ToString();
                    fields[MaxDamage] = 100.ToString();
                    fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 3.ToString();
                    fields[Precision] = 2.ToString();
                    fields[Defense] = 1.ToString();
                    fields[BaseDropLevel] = (-1).ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[Aoe] = 12.ToString();
                    fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    fields[MinDamage] = 48.ToString();
                    fields[MaxDamage] = 60.ToString();
                    fields[Knockback] = 0.6.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 1.ToString();
                    fields[Precision] = 1.ToString();
                    fields[Defense] = 0.ToString();
                    fields[BaseDropLevel] = (-1).ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[Aoe] = 8.ToString();
                    fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                }

                break;
            case WeaponIds.BoneSword:
                fields[MinDamage] = 34.ToString();
                fields[MaxDamage] = 46.ToString();
                fields[Knockback] = 0.45.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 2.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 0.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.8.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.OssifiedBlade:
                fields[MinDamage] = 64.ToString();
                fields[MaxDamage] = 85.ToString();
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-1).ToString();
                fields[Defense] = 1.ToString();
                fields[Precision] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 4.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.5.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.PiratesSword:
                fields[MinDamage] = 36.ToString();
                fields[MaxDamage] = 48.ToString();
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 0.ToString();
                fields[CritChance] = 0.075.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                break;

            // UNIQUE SWORDS
            case WeaponIds.ObsidianEdge:
                fields[Description] = I18n.Weapons_ObsidianEdge_Desc();
                fields[MinDamage] = 70.ToString();
                fields[MaxDamage] = 95.ToString();
                fields[Knockback] = 0.7.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-2).ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 4.ToString();
                fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.5.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.LavaKatana:
                fields[Description] += I18n.Weapons_LavaKatana_ExtraDesc();
                fields[MinDamage] = 95.ToString();
                fields[MaxDamage] = 110.ToString();
                fields[Knockback] = 0.4.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 1.ToString();
                fields[Precision] = 1.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 4.ToString();
                fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 3.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.NeptuneGlaive:
                fields[Description] = I18n.Weapons_NeptuneGlaive_Desc();
                fields[MinDamage] = 90.ToString();
                fields[MaxDamage] = 120.ToString();
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-1).ToString();
                fields[Defense] = 2.ToString();
                fields[Precision] = 1.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 8.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.YetiTooth:
                fields[Description] += I18n.Weapons_YetiTooth_ExtraDesc();
                fields[MinDamage] = 33.ToString();
                fields[MaxDamage] = 44.ToString();
                fields[Knockback] = 0.6.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-1).ToString();
                fields[Defense] = 1.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 8.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.25.ToString(CultureInfo.InvariantCulture);
                break;

            // BIS SWORDS
            case WeaponIds.DarkSword:
                fields[Name] = I18n.Weapons_DarkSword_Name();
                fields[Description] = I18n.Weapons_DarkSword_Desc();
                fields[MinDamage] = 100.ToString();
                fields[MaxDamage] = 140.ToString();
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[Aoe] = 8.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.HolyBlade:
                fields[Name] = I18n.Weapons_HolyBlade_Name();
                fields[Description] = I18n.Weapons_HolyBlade_Desc();
                fields[MinDamage] = 120.ToString();
                fields[MaxDamage] = 160.ToString();
                fields[Knockback] = 0.55.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 2.ToString();
                fields[Aoe] = 8.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.GalaxySword:
                fields[MinDamage] = 80.ToString();
                fields[MaxDamage] = 120.ToString();
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 1.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 4.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.1.ToString(CultureInfo.InvariantCulture);
                break;

            case WeaponIds.DwarfSword:
                fields[MinDamage] = 130.ToString();
                fields[MaxDamage] = 175.ToString();
                fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-2).ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 4.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 16.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.DragontoothCutlass:
                fields[MinDamage] = 160.ToString();
                fields[MaxDamage] = 200.ToString();
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 1.ToString();
                fields[Precision] = 1.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 0.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 3.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.InfinityBlade:
                fields[MinDamage] = 140.ToString();
                fields[MaxDamage] = 180.ToString();
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 2.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 12.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.2.ToString(CultureInfo.InvariantCulture);
                break;

            #endregion swords

            #region daggers

            // BASIC DAGGERS
            case WeaponIds.CarvingKnife:
                fields[MinDamage] = 4.ToString();
                fields[MaxDamage] = 6.ToString();
                fields[Knockback] = 0.2.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = 5.ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 0.ToString();
                fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.BurglarsShank:
                fields[MinDamage] = 13.ToString();
                fields[MaxDamage] = 16.ToString();
                fields[Knockback] = 0.25.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = 25.ToString();
                fields[MinDropLevel] = 10.ToString();
                fields[Aoe] = (-4).ToString();
                fields[CritChance] = 0.15.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.4.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.WindSpire:
                fields[MinDamage] = 22.ToString();
                fields[MaxDamage] = 26.ToString();
                fields[Knockback] = 0.35.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 1.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = 50.ToString();
                fields[MinDropLevel] = 25.ToString();
                fields[Aoe] = 4.ToString();
                fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.IronDirk:
                fields[MinDamage] = 30.ToString();
                fields[MaxDamage] = 36.ToString();
                fields[Knockback] = 0.25.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = 80.ToString();
                fields[MinDropLevel] = 50.ToString();
                fields[Aoe] = 0.ToString();
                fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.875.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.WickedKris:
                fields[MinDamage] = 44.ToString();
                fields[MaxDamage] = 52.ToString();
                fields[Knockback] = 0.25.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = (-1).ToString();
                fields[BaseDropLevel] = 150.ToString();
                fields[MinDropLevel] = 120.ToString();
                fields[Aoe] = 8.ToString();
                fields[CritChance] = 0.15.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                break;

            case WeaponIds.CrystalDagger:
                fields[MinDamage] = 28.ToString();
                fields[MaxDamage] = 32.ToString();
                fields[Knockback] = 0.25.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 1.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 4.ToString();
                fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.ShadowDagger:
                fields[MinDamage] = 54.ToString();
                fields[MaxDamage] = 62.ToString();
                fields[Knockback] = 0.25.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 1.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = (-1).ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 8.ToString();
                fields[CritChance] = 0.11.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.ElfBlade:
                if (CombatModule.Config.DwarvenLegacy)
                {
                    fields[MinDamage] = 50.ToString();
                    fields[MaxDamage] = 60.ToString();
                    fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 3.ToString();
                    fields[Precision] = 2.ToString();
                    fields[Defense] = 1.ToString();
                    fields[BaseDropLevel] = (-1).ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[Aoe] = 8.ToString();
                    fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    fields[MinDamage] = 32.ToString();
                    fields[MaxDamage] = 38.ToString();
                    fields[Knockback] = 0.25.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 2.ToString();
                    fields[Precision] = 1.ToString();
                    fields[Defense] = 0.ToString();
                    fields[BaseDropLevel] = (-1).ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[Aoe] = 0.ToString();
                    fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                }

                break;
            case WeaponIds.BrokenTrident:
                fields[MinDamage] = 50.ToString();
                fields[MaxDamage] = 58.ToString();
                fields[Knockback] = 0.35.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 1.ToString();
                fields[Defense] = (-1).ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 12.ToString();
                fields[CritChance] = 0.13333333333333d.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                break;

            // UNIQUE DAGGERS
            case WeaponIds.InsectHead:
                fields[Description] += I18n.Weapons_InsectHead_ExtraDesc();
                fields[MinDamage] = 1.ToString();
                fields[MaxDamage] = 3.ToString();
                fields[Knockback] = 0.15.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = (-1).ToString();
                fields[Defense] = (-3).ToString();
                fields[Type] = 1.ToString(); // this gets overwritten for some reason
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = (-4).ToString();
                fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.ToString(CultureInfo.InvariantCulture);
                break;

            // BIS DAGGERS
            case WeaponIds.GalaxyDagger:
                fields[MinDamage] = 55.ToString();
                fields[MaxDamage] = 70.ToString();
                fields[Knockback] = 0.275.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 4.ToString();
                fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.65.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.DwarfDagger:
                fields[MinDamage] = 95.ToString();
                fields[MaxDamage] = 115.ToString();
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-1).ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 2.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 12.ToString();
                fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.DragontoothShiv:
                fields[MinDamage] = 125.ToString();
                fields[MaxDamage] = 140.ToString();
                fields[Knockback] = 0.25.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 1.ToString();
                fields[Precision] = 1.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 0.ToString();
                fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.5.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.InfinityDagger:
                fields[MinDamage] = 105.ToString();
                fields[MaxDamage] = 120.ToString();
                fields[Knockback] = 0.3.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 8.ToString();
                fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.8.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.IridiumNeedle:
                fields[Description] += I18n.Weapons_IridiumNeedle_ExtraDesc();
                fields[MinDamage] = 68.ToString();
                fields[MaxDamage] = 80.ToString();
                fields[Knockback] = 0.1.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 1.ToString();
                fields[Precision] = 2.ToString();
                fields[Defense] = (-2).ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = (-8).ToString();
                fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.ToString(CultureInfo.InvariantCulture);
                break;

            #endregion daggers

            #region clubs

            // BASIC CLUBS
            case WeaponIds.WoodClub:
                fields[MinDamage] = 5.ToString();
                fields[MaxDamage] = 16.ToString();
                fields[Knockback] = 1.0.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = 5.ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 0.ToString();
                fields[CritChance] = 0.025.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.WoodMallet:
                fields[MinDamage] = 13.ToString();
                fields[MaxDamage] = 40.ToString();
                fields[Knockback] = 1.1.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 1.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = 25.ToString();
                fields[MinDropLevel] = 10.ToString();
                fields[Aoe] = 4.ToString();
                fields[CritChance] = 0.025.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 3.0.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.LeadRod:
                fields[MinDamage] = 23.ToString();
                fields[MaxDamage] = 70.ToString();
                fields[Knockback] = 1.2.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-3).ToString();
                fields[Precision] = (-1).ToString();
                fields[Defense] = 1.ToString();
                fields[BaseDropLevel] = 50.ToString();
                fields[MinDropLevel] = 25.ToString();
                fields[Aoe] = 8.ToString();
                fields[CritChance] = 0.025.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 3.0.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.Kudgel:
                fields[MinDamage] = 30.ToString();
                fields[MaxDamage] = 90.ToString();
                fields[Knockback] = 1.25.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-1).ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = 80.ToString();
                fields[MinDropLevel] = 50.ToString();
                fields[Aoe] = 8.ToString();
                fields[CritChance] = 0.025.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 3.0.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.TheSlammer:
                fields[MinDamage] = 44.ToString();
                fields[MaxDamage] = 133.ToString();
                fields[Knockback] = 1.33333333333333d.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-2).ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 1.ToString();
                fields[BaseDropLevel] = 150.ToString();
                fields[MinDropLevel] = 120.ToString();
                fields[Aoe] = 8.ToString();
                fields[CritChance] = 0.025.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 3.5.ToString(CultureInfo.InvariantCulture);
                break;

            case WeaponIds.Femur:
                fields[MinDamage] = 25.ToString();
                fields[MaxDamage] = 76.ToString();
                fields[Knockback] = 1.1.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-1).ToString();
                fields[Precision] = (-1).ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 4.ToString();
                fields[CritChance] = 0.025.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 3.0.ToString(CultureInfo.InvariantCulture);
                break;

            // BIS CLUBS
            case WeaponIds.GalaxyHammer:
                fields[MinDamage] = 60.ToString();
                fields[MaxDamage] = 200.ToString();
                fields[Knockback] = 1.1.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 1.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 8.ToString();
                fields[CritChance] = 0.025.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 3.0.ToString(CultureInfo.InvariantCulture);
                break;

            case WeaponIds.DwarfHammer:
                fields[MinDamage] = 90.ToString();
                fields[MaxDamage] = 270.ToString();
                fields[Knockback] = 1.25.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-1).ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 3.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 20.ToString();
                fields[CritChance] = 0.025.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 3.0.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.DragontoothClub:
                fields[MinDamage] = 120.ToString();
                fields[MaxDamage] = 360.ToString();
                fields[Knockback] = 1.0.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 1.ToString();
                fields[Precision] = 1.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 4.ToString();
                fields[CritChance] = 0.025.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 4.0.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.InfinityGavel:
                fields[MinDamage] = 100.ToString();
                fields[MaxDamage] = 300.ToString();
                fields[Knockback] = 1.2.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 1.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 16.ToString();
                fields[CritChance] = 0.025.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 3.0.ToString(CultureInfo.InvariantCulture);
                break;

            #endregion clubs

            #region bachelor(ette) weapons

            case 40: // abby (dagger)
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 1.ToString();
                fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                break;
            case 42: // haley (sword)
                fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Aoe] = (-4).ToString();
                fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                break;
            case 39: // leah (dagger)
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                break;
            case 36: // maru (club)
                fields[Knockback] = 1.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[CritChance] = 0.025.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 3.ToString(CultureInfo.InvariantCulture);
                break;
            case 38: // penny (club)
                fields[Knockback] = 0.8.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[CritChance] = 0.0166666666666667d.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 3.0.ToString(CultureInfo.InvariantCulture);
                break;
            case 25: // alex (club)
                fields[Knockback] = 1.0.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 1.ToString();
                fields[Precision] = 1.ToString();
                fields[CritChance] = 0.025.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 3.ToString(CultureInfo.InvariantCulture);
                break;
            case 35: // eliott (dagger)
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Aoe] = (-8).ToString();
                fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.125.ToString(CultureInfo.InvariantCulture);
                break;
            case 37: // harvey (club)
                fields[Knockback] = 1.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 1.ToString();
                fields[CritChance] = 0.025.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 3.ToString(CultureInfo.InvariantCulture);
                break;
            case 30: // sam (club)
                fields[Knockback] = 1.0.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-2).ToString();
                fields[Precision] = (-1).ToString();
                fields[Aoe] = 8.ToString();
                fields[CritChance] = 0.025.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                break;
            case 41: // seb (club)
                fields[Knockback] = 1.1.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Aoe] = 4.ToString();
                fields[CritChance] = 0.025.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 3.0.ToString(CultureInfo.InvariantCulture);
                break;

            #endregion bachelor(ette) weapons

            #region scythes

            case WeaponIds.Scythe:
                fields[MinDamage] = 1.ToString();
                fields[MaxDamage] = 1.ToString();
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                break;
            case WeaponIds.GoldenScythe:
                fields[MinDamage] = 13.ToString();
                fields[MaxDamage] = 13.ToString();
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[CritChance] = 0.15.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                break;

                #endregion scythes
        }
    }

    #endregion helpers
}
