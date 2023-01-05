namespace DaLion.Overhaul.Modules.Arsenal.Events;

#region using directives

using System.Globalization;
using DaLion.Overhaul.Modules.Arsenal.Integrations;
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
internal sealed class ArsenalAssetRequestedEvent : AssetRequestedEvent
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

    /// <summary>Initializes a new instance of the <see cref="ArsenalAssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ArsenalAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        this.Edit("Characters/Dialogue/Gil", new AssetEditor(EditGilDialogue, AssetEditPriority.Late));
        this.Edit("Data/ObjectInformation", new AssetEditor(EditObjectInformationData, AssetEditPriority.Default));
        this.Edit("Data/Events/AdventureGuild", new AssetEditor(EditSveEventsData, AssetEditPriority.Late));
        this.Edit("Data/Events/Blacksmith", new AssetEditor(EditBlacksmithEventsData, AssetEditPriority.Default));
        this.Edit("Data/Events/WizardHouse", new AssetEditor(EditWizardEventsData, AssetEditPriority.Default));
        this.Edit("Data/mail", new AssetEditor(EditMailData, AssetEditPriority.Default));
        this.Edit("Data/Monsters", new AssetEditor(EditMonstersData, AssetEditPriority.Late));
        this.Edit("Data/Quests", new AssetEditor(EditQuestsData, AssetEditPriority.Default));
        this.Edit("Data/weapons", new AssetEditor(EditWeaponsData, AssetEditPriority.Late));
        this.Edit("Strings/Locations", new AssetEditor(EditLocationsStrings, AssetEditPriority.Default));
        this.Edit("TileSheets/BuffsIcons", new AssetEditor(EditBuffsIconsTileSheet, AssetEditPriority.Default));
        this.Edit("TileSheets/Projectiles", new AssetEditor(EditProjectilesTileSheet, AssetEditPriority.Default));
        this.Edit("TileSheets/weapons", new AssetEditor(EditWeaponsTileSheetEarly, AssetEditPriority.Early));
        this.Edit("TileSheets/weapons", new AssetEditor(EditWeaponsTileSheetLate, AssetEditPriority.Late));

        this.Provide(
            $"{Manifest.UniqueID}/BeamCollisionAnimation",
            new ModTextureProvider(() => "assets/animations/beam.png", Priority: AssetLoadPriority.Medium));
        this.Provide(
            $"{Manifest.UniqueID}/InfinityCollisionAnimation",
            new ModTextureProvider(() => "assets/animations/infinity.png", AssetLoadPriority.Medium));
        this.Provide(
            $"{Manifest.UniqueID}/QuincyCollisionAnimation",
            new ModTextureProvider(() => "assets/animations/quincy.png", Priority: AssetLoadPriority.Medium));
        this.Provide(
            $"{Manifest.UniqueID}/SnowballCollisionAnimation",
            new ModTextureProvider(() => "assets/animations/snowball.png", Priority: AssetLoadPriority.Medium));
        this.Provide("Data/Events/Blacksmith", new DictionaryProvider<string, string>(null, AssetLoadPriority.Low));
    }

    #region editor callbacks

    /// <summary>Edits events data with custom Dwarvish Blueprint introduction event.</summary>
    private static void EditBlacksmithEventsData(IAssetData asset)
    {
        if (!Context.IsWorldReady || !ArsenalModule.Config.DwarvishCrafting ||
            string.IsNullOrEmpty(Game1.player.Read(DataFields.BlueprintsFound)) || !Game1.player.canUnderstandDwarves)
        {
            return;
        }

        var data = asset.AsDictionary<string, string>().Data;
        data["144701/f Clint 1500/p Clint"] = I18n.Get("events.forge.intro");
    }

    /// <summary>Patches buffs icons with energized buff icon.</summary>
    private static void EditBuffsIconsTileSheet(IAssetData asset)
    {
        if (!ArsenalModule.Config.Weapons.EnableEnchants)
        {
            return;
        }

        var editor = asset.AsImage();
        editor.ExtendImage(192, 64);

        var sourceArea = new Rectangle(64, 16, 16, 16);
        var targetArea = new Rectangle(96, 48, 16, 16);
        editor.PatchImage(
            ModHelper.ModContent.Load<Texture2D>("assets/sprites/buffs"),
            sourceArea,
            targetArea);
    }

    /// <summary>Patches custom Gil dialogue.</summary>
    private static void EditGilDialogue(IAssetData asset)
    {
        var data = asset.AsDictionary<string, string>().Data;
        data[StardewValleyExpandedIntegration.Instance?.IsLoaded == true ? "Snoring" : "ComeBackLater"] =
            I18n.Get("dialogue.gil.virtues");
    }

    /// <summary>Edits location string data with custom legendary sword rhyme.</summary>
    private static void EditLocationsStrings(IAssetData asset)
    {
        if (!ArsenalModule.Config.InfinityPlusOne)
        {
            return;
        }

        var data = asset.AsDictionary<string, string>().Data;
        data["Town_DwarfGrave_Translated"] = I18n.Get("locations.Town.DwarfGrave.Translated");
        data["SeedShop_Yoba"] = I18n.Get("locations.SeedShop.Yoba");
    }

    /// <summary>Patches mail data with mail from the Ferngill Revenue Service.</summary>
    private static void EditMailData(IAssetData asset)
    {
        var data = asset.AsDictionary<string, string>().Data;
        data["viegoCurse"] = I18n.Get("mail.curse.intro");
    }

    /// <summary>Edits monsters data for ancient weapon crafting materials.</summary>
    private static void EditMonstersData(IAssetData asset)
    {
        if (!ArsenalModule.Config.DwarvishCrafting)
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
        if (!ArsenalModule.Config.InfinityPlusOne)
        {
            return;
        }

        var data = asset.AsDictionary<int, string>().Data;

        // edit galaxy soul description
        var fields = data[Constants.GalaxySoulIndex].Split('/');
        fields[5] = I18n.Get("objects.galaxysoul.desc");
        data[Constants.GalaxySoulIndex] = string.Join('/', fields);
    }

    /// <summary>Adds the infinity enchantment projectile.</summary>
    private static void EditProjectilesTileSheet(IAssetData asset)
    {
        if (!ArsenalModule.Config.InfinityPlusOne)
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
        if (!ArsenalModule.Config.DwarvishCrafting)
        {
            return;
        }

        var data = asset.AsDictionary<int, string>().Data;
        data[Constants.ForgeIntroQuestId] = I18n.Get("quests.forge.intro");
        data[Constants.ForgeNextQuestId] = I18n.Get("quests.forge.next");
        data[Constants.VirtuesIntroQuestId] = I18n.Get("quests.curse.intro");
        data[Constants.VirtuesNextQuestId] = I18n.Get("quests.curse.next");
        data[Constants.VirtuesLastQuestId] = I18n.Get("quests.curse.last");
        data[Virtue.Honor] = I18n.Get("quests.virtues.honor");
        data[Virtue.Compassion] = I18n.Get("quests.virtues.compassion");
        data[Virtue.Wisdom] = I18n.Get("quests.virtues.wisdom");
        data[Virtue.Generosity] = I18n.Get("quests.virtues.generosity");
        data[Virtue.Valor] = I18n.Get("quests.virtues.valor");
    }

    /// <summary>Edits Marlon's Galaxy Sword event in SVE, removing references to purchasable Galaxy weapons.</summary>
    private static void EditSveEventsData(IAssetData asset)
    {
        var data = asset.AsDictionary<string, string>().Data;
        if (data.ContainsKey("1337098") && ArsenalModule.Config.InfinityPlusOne)
        {
            data["1337098"] = I18n.Get("events.1337098.nopurchase");
        }
    }

    /// <summary>Edits weapons data with rebalanced stats.</summary>
    private static void EditWeaponsData(IAssetData asset)
    {
        if (!ArsenalModule.Config.Weapons.EnableRebalance &&
            !ArsenalModule.Config.Weapons.EnableStabbySwords && !ArsenalModule.Config.DwarvishCrafting &&
            !ArsenalModule.Config.InfinityPlusOne)
        {
            return;
        }

        var data = asset.AsDictionary<int, string>().Data;
        var keys = data.Keys;
        foreach (var key in keys)
        {
            var fields = data[key].Split('/');

            if (ArsenalModule.Config.Weapons.EnableRebalance)
            {
                EditSingleWeapon(key, fields);
            }

            if (ArsenalModule.Config.DwarvishCrafting)
            {
                if (fields[Name].Contains("Dwarf"))
                {
                    fields[Name] = fields[Name].Replace("Dwarf", "Dwarven");
                }
                else if (key is Constants.ElfBladeIndex or Constants.ForestSwordIndex)
                {
                    fields[Name] = fields[Name].Replace(key == Constants.ElfBladeIndex ? "Elf" : "Forest", "Elven");
                }
            }

            if (ArsenalModule.Config.InfinityPlusOne)
            {
                switch (key)
                {
                    case Constants.DarkSwordIndex:
                        fields[Name] = I18n.Get("weapons.darksword.name");
                        fields[Description] = I18n.Get("weapons.darksword.desc");
                        break;
                    case Constants.HolyBladeIndex:
                        fields[Name] = I18n.Get("weapons.holyblade.name");
                        fields[Description] = I18n.Get("weapons.holyblade.desc");
                        break;
                }
            }

            data[key] = string.Join('/', fields);
        }

        if (ArsenalModule.Config.InfinityPlusOne)
        {
            data[Constants.InfinitySlingshotIndex] = string.Format(
                "Infinity Slingshot/{0}/1/3/1/308/0/0/4/-1/-1/0/.02/3/{1}",
                I18n.Get("slingshots.infinity.desc"),
                I18n.Get("slingshots.infinity.name"));
        }
    }

    /// <summary>Edits weapons tilesheet with touched up textures.</summary>
    private static void EditWeaponsTileSheetEarly(IAssetData asset)
    {
        if (!ArsenalModule.Config.Weapons.EnableRetexture && !ArsenalModule.Config.InfinityPlusOne)
        {
            return;
        }

        var editor = asset.AsImage();
        if (ArsenalModule.Config.Weapons.EnableRetexture)
        {
            editor.PatchImage(ModHelper.ModContent.Load<Texture2D>("assets/sprites/weapons"));
        }
        else
        {
            var area = new Rectangle(16, 128, 16, 16);
            editor.PatchImage(ModHelper.ModContent.Load<Texture2D>("assets/sprites/weapons"), area, area);
        }
    }

    /// <summary>Edits weapons tilesheet with touched up textures.</summary>
    private static void EditWeaponsTileSheetLate(IAssetData asset)
    {
        if (VanillaTweaksIntegration.Instance?.WeaponsCategoryEnabled != true)
        {
            return;
        }

        var editor = asset.AsImage();
        var sourceTx = ModHelper.ModContent.Load<Texture2D>("assets/sprites/weapons_vanillatweaks.png");
        Rectangle sourceArea, targetArea;
        if (ArsenalModule.Config.InfinityPlusOne)
        {
            sourceArea = new Rectangle(0, 0, 32, 16);
            targetArea = new Rectangle(32, 0, 32, 16);
            editor.PatchImage(sourceTx, sourceArea, targetArea);
        }

        if (ArsenalModule.Config.DwarvishCrafting)
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
        if (!ArsenalModule.Config.InfinityPlusOne)
        {
            return;
        }

        var data = asset.AsDictionary<string, string>().Data;
        data["144703/n viegoCurse/p Wizard"] = StardewValleyExpandedIntegration.Instance?.IsLoaded == true
            ? I18n.Get("events.curse.intro.sve")
            : I18n.Get("events.curse.intro");
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
                fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
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
            case Constants.WoodenBladeIndex:
                fields[MinDamage] = 2.ToString();
                fields[MaxDamage] = 5.ToString();
                fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 1.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 0.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                break;
            case Constants.SteelSmallswordIndex:
                fields[MinDamage] = 8.ToString();
                fields[MaxDamage] = 12.ToString();
                fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 1.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = 10.ToString();
                fields[MinDropLevel] = 1.ToString();
                fields[Aoe] = 0.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                break;
            case Constants.CutlassIndex:
                fields[MinDamage] = 20.ToString();
                fields[MaxDamage] = 26.ToString();
                fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = 25.ToString();
                fields[MinDropLevel] = 10.ToString();
                fields[Aoe] = 4.ToString();
                fields[CritChance] = 0.06.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                break;
            case Constants.RapierIndex:
                fields[MinDamage] = 30.ToString();
                fields[MaxDamage] = 40.ToString();
                fields[Knockback] = 0.55.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 2.ToString();
                fields[Precision] = 2.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = 50.ToString();
                fields[MinDropLevel] = 25.ToString();
                fields[Aoe] = (-8).ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                break;
            case Constants.SteelFalchionIndex:
                fields[MinDamage] = 40.ToString();
                fields[MaxDamage] = 54.ToString();
                fields[Knockback] = 0.7.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 1.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = 80.ToString();
                fields[MinDropLevel] = 50.ToString();
                fields[Aoe] = 0.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.4.ToString(CultureInfo.InvariantCulture);
                break;
            case Constants.SilverSaberIndex:
                fields[MinDamage] = 7.ToString();
                fields[MaxDamage] = 13.ToString();
                fields[Knockback] = 0.7875.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 1.ToString();
                fields[BaseDropLevel] = 10.ToString();
                fields[MinDropLevel] = 1.ToString();
                fields[Aoe] = 0.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                break;
            case Constants.IronEdgeIndex:
                fields[MinDamage] = 18.ToString();
                fields[MaxDamage] = 28.ToString();
                fields[Knockback] = 0.935.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-1).ToString();
                fields[Defense] = 2.ToString();
                fields[BaseDropLevel] = 25.ToString();
                fields[MinDropLevel] = 10.ToString();
                fields[Aoe] = 8.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                break;
            case Constants.ClaymoreIndex:
                fields[MinDamage] = 28.ToString();
                fields[MaxDamage] = 42.ToString();
                fields[Knockback] = 1.125.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-3).ToString();
                fields[Defense] = 3.ToString();
                fields[BaseDropLevel] = 50.ToString();
                fields[MinDropLevel] = 25.ToString();
                fields[Aoe] = 12.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.5.ToString(CultureInfo.InvariantCulture);
                break;
            case Constants.TemperedBroadswordIndex:
                fields[MinDamage] = 36.ToString();
                fields[MaxDamage] = 58.ToString();
                fields[Knockback] = 1.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-2).ToString();
                fields[Defense] = 3.ToString();
                fields[BaseDropLevel] = 80.ToString();
                fields[MinDropLevel] = 50.ToString();
                fields[Aoe] = 8.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.ToString(CultureInfo.InvariantCulture);
                break;
            case Constants.TemplarsBladeIndex:
                fields[MinDamage] = 60.ToString();
                fields[MaxDamage] = 80.ToString();
                fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Defense] = 2.ToString();
                fields[Precision] = 0.ToString();
                fields[BaseDropLevel] = 150.ToString();
                fields[MinDropLevel] = 120.ToString();
                fields[Aoe] = 4.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.5.ToString(CultureInfo.InvariantCulture);
                break;

            case Constants.ForestSwordIndex:
                if (ArsenalModule.Config.DwarvishCrafting)
                {
                    fields[MinDamage] = 85.ToString();
                    fields[MaxDamage] = 100.ToString();
                    fields[Knockback] = 0.9375.ToString(CultureInfo.InvariantCulture);
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
                    fields[Knockback] = 0.825.ToString(CultureInfo.InvariantCulture);
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
            case Constants.BoneSwordIndex:
                fields[MinDamage] = 34.ToString();
                fields[MaxDamage] = 46.ToString();
                fields[Knockback] = 0.675.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 2.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 0.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.8.ToString(CultureInfo.InvariantCulture);
                break;
            case Constants.OssifiedBladeIndex:
                fields[MinDamage] = 64.ToString();
                fields[MaxDamage] = 85.ToString();
                fields[Knockback] = 0.7875.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-1).ToString();
                fields[Defense] = 1.ToString();
                fields[Precision] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 4.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.5.ToString(CultureInfo.InvariantCulture);
                break;
            case Constants.PiratesSwordIndex:
                fields[MinDamage] = 36.ToString();
                fields[MaxDamage] = 48.ToString();
                fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 0.ToString();
                fields[CritChance] = 0.075.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                break;
            case Constants.YetiToothIndex:
                fields[MinDamage] = 33.ToString();
                fields[MaxDamage] = 44.ToString();
                fields[Knockback] = 0.8625.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-1).ToString();
                fields[Defense] = 1.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 8.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                break;
            case Constants.ObsidianEdgeIndex:
                fields[Description] += I18n.Get("weapons.obsidianedge.extradesc");
                fields[MinDamage] = 70.ToString();
                fields[MaxDamage] = 95.ToString();
                fields[Knockback] = 0.8625.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-2).ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 4.ToString();
                fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.5.ToString(CultureInfo.InvariantCulture);
                break;
            case Constants.LavaKatanaIndex:
                fields[MinDamage] = 95.ToString();
                fields[MaxDamage] = 110.ToString();
                fields[Knockback] = 0.65.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 1.ToString();
                fields[Precision] = 1.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 4.ToString();
                fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 3.ToString(CultureInfo.InvariantCulture);
                break;

            // UNIQUE SWORDS
            case Constants.NeptunesGlaiveIndex:
                fields[MinDamage] = 90.ToString();
                fields[MaxDamage] = 120.ToString();
                fields[Knockback] = 0.825.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-1).ToString();
                fields[Defense] = 2.ToString();
                fields[Precision] = 1.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 8.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                break;

            // BIS SWORDS
            case Constants.DarkSwordIndex:
                fields[Name] = I18n.Get("weapons.darksword.name");
                fields[Description] = I18n.Get("weapons.darksword.desc");
                fields[MinDamage] = 100.ToString();
                fields[MaxDamage] = 140.ToString();
                fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[Aoe] = 8.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                break;
            case Constants.HolyBladeIndex:
                fields[Name] = I18n.Get("weapons.holyblade.name");
                fields[Description] = I18n.Get("weapons.holyblade.desc");
                fields[MinDamage] = 120.ToString();
                fields[MaxDamage] = 160.ToString();
                fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 2.ToString();
                fields[Aoe] = 8.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                break;
            case Constants.GalaxySwordIndex:
                fields[MinDamage] = 80.ToString();
                fields[MaxDamage] = 120.ToString();
                fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 4.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                break;

            case Constants.DwarfSwordIndex:
                fields[MinDamage] = 130.ToString();
                fields[MaxDamage] = 175.ToString();
                fields[Knockback] = 0.9.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-2).ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 3.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 12.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                break;
            case Constants.DragonToothIndex:
                fields[MinDamage] = 160.ToString();
                fields[MaxDamage] = 200.ToString();
                fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 1.ToString();
                fields[Precision] = 1.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 0.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 3.ToString(CultureInfo.InvariantCulture);
                break;
            case Constants.InfinityBladeIndex:
                fields[MinDamage] = 140.ToString();
                fields[MaxDamage] = 180.ToString();
                fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 12.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                break;

            #endregion swords

            #region daggers

            // BASIC DAGGERS
            case Constants.CarvingKnife:
                fields[MinDamage] = 4.ToString();
                fields[MaxDamage] = 6.ToString();
                fields[Knockback] = 0.4.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = 5.ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 0.ToString();
                fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                break;
            case Constants.BurglarsShankIndex:
                fields[MinDamage] = 13.ToString();
                fields[MaxDamage] = 16.ToString();
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = 25.ToString();
                fields[MinDropLevel] = 10.ToString();
                fields[Aoe] = (-4).ToString();
                fields[CritChance] = 0.12.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                break;
            case Constants.WindSpireIndex:
                fields[MinDamage] = 22.ToString();
                fields[MaxDamage] = 26.ToString();
                fields[Knockback] = 0.666666666666667d.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 1.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = 50.ToString();
                fields[MinDropLevel] = 25.ToString();
                fields[Aoe] = 4.ToString();
                fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                break;
            case Constants.IronDirkIndex:
                fields[MinDamage] = 30.ToString();
                fields[MaxDamage] = 36.ToString();
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = 80.ToString();
                fields[MinDropLevel] = 50.ToString();
                fields[Aoe] = 0.ToString();
                fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.875.ToString(CultureInfo.InvariantCulture);
                break;
            case Constants.WickedKrisIndex:
                fields[MinDamage] = 44.ToString();
                fields[MaxDamage] = 52.ToString();
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = (-1).ToString();
                fields[BaseDropLevel] = 150.ToString();
                fields[MinDropLevel] = 120.ToString();
                fields[Aoe] = 8.ToString();
                fields[CritChance] = 0.15.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                break;

            case Constants.CrystalDaggerIndex:
                fields[MinDamage] = 28.ToString();
                fields[MaxDamage] = 32.ToString();
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 1.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 4.ToString();
                fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                break;
            case Constants.ShadowDaggerIndex:
                fields[MinDamage] = 54.ToString();
                fields[MaxDamage] = 62.ToString();
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 1.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = (-1).ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 8.ToString();
                fields[CritChance] = 0.11.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                break;
            case Constants.ElfBladeIndex:
                if (ArsenalModule.Config.DwarvishCrafting)
                {
                    fields[MinDamage] = 50.ToString();
                    fields[MaxDamage] = 60.ToString();
                    fields[Knockback] = 0.625.ToString(CultureInfo.InvariantCulture);
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
                    fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
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
            case Constants.BrokenTridentIndex:
                fields[MinDamage] = 50.ToString();
                fields[MaxDamage] = 58.ToString();
                fields[Knockback] = 0.66666666666666d.ToString(CultureInfo.InvariantCulture);
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
            case Constants.InsectHeadIndex:
                fields[Description] += I18n.Get("weapons.insecthead.extradesc");
                fields[MinDamage] = 1.ToString();
                fields[MaxDamage] = 3.ToString();
                fields[Knockback] = 0.1.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = (-1).ToString();
                fields[Defense] = (-3).ToString();
                fields[Type] = 1.ToString(); // this gets overwritten for some reason
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = (-4).ToString();
                fields[CritChance] = 0.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.ToString(CultureInfo.InvariantCulture);
                break;

            // BIS DAGGERS
            case Constants.GalaxyDaggerIndex:
                fields[MinDamage] = 55.ToString();
                fields[MaxDamage] = 70.ToString();
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 4.ToString();
                fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                break;
            case Constants.IridiumNeedleIndex:
                fields[Description] += I18n.Get("weapons.iridiumneedle.extradesc");
                fields[MinDamage] = 68.ToString();
                fields[MaxDamage] = 80.ToString();
                fields[Knockback] = 0.25.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 1.ToString();
                fields[Precision] = 2.ToString();
                fields[Defense] = (-2).ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = (-8).ToString();
                fields[CritChance] = 1.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                break;

            case Constants.DwarfDaggerIndex:
                fields[MinDamage] = 95.ToString();
                fields[MaxDamage] = 115.ToString();
                fields[Knockback] = 0.6.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-1).ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 2.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 12.ToString();
                fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                break;
            case Constants.DragontoothShivIndex:
                fields[MinDamage] = 125.ToString();
                fields[MaxDamage] = 140.ToString();
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 1.ToString();
                fields[Precision] = 1.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 0.ToString();
                fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.5.ToString(CultureInfo.InvariantCulture);
                break;
            case Constants.InfinityDaggerIndex:
                fields[MinDamage] = 105.ToString();
                fields[MaxDamage] = 120.ToString();
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 12.ToString();
                fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                break;

            #endregion daggers

            #region clubs

            // BASIC CLUBS
            case Constants.WoodClubIndex:
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
            case Constants.WoodMalletIndex:
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
            case Constants.LeadRodIndex:
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
            case Constants.KudgelIndex:
                fields[MinDamage] = 30.ToString();
                fields[MaxDamage] = 90.ToString();
                fields[Knockback] = 1.33333333333333d.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-2).ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = 80.ToString();
                fields[MinDropLevel] = 50.ToString();
                fields[Aoe] = 8.ToString();
                fields[CritChance] = 0.025.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 3.0.ToString(CultureInfo.InvariantCulture);
                break;
            case Constants.TheSlammerIndex:
                fields[MinDamage] = 44.ToString();
                fields[MaxDamage] = 133.ToString();
                fields[Knockback] = 1.2.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-2).ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 1.ToString();
                fields[BaseDropLevel] = 150.ToString();
                fields[MinDropLevel] = 120.ToString();
                fields[Aoe] = 8.ToString();
                fields[CritChance] = 0.025.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 3.5.ToString(CultureInfo.InvariantCulture);
                break;

            case Constants.FemurIndex:
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
            case Constants.GalaxyHammerIndex:
                fields[MinDamage] = 60.ToString();
                fields[MaxDamage] = 200.ToString();
                fields[Knockback] = 1.0.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 8.ToString();
                fields[CritChance] = 0.025.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 3.0.ToString(CultureInfo.InvariantCulture);
                break;

            case Constants.DwarfHammerIndex:
                fields[MinDamage] = 90.ToString();
                fields[MaxDamage] = 270.ToString();
                fields[Knockback] = 1.2.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-2).ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 2.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 20.ToString();
                fields[CritChance] = 0.025.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 3.0.ToString(CultureInfo.InvariantCulture);
                break;
            case Constants.DragontoothClubIndex:
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
            case Constants.InfinityGavelIndex:
                fields[MinDamage] = 100.ToString();
                fields[MaxDamage] = 300.ToString();
                fields[Knockback] = 1.0.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
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

            case Constants.ScytheIndex:
                fields[MinDamage] = 1.ToString();
                fields[MaxDamage] = 1.ToString();
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                break;
            case Constants.GoldenScytheIndex:
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
