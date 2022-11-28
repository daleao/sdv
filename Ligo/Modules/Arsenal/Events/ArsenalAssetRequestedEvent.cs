namespace DaLion.Ligo.Modules.Arsenal.Events;

#region using directives

using System.Collections.Generic;
using System.Globalization;
using DaLion.Shared.Events;
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

    private static readonly Dictionary<string, (Action<IAssetData> Callback, AssetEditPriority Priority)> AssetEditors =
        new();

    private static readonly Dictionary<string, (Func<string> Provide, AssetLoadPriority Priority)> AssetProviders =
        new();

    /// <summary>Initializes a new instance of the <see cref="ArsenalAssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ArsenalAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        AssetEditors["Data/ObjectInformation"] =
            (Callback: EditObjectInformationData, Priority: AssetEditPriority.Default);
        AssetEditors["Data/weapons"] = (Callback: EditWeaponsData, Priority: AssetEditPriority.Late);
        AssetEditors["Strings/Locations"] = (Callback: EditLocationsStrings, Priority: AssetEditPriority.Default);
        AssetEditors["Strings/StringsFromCSFiles"] =
            (Callback: EditStringsFromCsFilesStrings, Priority: AssetEditPriority.Default);
        AssetEditors["TileSheets/BuffsIcons"] =
            (Callback: EditBuffsIconsTileSheet, Priority: AssetEditPriority.Default);
        AssetEditors["TileSheets/Projectiles"] =
            (Callback: EditProjectilesTileSheet, Priority: AssetEditPriority.Default);
        AssetEditors["TileSheets/weapons"] =
            (Callback: EditWeaponsTileSheet, Priority: AssetEditPriority.Default);

        AssetProviders[$"{ModEntry.Manifest.UniqueID}/InfinityCollisionAnimation"] = (Provide: () => "assets/animations/infinity.png",
            Priority: AssetLoadPriority.Medium);
        AssetProviders[$"{ModEntry.Manifest.UniqueID}/QuincyCollisionAnimation"] = (Provide: () => "assets/animations/quincy.png",
            Priority: AssetLoadPriority.Medium);
        AssetProviders[$"{ModEntry.Manifest.UniqueID}/SnowballCollisionAnimation"] = (Provide: () => "assets/animations/snowball.png",
            Priority: AssetLoadPriority.Medium);
    }

    /// <inheritdoc />
    protected override void OnAssetRequestedImpl(object? sender, AssetRequestedEventArgs e)
    {
        if (AssetEditors.TryGetValue(e.NameWithoutLocale.Name, out var editor))
        {
            e.Edit(editor.Callback, editor.Priority);
        }
        else if (AssetProviders.TryGetValue(e.NameWithoutLocale.Name, out var provider))
        {
            e.LoadFromModFile<Texture2D>(provider.Provide(), provider.Priority);
        }
    }

    #region editor callbacks

    /// <summary>Patches buffs icons with energized buff icon.</summary>
    private static void EditBuffsIconsTileSheet(IAssetData asset)
    {
        var editor = asset.AsImage();
        editor.ExtendImage(192, 64);

        var srcArea = new Rectangle(64, 16, 16, 16);
        var targetArea = new Rectangle(96, 48, 16, 16);
        editor.PatchImage(
            Textures.BuffSheetTx,
            srcArea,
            targetArea);
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
        fields[5] = ModEntry.i18n.Get("galaxysoul.desc");
        data[Constants.GalaxySoulIndex] = string.Join('/', fields);
    }

    /// <summary>Edits location string data with custom legendary sword rhyme.</summary>
    private static void EditLocationsStrings(IAssetData asset)
    {
        if (!ModEntry.Config.Arsenal.InfinityPlusOne)
        {
            return;
        }

        var data = asset.AsDictionary<string, string>().Data;
        data["Town_DwarfGrave_Translated"] = ModEntry.i18n.Get("locations.Town_DwarfGrave_Translated");
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

    /// <summary>Edits strings data with custom legendary sword reward prompt.</summary>
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
            Textures.ProjectileSheetTx,
            srcArea,
            targetArea);
    }

    /// <summary>Edits weapons data with rebalanced stats.</summary>
    private static void EditWeaponsData(IAssetData asset)
    {
        if (!ModEntry.Config.Arsenal.Weapons.RebalancedWeapons && !ModEntry.Config.Arsenal.Weapons.BringBackStabbySwords)
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
                switch (key)
                {
                    #region weapon switch-case

                    #region swords

                    case 0: // rusty sword (removed)
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
                        fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 11: // steel smallsword
                        fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 0.ToString();
                        fields[Precision] = 0.ToString();
                        fields[Defense] = 0.ToString();
                        fields[BaseDropLevel] = 5.ToString();
                        fields[MinDropLevel] = (-1).ToString();
                        fields[Aoe] = 0.ToString();
                        fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 44: // cutlass
                        fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 0.ToString();
                        fields[Precision] = 0.ToString();
                        fields[Defense] = 0.ToString();
                        fields[BaseDropLevel] = 20.ToString();
                        fields[MinDropLevel] = 5.ToString();
                        fields[Aoe] = 4.ToString();
                        fields[CritChance] = 0.09375.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 49: // rapier
                        fields[Knockback] = 0.6.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 2.ToString();
                        fields[Precision] = 2.ToString();
                        fields[Defense] = 0.ToString();
                        fields[BaseDropLevel] = 35.ToString();
                        fields[MinDropLevel] = 15.ToString();
                        fields[Aoe] = (-8).ToString();
                        fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 1.75.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 50: // steel falchion
                        fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 1.ToString();
                        fields[Precision] = 0.ToString();
                        fields[Defense] = 0.ToString();
                        fields[BaseDropLevel] = 50.ToString();
                        fields[MinDropLevel] = 25.ToString();
                        fields[Aoe] = 0.ToString();
                        fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 2.25.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 1: // silver saber
                        fields[Knockback] = 0.8.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 0.ToString();
                        fields[Precision] = 0.ToString();
                        fields[Defense] = 0.ToString();
                        fields[BaseDropLevel] = 10.ToString();
                        fields[MinDropLevel] = 1.ToString();
                        fields[Aoe] = 0.ToString();
                        fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 6: // iron edge
                        fields[Knockback] = 0.85.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = (-1).ToString();
                        fields[Defense] = 1.ToString();
                        fields[BaseDropLevel] = 20.ToString();
                        fields[MinDropLevel] = 5.ToString();
                        fields[Aoe] = 8.ToString();
                        fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 10: // claymore
                        fields[Knockback] = 1.0.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = (-3).ToString();
                        fields[Defense] = 2.ToString();
                        fields[BaseDropLevel] = 35.ToString();
                        fields[MinDropLevel] = 15.ToString();
                        fields[Aoe] = 12.ToString();
                        fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 52: // tempered broadsword
                        fields[Knockback] = 0.9.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = (-2).ToString();
                        fields[Defense] = 2.ToString();
                        fields[BaseDropLevel] = 50.ToString();
                        fields[MinDropLevel] = 25.ToString();
                        fields[Aoe] = 8.ToString();
                        fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 7: // templar's blade
                        fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 0.ToString();
                        fields[Defense] = 1.ToString();
                        fields[Precision] = 0.ToString();
                        fields[BaseDropLevel] = 80.ToString();
                        fields[MinDropLevel] = 50.ToString();
                        fields[Aoe] = 4.ToString();
                        fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 2.25.ToString(CultureInfo.InvariantCulture);
                        break;

                    case 15: // forest sword (scavenger hunt)
                        fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 1.ToString();
                        fields[Precision] = 1.ToString();
                        fields[Defense] = 0.ToString();
                        fields[BaseDropLevel] = (-1).ToString();
                        fields[MinDropLevel] = (-1).ToString();
                        fields[Aoe] = 4.ToString();
                        fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 5: // bone sword (prospector hunt)
                        fields[Knockback] = 0.6.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 2.ToString();
                        fields[Precision] = 0.ToString();
                        fields[Defense] = 0.ToString();
                        fields[BaseDropLevel] = (-1).ToString();
                        fields[MinDropLevel] = (-1).ToString();
                        fields[Aoe] = 0.ToString();
                        fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 1.8.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 60: // ossified blade (prospector hunt)
                        fields[Knockback] = 0.8.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = (-1).ToString();
                        fields[Defense] = 1.ToString();
                        fields[Precision] = 0.ToString();
                        fields[BaseDropLevel] = (-1).ToString();
                        fields[MinDropLevel] = (-1).ToString();
                        fields[Aoe] = 4.ToString();
                        fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 2.25.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 43: // pirate sword (fishing chest)
                        fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 0.ToString();
                        fields[Precision] = 0.ToString();
                        fields[Defense] = 0.ToString();
                        fields[BaseDropLevel] = (-1).ToString();
                        fields[MinDropLevel] = (-1).ToString();
                        fields[Aoe] = 0.ToString();
                        fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                        break;

                    // UNIQUE SWORDS
                    case 14: // neptune's glaive (fishing chest)
                        fields[MinDamage] = 35.ToString();
                        fields[MaxDamage] = 60.ToString();
                        fields[Knockback] = 0.8.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = (-2).ToString();
                        fields[Defense] = 2.ToString();
                        fields[Precision] = 1.ToString();
                        fields[BaseDropLevel] = (-1).ToString();
                        fields[MinDropLevel] = (-1).ToString();
                        fields[Aoe] = 8.ToString();
                        fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 48: // yeti's tooth (mine chest, level 70)
                        fields[MinDamage] = 40.ToString();
                        fields[MaxDamage] = 55.ToString();
                        fields[Knockback] = 0.8.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = (-2).ToString();
                        fields[Defense] = 1.ToString();
                        fields[BaseDropLevel] = (-1).ToString();
                        fields[MinDropLevel] = (-1).ToString();
                        fields[Aoe] = 8.ToString();
                        fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 8: // obsidian edge
                        fields[MinDamage] = 50.ToString();
                        fields[MaxDamage] = 80.ToString();
                        fields[Knockback] = 0.6.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 2.ToString();
                        fields[Precision] = 0.ToString();
                        fields[Defense] = 0.ToString();
                        fields[BaseDropLevel] = (-1).ToString();
                        fields[MinDropLevel] = (-1).ToString();
                        fields[Aoe] = 0.ToString();
                        fields[CritChance] = 0.09375.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 9: // lava katana
                        fields[MinDamage] = 75.ToString();
                        fields[MaxDamage] = 90.ToString();
                        fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 1.ToString();
                        fields[Precision] = 1.ToString();
                        fields[Defense] = 0.ToString();
                        fields[BaseDropLevel] = (-1).ToString();
                        fields[MinDropLevel] = (-1).ToString();
                        fields[Aoe] = 4.ToString();
                        fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 2.25.ToString(CultureInfo.InvariantCulture);
                        break;

                    // BIS SWORDS
                    case 2: // dark sword
                        fields[Name] = ModEntry.i18n.Get("darksword.name");
                        fields[Description] = ModEntry.i18n.Get("darksword.desc");
                        fields[MinDamage] = 100.ToString();
                        fields[MaxDamage] = 140.ToString();
                        fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 0.ToString();
                        fields[Precision] = 0.ToString();
                        fields[Defense] = 0.ToString();
                        fields[Aoe] = 8.ToString();
                        fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 3: // holy blade
                        fields[Name] = ModEntry.i18n.Get("holyblade.name");
                        fields[Description] = ModEntry.i18n.Get("holyblade.desc");
                        fields[MinDamage] = 120.ToString();
                        fields[MaxDamage] = 160.ToString();
                        fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 0.ToString();
                        fields[Precision] = 0.ToString();
                        fields[Defense] = 2.ToString();
                        fields[Aoe] = 8.ToString();
                        fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
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
                        fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                        break;

                    case 54: // dwarf sword
                        fields[MinDamage] = 130.ToString();
                        fields[MaxDamage] = 175.ToString();
                        fields[Knockback] = 0.85.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = (-2).ToString();
                        fields[Precision] = 0.ToString();
                        fields[Defense] = 3.ToString();
                        fields[BaseDropLevel] = (-1).ToString();
                        fields[MinDropLevel] = (-1).ToString();
                        fields[Aoe] = 12.ToString();
                        fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 57: // dragontooth cutlass
                        fields[MinDamage] = 160.ToString();
                        fields[MaxDamage] = 200.ToString();
                        fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = (-1).ToString();
                        fields[Precision] = 1.ToString();
                        fields[Defense] = (-1).ToString();
                        fields[BaseDropLevel] = (-1).ToString();
                        fields[MinDropLevel] = (-1).ToString();
                        fields[Aoe] = 0.ToString();
                        fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 2.5.ToString(CultureInfo.InvariantCulture);
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
                        fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                        break;

                    #endregion swords

                    #region daggers

                    // BASIC DAGGERS
                    case 16: // carving knife
                        fields[Knockback] = 0.4.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 0.ToString();
                        fields[Precision] = 0.ToString();
                        fields[Defense] = 0.ToString();
                        fields[BaseDropLevel] = 1.ToString();
                        fields[MinDropLevel] = (-1).ToString();
                        fields[Aoe] = 0.ToString();
                        fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 18: // burglar's shank
                        fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 0.ToString();
                        fields[Precision] = 0.ToString();
                        fields[Defense] = 0.ToString();
                        fields[BaseDropLevel] = 20.ToString();
                        fields[MinDropLevel] = 5.ToString();
                        fields[Aoe] = 0.ToString();
                        fields[CritChance] = 0.15.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 22: // wind spire
                        fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 1.ToString();
                        fields[Precision] = 0.ToString();
                        fields[Defense] = 0.ToString();
                        fields[BaseDropLevel] = 35.ToString();
                        fields[MinDropLevel] = 15.ToString();
                        fields[Aoe] = 4.ToString();
                        fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 17: // iron dirk
                        fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 0.ToString();
                        fields[Precision] = 0.ToString();
                        fields[Defense] = 0.ToString();
                        fields[BaseDropLevel] = 50.ToString();
                        fields[MinDropLevel] = 25.ToString();
                        fields[Aoe] = 0.ToString();
                        fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 1.75.ToString(CultureInfo.InvariantCulture);
                        break;

                    case 21: // crystal dagger (drop from icy level)
                        fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 0.ToString();
                        fields[Precision] = 0.ToString();
                        fields[Defense] = 0.ToString();
                        fields[BaseDropLevel] = (-1).ToString();
                        fields[MinDropLevel] = (-1).ToString();
                        fields[Aoe] = 4.ToString();
                        fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 19: // shadow dagger (drop from dark level)
                        fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 0.ToString();
                        fields[Precision] = 0.ToString();
                        fields[Defense] = (-1).ToString();
                        fields[BaseDropLevel] = (-1).ToString();
                        fields[MinDropLevel] = (-1).ToString();
                        fields[Aoe] = 8.ToString();
                        fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 45: // wicked kriss (drop from dark level)
                        fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 0.ToString();
                        fields[Precision] = 0.ToString();
                        fields[Defense] = (-1).ToString();
                        fields[BaseDropLevel] = (-1).ToString();
                        fields[MinDropLevel] = (-1).ToString();
                        fields[Aoe] = 8.ToString();
                        fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 20: // elf blade (scavenger hunt)
                        fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 1.ToString();
                        fields[Precision] = 1.ToString();
                        fields[Defense] = 0.ToString();
                        fields[BaseDropLevel] = (-1).ToString();
                        fields[MinDropLevel] = (-1).ToString();
                        fields[Aoe] = 0.ToString();
                        fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;

                    // UNIQUE DAGGERS
                    case 51: // broken trident (fishing chest)
                        fields[MinDamage] = 30.ToString();
                        fields[MaxDamage] = 45.ToString();
                        fields[Knockback] = 0.4.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 0.ToString();
                        fields[Precision] = 1.ToString();
                        fields[Defense] = (-1).ToString();
                        fields[BaseDropLevel] = (-1).ToString();
                        fields[MinDropLevel] = (-1).ToString();
                        fields[Aoe] = 12.ToString();
                        fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 1.4.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 13: // insect head (quest)
                        fields[MinDamage] = 13.ToString();
                        fields[MaxDamage] = 63.ToString();
                        fields[Knockback] = 0.3333.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = (-1).ToString();
                        fields[Precision] = (-1).ToString();
                        fields[Defense] = (-2).ToString();
                        fields[Type] = 1.ToString();
                        fields[BaseDropLevel] = (-1).ToString();
                        fields[MinDropLevel] = (-1).ToString();
                        fields[Aoe] = (-4).ToString();
                        fields[CritChance] = 0.14167.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 1.3.ToString(CultureInfo.InvariantCulture);
                        break;

                    // BIS DAGGERS
                    case 23: // galaxy dagger
                        fields[MinDamage] = 50.ToString();
                        fields[MaxDamage] = 70.ToString();
                        fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 0.ToString();
                        fields[Precision] = 0.ToString();
                        fields[Defense] = 0.ToString();
                        fields[BaseDropLevel] = (-1).ToString();
                        fields[MinDropLevel] = (-1).ToString();
                        fields[Aoe] = 4.ToString();
                        fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 61: // iridium needle (quest or drop)
                        fields[MinDamage] = 45.ToString();
                        fields[MaxDamage] = 60.ToString();
                        fields[Knockback] = 0.3.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 1.ToString();
                        fields[Precision] = 2.ToString();
                        fields[Defense] = (-2).ToString();
                        fields[BaseDropLevel] = (-1).ToString();
                        fields[MinDropLevel] = (-1).ToString();
                        fields[Aoe] = (-8).ToString();
                        fields[CritChance] = 0.2.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 4.0.ToString(CultureInfo.InvariantCulture);
                        break;

                    case 56: // dwarf dagger
                        fields[MinDamage] = 95.ToString();
                        fields[MaxDamage] = 115.ToString();
                        fields[Knockback] = 0.65.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = (-1).ToString();
                        fields[Precision] = 0.ToString();
                        fields[Defense] = 2.ToString();
                        fields[BaseDropLevel] = (-1).ToString();
                        fields[MinDropLevel] = (-1).ToString();
                        fields[Aoe] = 12.ToString();
                        fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 59: // dragontooth shiv
                        fields[MinDamage] = 110.ToString();
                        fields[MaxDamage] = 130.ToString();
                        fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = (-1).ToString();
                        fields[Precision] = 1.ToString();
                        fields[Defense] = (-1).ToString();
                        fields[BaseDropLevel] = (-1).ToString();
                        fields[MinDropLevel] = (-1).ToString();
                        fields[Aoe] = 0.ToString();
                        fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 1.875.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 64: // infinity dagger
                        fields[MinDamage] = 100.ToString();
                        fields[MaxDamage] = 120.ToString();
                        fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 0.ToString();
                        fields[Precision] = 0.ToString();
                        fields[Defense] = 0.ToString();
                        fields[BaseDropLevel] = (-1).ToString();
                        fields[MinDropLevel] = (-1).ToString();
                        fields[Aoe] = 8.ToString();
                        fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;

                    #endregion daggers

                    #region clubs

                    // BASIC CLUBS
                    case 24: // wood club
                        fields[Knockback] = 1.0.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 0.ToString();
                        fields[Precision] = 0.ToString();
                        fields[Defense] = 0.ToString();
                        fields[BaseDropLevel] = 5.ToString();
                        fields[MinDropLevel] = (-1).ToString();
                        fields[Aoe] = 0.ToString();
                        fields[CritChance] = 0.03125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 27: // wood mallet
                        fields[Knockback] = 1.0.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 0.ToString();
                        fields[Precision] = 0.ToString();
                        fields[Defense] = 0.ToString();
                        fields[BaseDropLevel] = 20.ToString();
                        fields[MinDropLevel] = 5.ToString();
                        fields[Aoe] = 4.ToString();
                        fields[CritChance] = 0.03125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 3.0.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 26: // lead rod
                        fields[Knockback] = 1.2.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = (-3).ToString();
                        fields[Precision] = (-1).ToString();
                        fields[Defense] = 1.ToString();
                        fields[BaseDropLevel] = 35.ToString();
                        fields[MinDropLevel] = 15.ToString();
                        fields[Aoe] = 8.ToString();
                        fields[CritChance] = 0.03125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 3.0.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 46: // kudgel
                        fields[Knockback] = 1.1.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = (-2).ToString();
                        fields[Precision] = 0.ToString();
                        fields[Defense] = 0.ToString();
                        fields[BaseDropLevel] = 50.ToString();
                        fields[MinDropLevel] = 25.ToString();
                        fields[Aoe] = 8.ToString();
                        fields[CritChance] = 0.03125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 3.0.ToString(CultureInfo.InvariantCulture);
                        break;

                    case 31: // femur (prospector hunt)
                        fields[Knockback] = 1.1.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = (-1).ToString();
                        fields[Precision] = (-1).ToString();
                        fields[Defense] = 1.ToString();
                        fields[BaseDropLevel] = (-1).ToString();
                        fields[MinDropLevel] = (-1).ToString();
                        fields[Aoe] = 4.ToString();
                        fields[CritChance] = 0.03125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 3.0.ToString(CultureInfo.InvariantCulture);
                        break;

                    // UNIQUE CLUBS
                    case 28: // the slammer
                        fields[MinDamage] = 50.ToString();
                        fields[MaxDamage] = 125.ToString();
                        fields[Knockback] = 1.2.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = (-2).ToString();
                        fields[Precision] = 0.ToString();
                        fields[Defense] = 1.ToString();
                        fields[BaseDropLevel] = (-1).ToString();
                        fields[MinDropLevel] = (-1).ToString();
                        fields[Aoe] = 8.ToString();
                        fields[CritChance] = 0.03125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 3.3.ToString(CultureInfo.InvariantCulture);
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
                        fields[CritChance] = 0.03125.ToString(CultureInfo.InvariantCulture);
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
                        fields[CritChance] = 0.03125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 3.0.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 58: // dragontooth club
                        fields[MinDamage] = 120.ToString();
                        fields[MaxDamage] = 360.ToString();
                        fields[Knockback] = 1.0.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = (-1).ToString();
                        fields[Precision] = 1.ToString();
                        fields[Defense] = (-1).ToString();
                        fields[BaseDropLevel] = (-1).ToString();
                        fields[MinDropLevel] = (-1).ToString();
                        fields[Aoe] = 4.ToString();
                        fields[CritChance] = 0.03125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 3.75.ToString(CultureInfo.InvariantCulture);
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
                        fields[CritChance] = 0.03125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 3.0.ToString(CultureInfo.InvariantCulture);
                        break;

                    #endregion clubs

                    #region bachelor(ette) weapons

                    case 40: // abby (dagger)
                        fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 0.ToString();
                        fields[Precision] = 1.ToString();
                        fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 42: // haley (sword)
                        fields[Knockback] = 0.625.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 0.ToString();
                        fields[Precision] = 0.ToString();
                        fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 39: // leah (dagger)
                        fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 0.ToString();
                        fields[Precision] = 0.ToString();
                        fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 36: // maru (club)
                        fields[Knockback] = 0.8.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 0.ToString();
                        fields[Precision] = 0.ToString();
                        fields[CritChance] = 0.03125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 2.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 38: // penny (club)
                        fields[Knockback] = 0.8.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 0.ToString();
                        fields[Precision] = 0.ToString();
                        fields[CritChance] = 0.03125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 25: // alex (club)
                        fields[Knockback] = 0.9.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 1.ToString();
                        fields[Precision] = 1.ToString();
                        fields[CritChance] = 0.03125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 2.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 35: // eliott (dagger)
                        fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 1.ToString();
                        fields[Precision] = 0.ToString();
                        fields[Aoe] = (-8).ToString();
                        fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 1.25.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 37: // harvey (club)
                        fields[Knockback] = 0.8.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 1.ToString();
                        fields[Precision] = 1.ToString();
                        fields[CritChance] = 0.03125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 2.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 30: // sam (club)
                        fields[Knockback] = 1.0.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = (-2).ToString();
                        fields[Precision] = (-1).ToString();
                        fields[Aoe] = 8.ToString();
                        fields[CritChance] = 0.03125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 41: // seb (club)
                        fields[Knockback] = 1.0.ToString(CultureInfo.InvariantCulture);
                        fields[Speed] = 0.ToString();
                        fields[Precision] = 0.ToString();
                        fields[Aoe] = 4.ToString();
                        fields[CritChance] = 0.03125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 3.0.ToString(CultureInfo.InvariantCulture);
                        break;

                    #endregion bachelor(ette) weapons

                    #endregion weapon switch-case
                }
            }

            if (ModEntry.Config.Arsenal.Weapons.BringBackStabbySwords &&
                ModEntry.Config.Arsenal.Weapons.StabbySwords.Contains(fields[Name]))
            {
                fields[Type] = "0";
            }

            if (ModEntry.Config.Arsenal.AncientCrafting && fields[Name].Contains("Dwarf"))
            {
                fields[Name] = fields[Name].Replace("Dwarf", "Dwarven");
            }

            if (ModEntry.Config.Arsenal.InfinityPlusOne)
            {
                switch (key)
                {
                    case Constants.DarkSwordIndex:
                        fields[Name] = ModEntry.i18n.Get("darksword.name");
                        fields[Description] = ModEntry.i18n.Get("darksword.desc");
                        break;
                    case Constants.HolyBladeIndex:
                        fields[Name] = ModEntry.i18n.Get("holyblade.name");
                        fields[Description] = ModEntry.i18n.Get("holyblade.desc");
                        break;
                }
            }

            data[key] = string.Join('/', fields);
        }
    }

    /// <summary>Edits weapons tilesheet with touched up textures.</summary>
    private static void EditWeaponsTileSheet(IAssetData asset)
    {
        if (!ModEntry.Config.Arsenal.Weapons.RetexturedWeapons)
        {
            return;
        }

        var editor = asset.AsImage();
        var srcArea = new Rectangle(0, 0, 128, 144);
        var targetArea = new Rectangle(0, 0, 128, 144);
        editor.PatchImage(
            Textures.WeaponSheetTx,
            srcArea,
            targetArea);
    }

    #endregion editor callbacks
}
