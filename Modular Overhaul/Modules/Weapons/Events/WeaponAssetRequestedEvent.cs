namespace DaLion.Overhaul.Modules.Weapons.Events;

#region using directives

using System.Globalization;
using DaLion.Overhaul.Modules.Weapons.Integrations;
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
internal sealed class WeaponAssetRequestedEvent : AssetRequestedEvent
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

    /// <summary>Initializes a new instance of the <see cref="WeaponAssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal WeaponAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        this.Edit("Characters/Dialogue/Gil", new AssetEditor(EditGilDialogue, AssetEditPriority.Late));
        this.Edit("Data/ObjectInformation", new AssetEditor(EditObjectInformationData, AssetEditPriority.Late));
        this.Edit("Data/Events/AdventureGuild", new AssetEditor(EditSveEventsData, AssetEditPriority.Late));
        this.Edit("Data/Events/Blacksmith", new AssetEditor(EditBlacksmithEventsData, AssetEditPriority.Late));
        this.Edit("Data/Events/WizardHouse", new AssetEditor(EditWizardEventsData, AssetEditPriority.Late));
        this.Edit("Data/mail", new AssetEditor(EditMailData, AssetEditPriority.Late));
        this.Edit("Data/Monsters", new AssetEditor(EditMonstersData, AssetEditPriority.Late));
        this.Edit("Data/Quests", new AssetEditor(EditQuestsData, AssetEditPriority.Late));
        this.Edit("Data/weapons", new AssetEditor(EditWeaponsData, AssetEditPriority.Late));
        this.Edit("Strings/Locations", new AssetEditor(EditLocationsStrings));
        this.Edit("TileSheets/Projectiles", new AssetEditor(EditProjectilesTileSheet));
        this.Edit("TileSheets/weapons", new AssetEditor(EditWeaponsTileSheetEarly, AssetEditPriority.Early));
        this.Edit("TileSheets/weapons", new AssetEditor(EditWeaponsTileSheetLate, AssetEditPriority.Late));

        //this.Provide(
        //    $"{Manifest.UniqueID}/BeamCollisionAnimation",
        //    new ModTextureProvider(() => "assets/sprites/beam.png", AssetLoadPriority.Medium));
        //this.Provide(
        //    $"{Manifest.UniqueID}/InfinityCollisionAnimation",
        //    new ModTextureProvider(() => "assets/sprites/infinity.png", AssetLoadPriority.Medium));
        this.Provide("Data/Events/Blacksmith", new DictionaryProvider<string, string>(null, AssetLoadPriority.Low));
    }

    #region editor callbacks

    /// <summary>Edits events data with custom Dwarvish Blueprint introduction event.</summary>
    private static void EditBlacksmithEventsData(IAssetData asset)
    {
        if (!Context.IsWorldReady || !WeaponsModule.Config.DwarvenLegacy ||
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
        if (!WeaponsModule.Config.InfinityPlusOne)
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
        data["viegoCurse"] = I18n.Mail_Curse_Intro();
    }

    /// <summary>Edits monsters data for ancient weapon crafting materials.</summary>
    private static void EditMonstersData(IAssetData asset)
    {
        if (!WeaponsModule.Config.DwarvenLegacy)
        {
            return;
        }

        var data = asset.AsDictionary<string, string>().Data;
        var fields = data["Lava Lurk"].Split('/');
        var drops = fields[6].Split(' ');
        drops[^1] = ".05";
        fields[6] = string.Join(' ', drops);
        data["Lava Lurk"] = string.Join('/', fields);
        if (!Globals.DwarvenScrapIndex.HasValue)
        {
            return;
        }

        fields = data["Dwarvish Sentry"].Split('/');
        drops = fields[6].Split(' ');
        drops = drops.AddRangeToArray(new[] { Globals.DwarvenScrapIndex.Value.ToString(), ".05" });
        fields[6] = string.Join(' ', drops);
        data["Dwarvish Sentry"] = string.Join('/', fields);
    }

    /// <summary>Edits galaxy soul description.</summary>
    private static void EditObjectInformationData(IAssetData asset)
    {
        if (!WeaponsModule.Config.InfinityPlusOne)
        {
            return;
        }

        var data = asset.AsDictionary<int, string>().Data;

        // edit galaxy soul description
        var fields = data[ItemIDs.GalaxySoul].Split('/');
        fields[5] = I18n.Objects_Galaxysoul_Desc();
        data[ItemIDs.GalaxySoul] = string.Join('/', fields);
    }

    /// <summary>Adds the infinity enchantment projectile.</summary>
    private static void EditProjectilesTileSheet(IAssetData asset)
    {
        if (!WeaponsModule.Config.InfinityPlusOne)
        {
            return;
        }

        var editor = asset.AsImage();
        var sourceArea = new Rectangle(0, 0, 16, 16);
        var targetArea = new Rectangle(112, 16, 16, 16);
        editor.PatchImage(
            ModHelper.ModContent.Load<Texture2D>("assets/sprites/projectiles"),
            sourceArea,
            targetArea);
    }

    /// <summary>Edits quests data with custom Dwarvish Blueprint introduction quest.</summary>
    private static void EditQuestsData(IAssetData asset)
    {
        if (!WeaponsModule.Config.DwarvenLegacy)
        {
            return;
        }

        var data = asset.AsDictionary<int, string>().Data;
        data[(int)Quest.ForgeIntro] = I18n.Quests_Forge_Intro();
        data[(int)Quest.ForgeNext] = I18n.Quests_Forge_Next();
        data[(int)Quest.CurseIntro] = I18n.Quests_Hero_Curse();
        data[(int)Quest.HeroReward] = I18n.Quests_Hero_Reward();
    }

    /// <summary>Edits Marlon's Galaxy Sword event in SVE, removing references to purchasable Galaxy weapons.</summary>
    private static void EditSveEventsData(IAssetData asset)
    {
        var data = asset.AsDictionary<string, string>().Data;
        if (data.ContainsKey("1337098") && WeaponsModule.Config.InfinityPlusOne)
        {
            data["1337098"] = I18n.Events_1337098_Nopurchase();
        }
    }

    /// <summary>Edits weapons data with rebalanced stats.</summary>
    private static void EditWeaponsData(IAssetData asset)
    {
        if (!WeaponsModule.Config.EnableRebalance &&
            !WeaponsModule.Config.EnableStabbySwords && !WeaponsModule.Config.DwarvenLegacy &&
            !WeaponsModule.Config.InfinityPlusOne)
        {
            return;
        }

        var data = asset.AsDictionary<int, string>().Data;
        var keys = data.Keys;
        foreach (var key in keys)
        {
            var fields = data[key].Split('/');

            if (WeaponsModule.Config.EnableRebalance)
            {
                EditSingleWeapon(key, fields);
            }

            if (WeaponsModule.Config.DwarvenLegacy)
            {
                if (fields[Name].Contains("Dwarf"))
                {
                    fields[Name] = fields[Name].Replace("Dwarf", "Dwarven");
                }
                else if (key is ItemIDs.ElfBlade or ItemIDs.ForestSword)
                {
                    fields[Name] = fields[Name].Replace(key == ItemIDs.ElfBlade ? "Elf" : "Forest", "Elven");
                }
            }

            if (WeaponsModule.Config.InfinityPlusOne)
            {
                switch (key)
                {
                    case ItemIDs.DarkSword:
                        fields[Name] = I18n.Weapons_Darksword_Name();
                        fields[Description] = I18n.Weapons_Darksword_Desc();
                        break;
                    case ItemIDs.HolyBlade:
                        fields[Name] = I18n.Weapons_Holyblade_Name();
                        fields[Description] = I18n.Weapons_Holyblade_Desc();
                        break;
                }
            }

            data[key] = string.Join('/', fields);
        }
    }

    /// <summary>Edits weapons tilesheet with touched up textures.</summary>
    private static void EditWeaponsTileSheetEarly(IAssetData asset)
    {
        if (!WeaponsModule.Config.EnableRetexture)
        {
            return;
        }

        var editor = asset.AsImage();
        editor.PatchImage(ModHelper.ModContent.Load<Texture2D>("assets/sprites/weapons"));
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
        if (WeaponsModule.Config.InfinityPlusOne)
        {
            sourceArea = new Rectangle(0, 0, 32, 16);
            targetArea = new Rectangle(32, 0, 32, 16);
            editor.PatchImage(sourceTx, sourceArea, targetArea);
        }

        if (WeaponsModule.Config.DwarvenLegacy)
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
        if (!WeaponsModule.Config.InfinityPlusOne)
        {
            return;
        }

        var data = asset.AsDictionary<string, string>().Data;
        data["144703/n viegoCurse/p Wizard"] = StardewValleyExpandedIntegration.Instance?.IsLoaded == true
                ? I18n.Events_Curse_Intro_Sve()
                : I18n.Events_Curse_Intro();
    }

    #endregion editor callbacks

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
            case ItemIDs.WoodenBlade:
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
            case ItemIDs.SteelSmallsword:
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
            case ItemIDs.Cutlass:
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
            case ItemIDs.Rapier:
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
            case ItemIDs.SteelFalchion:
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
            case ItemIDs.SilverSaber:
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
            case ItemIDs.IronEdge:
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
            case ItemIDs.Claymore:
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
            case ItemIDs.TemperedBroadsword:
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
            case ItemIDs.TemplarsBlade:
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

            case ItemIDs.ForestSword:
                if (WeaponsModule.Config.DwarvenLegacy)
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
            case ItemIDs.BoneSword:
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
            case ItemIDs.OssifiedBlade:
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
            case ItemIDs.PirateSword:
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
            case ItemIDs.ObsidianEdge:
                fields[Description] = I18n.Weapons_Obsidianedge_Desc();
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
            case ItemIDs.LavaKatana:
                fields[Description] += I18n.Weapons_Lavakatana_Extradesc();
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
            case ItemIDs.NeptuneGlaive:
                fields[Description] = I18n.Weapons_Neptuneglaive_Desc();
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
            case ItemIDs.YetiTooth:
                fields[Description] += I18n.Weapons_Yetitooth_Extradesc();
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
            case ItemIDs.DarkSword:
                fields[Name] = I18n.Weapons_Darksword_Name();
                fields[Description] = I18n.Weapons_Darksword_Desc();
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
            case ItemIDs.HolyBlade:
                fields[Name] = I18n.Weapons_Holyblade_Name();
                fields[Description] = I18n.Weapons_Holyblade_Desc();
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
            case ItemIDs.GalaxySword:
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

            case ItemIDs.DwarfSword:
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
            case ItemIDs.DragonTooth:
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
            case ItemIDs.InfinityBlade:
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
            case ItemIDs.CarvingKnife:
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
            case ItemIDs.BurglarsShank:
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
            case ItemIDs.WindSpire:
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
            case ItemIDs.IronDirk:
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
            case ItemIDs.WickedKris:
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

            case ItemIDs.CrystalDagger:
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
            case ItemIDs.ShadowDagger:
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
            case ItemIDs.ElfBlade:
                if (WeaponsModule.Config.DwarvenLegacy)
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
            case ItemIDs.BrokenTrident:
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
            case ItemIDs.InsectHead:
                fields[Description] += I18n.Weapons_Insecthead_Extradesc();
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
            case ItemIDs.GalaxyDagger:
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
            case ItemIDs.DwarfDagger:
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
            case ItemIDs.DragontoothShiv:
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
            case ItemIDs.InfinityDagger:
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
            case ItemIDs.IridiumNeedle:
                fields[Description] += I18n.Weapons_Iridiumneedle_Extradesc();
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
            case ItemIDs.WoodClub:
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
            case ItemIDs.WoodMallet:
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
            case ItemIDs.LeadRod:
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
            case ItemIDs.Kudgel:
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
            case ItemIDs.TheSlammer:
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

            case ItemIDs.Femur:
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
            case ItemIDs.GalaxyHammer:
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

            case ItemIDs.DwarfHammer:
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
            case ItemIDs.DragontoothClub:
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
            case ItemIDs.InfinityGavel:
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

            case ItemIDs.Scythe:
                fields[MinDamage] = 1.ToString();
                fields[MaxDamage] = 1.ToString();
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                break;
            case ItemIDs.GoldenScythe:
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
