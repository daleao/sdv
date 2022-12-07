namespace DaLion.Ligo.Modules.Arsenal.Events;

#region using directives

using System.Globalization;
using DaLion.Shared.Content;
using DaLion.Shared.Events;
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
        this.Edit("Data/ObjectInformation", new AssetEditor(EditObjectInformationData, AssetEditPriority.Default));
        this.Edit("Data/Events/AdventureGuild", new AssetEditor(EditSveEventsData, AssetEditPriority.Late));
        this.Edit("Data/Events/Blacksmith", new AssetEditor(EditBlacksmithEventsData, AssetEditPriority.Default));
        this.Edit("Data/Events/WizardHouse", new AssetEditor(EditWizardEventsData, AssetEditPriority.Default));
        this.Edit("Data/Monsters", new AssetEditor(EditMonstersData, AssetEditPriority.Late));
        this.Edit("Data/Quests", new AssetEditor(EditQuestsData, AssetEditPriority.Default));
        this.Edit("Data/weapons", new AssetEditor(EditWeaponsData, AssetEditPriority.Late));
        this.Edit("Strings/Locations", new AssetEditor(EditLocationsStrings, AssetEditPriority.Default));
        this.Edit(
            "Strings/StringsFromCSFiles",
            new AssetEditor(EditStringsFromCsFilesStrings, AssetEditPriority.Default));
        this.Edit("TileSheets/BuffsIcons", new AssetEditor(EditBuffsIconsTileSheet, AssetEditPriority.Default));
        this.Edit("TileSheets/Projectiles", new AssetEditor(EditProjectilesTileSheet, AssetEditPriority.Default));
        this.Edit("TileSheets/weapons", new AssetEditor(EditWeaponsTileSheetEarly, AssetEditPriority.Early));
        this.Edit("TileSheets/weapons", new AssetEditor(EditWeaponsTileSheetLate, AssetEditPriority.Early));

        this.Provide(
            $"{ModEntry.Manifest.UniqueID}/InfinityCollisionAnimation",
            new ModTextureProvider(() => "assets/animations/infinity.png", AssetLoadPriority.Medium));
        this.Provide(
            $"{ModEntry.Manifest.UniqueID}/QuincyCollisionAnimation",
            new ModTextureProvider(() => "assets/animations/quincy.png", Priority: AssetLoadPriority.Medium));
        this.Provide(
            $"{ModEntry.Manifest.UniqueID}/SnowballCollisionAnimation",
            new ModTextureProvider(() => "assets/animations/snowball.png", Priority: AssetLoadPriority.Medium));
        this.Provide("Data/Events/Blacksmith", new DictionaryProvider<string, string>(null, AssetLoadPriority.Low));
    }

    #region editor callbacks

    /// <summary>Patches buffs icons with energized buff icon.</summary>
    private static void EditBuffsIconsTileSheet(IAssetData asset)
    {
        if (!ModEntry.Config.Arsenal.Weapons.UseLigoEnchants)
        {
            return;
        }

        var editor = asset.AsImage();
        editor.ExtendImage(192, 64);

        var srcArea = new Rectangle(64, 16, 16, 16);
        var targetArea = new Rectangle(96, 48, 16, 16);
        editor.PatchImage(
            ModEntry.ModHelper.ModContent.Load<Texture2D>("assets/sprites/buffs"),
            srcArea,
            targetArea);
    }

    /// <summary>Edits location string data with custom legendary sword rhyme.</summary>
    private static void EditLocationsStrings(IAssetData asset)
    {
        if (!ModEntry.Config.Arsenal.InfinityPlusOne)
        {
            return;
        }

        var data = asset.AsDictionary<string, string>().Data;
        data["Town_DwarfGrave_Translated"] = ModEntry.i18n.Get("locations.Town.DwarfGrave.Translated");
        data["SeedShop_Yoba"] = ModEntry.i18n.Get("locations.SeedShop.Yoba");
    }

    /// <summary>Edits galaxy soul description.</summary>
    private static void EditObjectInformationData(IAssetData asset)
    {
        if (!ModEntry.Config.Arsenal.InfinityPlusOne)
        {
            return;
        }

        var data = asset.AsDictionary<int, string>().Data;

        // edit galaxy soul description
        var fields = data[Constants.GalaxySoulIndex].Split('/');
        fields[5] = ModEntry.i18n.Get("objects.galaxysoul.desc");
        data[Constants.GalaxySoulIndex] = string.Join('/', fields);
    }

    /// <summary>Edits strings data with custom legendary sword reward prompt.</summary>
    private static void EditStringsFromCsFilesStrings(IAssetData asset)
    {
        if (!ModEntry.Config.Arsenal.InfinityPlusOne)
        {
            return;
        }

        var data = asset.AsDictionary<string, string>().Data;
        data["MeleeWeapon.cs.14122"] = ModEntry.i18n.Get("fromcsfiles.MeleeWeapon.cs.14122");
    }

    /// <summary>Edits events data with custom Dwarvish Blueprint introduction event.</summary>
    private static void EditBlacksmithEventsData(IAssetData asset)
    {
        if (!ModEntry.Config.Arsenal.DwarvishCrafting)
        {
            return;
        }

        var data = asset.AsDictionary<string, string>().Data;
        data["144701/n dwarvishBlueprintFound/n canUnderstandDwarves/f Clint 1500/p Clint"] =
            ModEntry.i18n.Get("events.forge.intro");
    }

    /// <summary>Edits events data with custom Blade of Ruin introduction event.</summary>
    private static void EditWizardEventsData(IAssetData asset)
    {
        if (!ModEntry.Config.Arsenal.InfinityPlusOne)
        {
            return;
        }

        var data = asset.AsDictionary<string, string>().Data;
        data["144703/n darkSwordFound/p Wizard"] = ModEntry.i18n.Get("events.curse.intro");
    }

    /// <summary>Edits Marlon's Galaxy Sword event in SVE, removing references to purchasable Galaxy weapons.</summary>
    private static void EditSveEventsData(IAssetData asset)
    {
        var data = asset.AsDictionary<string, string>().Data;
        if (data.ContainsKey("1337098") && ModEntry.Config.Arsenal.InfinityPlusOne)
        {
            data["1337098"] = ModEntry.i18n.Get("events.1337098.nopurchase");
        }
    }

    /// <summary>Edits quests data with custom Dwarvish Blueprint introduction quest.</summary>
    private static void EditQuestsData(IAssetData asset)
    {
        if (!ModEntry.Config.Arsenal.DwarvishCrafting)
        {
            return;
        }

        var data = asset.AsDictionary<int, string>().Data;
        data[Constants.ForgeIntroQuestId] = ModEntry.i18n.Get("quests.forge.intro");
        data[Constants.ForgeNextQuestId] = ModEntry.i18n.Get("quests.forge.next");
        data[Constants.CurseQuestId] = ModEntry.i18n.Get("quests.curse");
    }

    /// <summary>Edits monsters data for ancient weapon crafting materials.</summary>
    private static void EditMonstersData(IAssetData asset)
    {
        if (!ModEntry.Config.Arsenal.DwarvishCrafting)
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

    /// <summary>Adds the infinity enchantment projectile.</summary>
    private static void EditProjectilesTileSheet(IAssetData asset)
    {
        if (!ModEntry.Config.Arsenal.InfinityPlusOne)
        {
            return;
        }

        var editor = asset.AsImage();
        var srcArea = new Rectangle(0, 0, 16, 16);
        var targetArea = new Rectangle(112, 16, 16, 16);
        editor.PatchImage(
            ModEntry.ModHelper.ModContent.Load<Texture2D>("assets/sprites/projectiles"),
            srcArea,
            targetArea);
    }

    /// <summary>Edits weapons data with rebalanced stats.</summary>
    private static void EditWeaponsData(IAssetData asset)
    {
        if (!ModEntry.Config.Arsenal.Weapons.RebalancedWeapons &&
            !ModEntry.Config.Arsenal.Weapons.BringBackStabbySwords && !ModEntry.Config.Arsenal.DwarvishCrafting &&
            !ModEntry.Config.Arsenal.InfinityPlusOne)
        {
            return;
        }

        var data = asset.AsDictionary<int, string>().Data;
        var keys = data.Keys;
        foreach (var key in keys)
        {
            var fields = data[key].Split('/');

            if (ModEntry.Config.Arsenal.Weapons.RebalancedWeapons)
            {
                EditSpecificWeapon(key, fields);
            }

            if (ModEntry.Config.Arsenal.Weapons.BringBackStabbySwords &&
                ModEntry.Config.Arsenal.Weapons.StabbySwords.Contains(fields[Name]))
            {
                fields[Type] = "0";
            }

            if (ModEntry.Config.Arsenal.DwarvishCrafting && fields[Name].Contains("Dwarf"))
            {
                fields[Name] = fields[Name].Replace("Dwarf", "Dwarven");
            }

            if (ModEntry.Config.Arsenal.InfinityPlusOne)
            {
                switch (key)
                {
                    case Constants.DarkSwordIndex:
                        fields[Name] = ModEntry.i18n.Get("weapons.darksword.name");
                        fields[Description] = ModEntry.i18n.Get("weapons.darksword.desc");
                        break;
                    case Constants.HolyBladeIndex:
                        fields[Name] = ModEntry.i18n.Get("weapons.holyblade.name");
                        fields[Description] = ModEntry.i18n.Get("weapons.holyblade.desc");
                        break;
                }
            }

            data[key] = string.Join('/', fields);
        }

        if (ModEntry.Config.Arsenal.InfinityPlusOne)
        {
            data[Constants.InfinitySlingshotIndex] = string.Format(
                "Infinity Slingshot/{0}/1/3/1/308/0/0/4/-1/-1/0/.02/3/{1}",
                ModEntry.i18n.Get("slingshots.infinity.desc"),
                ModEntry.i18n.Get("slingshots.infinity.name"));
        }
    }

    /// <summary>Edits weapons tilesheet with touched up textures.</summary>
    private static void EditWeaponsTileSheetEarly(IAssetData asset)
    {
        if (!ModEntry.Config.Arsenal.Weapons.RetexturedWeapons || Ligo.Integrations.UsingVanillaTweaksWeapons)
        {
            return;
        }

        var editor = asset.AsImage();
        editor.PatchImage(ModEntry.ModHelper.ModContent.Load<Texture2D>("assets/sprites/weapons"));
    }

    /// <summary>Edits weapons tilesheet with touched up textures.</summary>
    private static void EditWeaponsTileSheetLate(IAssetData asset)
    {
        if (!Ligo.Integrations.UsingVanillaTweaksWeapons || !ModEntry.Config.Arsenal.InfinityPlusOne)
        {
            return;
        }

        var editor = asset.AsImage();
        var targetArea = new Rectangle(32, 0, 32, 16);
        editor.PatchImage(ModEntry.ModHelper.ModContent.Load<Texture2D>("assets/sprites/VanillaTweaks/swords"), null, targetArea);
    }

    #endregion editor callbacks

    #region helpers

    private static void EditSpecificWeapon(int key, string[] fields)
    {
        switch (key)
        {
            #region swords

            case 0: // rusty sword (removed)
                fields[MinDamage] = 1.ToString();
                fields[MaxDamage] = 5.ToString();
                fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-1).ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[Aoe] = 0.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                break;

            // BASIC SWORDS
            case 12: // wooden blade
                fields[MinDamage] = 1.ToString();
                fields[MaxDamage] = 3.ToString();
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 0.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                break;
            case 11: // steel smallsword
                fields[MinDamage] = 6.ToString();
                fields[MaxDamage] = 10.ToString();
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
            case 44: // cutlass
                fields[MinDamage] = 18.ToString();
                fields[MaxDamage] = 24.ToString();
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
            case 49: // rapier
                fields[MinDamage] = 30.ToString();
                fields[MaxDamage] = 40.ToString();
                fields[Knockback] = 0.6.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 2.ToString();
                fields[Precision] = 2.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = 50.ToString();
                fields[MinDropLevel] = 25.ToString();
                fields[Aoe] = (-8).ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.75.ToString(CultureInfo.InvariantCulture);
                break;
            case 50: // steel falchion
                fields[MinDamage] = 40.ToString();
                fields[MaxDamage] = 54.ToString();
                fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 1.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = 80.ToString();
                fields[MinDropLevel] = 50.ToString();
                fields[Aoe] = 0.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.4.ToString(CultureInfo.InvariantCulture);
                break;
            case 1: // silver saber
                fields[MinDamage] = 5.ToString();
                fields[MaxDamage] = 8.ToString();
                fields[Knockback] = 0.8.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 1.ToString();
                fields[BaseDropLevel] = 10.ToString();
                fields[MinDropLevel] = 1.ToString();
                fields[Aoe] = 0.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                break;
            case 6: // iron edge
                fields[MinDamage] = 15.ToString();
                fields[MaxDamage] = 20.ToString();
                fields[Knockback] = 0.935.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-1).ToString();
                fields[Defense] = 2.ToString();
                fields[BaseDropLevel] = 25.ToString();
                fields[MinDropLevel] = 10.ToString();
                fields[Aoe] = 8.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                break;
            case 10: // claymore
                fields[MinDamage] = 28.ToString();
                fields[MaxDamage] = 38.ToString();
                fields[Knockback] = 1.125.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-3).ToString();
                fields[Defense] = 3.ToString();
                fields[BaseDropLevel] = 50.ToString();
                fields[MinDropLevel] = 25.ToString();
                fields[Aoe] = 12.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.5.ToString(CultureInfo.InvariantCulture);
                break;
            case 52: // tempered broadsword
                fields[MinDamage] = 38.ToString();
                fields[MaxDamage] = 52.ToString();
                fields[Knockback] = 1.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-2).ToString();
                fields[Defense] = 3.ToString();
                fields[BaseDropLevel] = 80.ToString();
                fields[MinDropLevel] = 50.ToString();
                fields[Aoe] = 8.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.ToString(CultureInfo.InvariantCulture);
                break;
            case 7: // templar's blade
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

            case 15: // forest sword (scavenger hunt)
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
                break;
            case 5: // bone sword (prospector hunt)
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
            case 60: // ossified blade (prospector hunt)
                fields[MinDamage] = 64.ToString();
                fields[MaxDamage] = 85.ToString();
                fields[Knockback] = 0.8.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-1).ToString();
                fields[Defense] = 1.ToString();
                fields[Precision] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 4.ToString();
                fields[CritChance] = 0.06.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.25.ToString(CultureInfo.InvariantCulture);
                break;
            case 43: // pirate sword (fishing chest)
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
            case 48: // yeti tooth (special drop from icy levels)
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
            case 8: // obsidian edge
                fields[MinDamage] = 70.ToString();
                fields[MaxDamage] = 95.ToString();
                fields[Knockback] = 0.8625.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 1.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 4.ToString();
                fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.5.ToString(CultureInfo.InvariantCulture);
                break;
            case 9: // lava katana
                fields[Description] = ModEntry.i18n.Get("weapons.lavakatana.desc");
                fields[MinDamage] = 85.ToString();
                fields[MaxDamage] = 110.ToString();
                fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 1.ToString();
                fields[Precision] = 1.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 4.ToString();
                fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.5.ToString(CultureInfo.InvariantCulture);
                break;

            // UNIQUE SWORDS
            case 14: // neptune's glaive (fishing chest)
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
            case 2: // dark sword
                fields[Name] = ModEntry.i18n.Get("weapons.darksword.name");
                fields[Description] = ModEntry.i18n.Get("weapons.darksword.desc");
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
            case 3: // holy blade
                fields[Name] = ModEntry.i18n.Get("weapons.holyblade.name");
                fields[Description] = ModEntry.i18n.Get("weapons.holyblade.desc");
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
            case 4: // galaxy sword
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

            case 54: // dwarf sword
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
            case 57: // dragontooth cutlass
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
            case 62: // infinity blade
                fields[MinDamage] = 140.ToString();
                fields[MaxDamage] = 180.ToString();
                fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 8.ToString();
                fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                break;

            #endregion swords

            #region daggers

            // BASIC DAGGERS
            case 16: // carving knife
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
            case 18: // burglar's shank
                fields[MinDamage] = 13.ToString();
                fields[MaxDamage] = 16.ToString();
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = 25.ToString();
                fields[MinDropLevel] = 10.ToString();
                fields[Aoe] = (-4).ToString();
                fields[CritChance] = 0.15.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                break;
            case 22: // wind spire
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
            case 17: // iron dirk
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
            case 45: // wicked kriss
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

            case 21: // crystal dagger (special drop from icy level)
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
            case 19: // shadow dagger (special drop from dark level)
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
            case 20: // elf blade (scavenger hunt)
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
                break;
            case 51: // broken trident (fishing chest)
                fields[MinDamage] = 50.ToString();
                fields[MaxDamage] = 58.ToString();
                fields[Knockback] = 0.6.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 1.ToString();
                fields[Defense] = (-1).ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 12.ToString();
                fields[CritChance] = 0.15.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                break;

            // UNIQUE DAGGERS
            case 13: // insect head (quest)
                fields[MinDamage] = 1.ToString();
                fields[MaxDamage] = 199.ToString();
                fields[Knockback] = 0.1.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = (-1).ToString();
                fields[Precision] = (-1).ToString();
                fields[Defense] = (-3).ToString();
                fields[Type] = 1.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = (-4).ToString();
                fields[CritChance] = 0.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.ToString(CultureInfo.InvariantCulture);
                break;

            // BIS DAGGERS
            case 23: // galaxy dagger
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
            case 61: // iridium needle (quest or drop)
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
                fields[CritPower] = 1.ToString(CultureInfo.InvariantCulture);
                break;

            case 56: // dwarf dagger
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
            case 59: // dragontooth shiv
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
            case 64: // infinity dagger
                fields[MinDamage] = 105.ToString();
                fields[MaxDamage] = 120.ToString();
                fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 8.ToString();
                fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                break;

            #endregion daggers

            #region clubs

            // BASIC CLUBS
            case 24: // wood club
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
            case 27: // wood mallet
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
            case 26: // lead rod
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
            case 46: // kudgel
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
            case 28: // the slammer
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

            case 31: // femur (prospector hunt)
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
            case 29: // galaxy hammer
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

            case 55: // dwarf hammer
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
            case 58: // dragontooth club
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
            case 63: // infinity gavel
                fields[MinDamage] = 100.ToString();
                fields[MaxDamage] = 300.ToString();
                fields[Knockback] = 1.0.ToString(CultureInfo.InvariantCulture);
                fields[Speed] = 0.ToString();
                fields[Precision] = 0.ToString();
                fields[Defense] = 0.ToString();
                fields[BaseDropLevel] = (-1).ToString();
                fields[MinDropLevel] = (-1).ToString();
                fields[Aoe] = 12.ToString();
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
        }
    }

    #endregion helpers
}
