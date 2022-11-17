namespace DaLion.Ligo.Modules.Arsenal.Weapons.Events;

#region using directives

using System.Collections.Generic;
using System.Globalization;
using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class WeaponAssetRequestedEvent : AssetRequestedEvent
{
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

    private static readonly Dictionary<string, (Action<IAssetData> Callback, AssetEditPriority Priority)> AssetEditors =
        new();

    /// <summary>Initializes a new instance of the <see cref="WeaponAssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal WeaponAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        AssetEditors["TileSheets/BuffsIcons"] =
            (Callback: EditBuffsIconsTileSheet, Priority: AssetEditPriority.Default);
        AssetEditors["Data/weapons"] = (Callback: EditWeaponsData, Priority: AssetEditPriority.Late);
    }

    /// <inheritdoc />
    protected override void OnAssetRequestedImpl(object? sender, AssetRequestedEventArgs e)
    {
        if (AssetEditors.TryGetValue(e.NameWithoutLocale.Name, out var editor))
        {
            e.Edit(editor.Callback, editor.Priority);
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
            Textures.BuffsSheetTx,
            srcArea,
            targetArea);
    }

    /// <summary>Edits weapons data with rebalanced stats.</summary>
    private static void EditWeaponsData(IAssetData asset)
    {
        var data = asset.AsDictionary<int, string>().Data;
        var keys = data.Keys;
        foreach (var key in keys)
        {
            var fields = data[key].Split('/');
            switch (key)
            {
                #region weapon switch-case

                #region stabbing swords

                // BASIC STABBING SWORDS
                case 12: // wooden blade
                    fields[MinDamage] = 2.ToString();
                    fields[MaxDamage] = 5.ToString();
                    fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 1.ToString();
                    fields[Precision] = 0.ToString();
                    fields[Defense] = 0.ToString();
                    fields[Type] = 0.ToString();
                    fields[BaseDropLevel] = (-1).ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[Aoe] = 0.ToString();
                    fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 11: // steel smallsword
                    fields[MinDamage] = 4.ToString();
                    fields[MaxDamage] = 8.ToString();
                    fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 0.ToString();
                    fields[Precision] = 0.ToString();
                    fields[Defense] = 0.ToString();
                    fields[Type] = 0.ToString();
                    fields[BaseDropLevel] = 5.ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                    break;
                case 44: // cutlass
                    fields[MinDamage] = 15.ToString();
                    fields[MaxDamage] = 25.ToString();
                    fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 0.ToString();
                    fields[Precision] = 0.ToString();
                    fields[Defense] = 0.ToString();
                    fields[Type] = 0.ToString();
                    fields[BaseDropLevel] = 20.ToString();
                    fields[MinDropLevel] = 5.ToString();
                    fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                    break;
                case 49: // rapier
                    fields[MinDamage] = 35.ToString();
                    fields[MaxDamage] = 55.ToString();
                    fields[Knockback] = 0.6.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 2.ToString();
                    fields[Precision] = 8.ToString();
                    fields[Defense] = 0.ToString();
                    fields[Type] = 0.ToString();
                    fields[BaseDropLevel] = 60.ToString();
                    fields[MinDropLevel] = 35.ToString();
                    fields[Aoe] = 0.ToString();
                    fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.75.ToString(CultureInfo.InvariantCulture);
                    break;
                case 50: // steel falchion
                    fields[MinDamage] = 50.ToString();
                    fields[MaxDamage] = 80.ToString();
                    fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 1.ToString();
                    fields[Precision] = 0.ToString();
                    fields[Defense] = 0.ToString();
                    fields[Type] = 0.ToString();
                    fields[BaseDropLevel] = 80.ToString();
                    fields[MinDropLevel] = 55.ToString();
                    fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                    break;

                // UNIQUE STABBING SWORDS
                case 43: // pirate's sword (fishing chest)
                    fields[MinDamage] = 40.ToString();
                    fields[MaxDamage] = 55.ToString();
                    fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 0.ToString();
                    fields[Precision] = 0.ToString();
                    fields[Defense] = 0.ToString();
                    fields[Type] = 0.ToString();
                    fields[BaseDropLevel] = (-1).ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                    break;
                case 15: // forest sword (scavenger hunt)
                    fields[MinDamage] = 42.ToString();
                    fields[MaxDamage] = 68.ToString();
                    fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 1.ToString();
                    fields[Precision] = 1.ToString();
                    fields[Defense] = 0.ToString();
                    fields[Type] = 0.ToString();
                    fields[BaseDropLevel] = (-1).ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[Aoe] = 1.ToString();
                    fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                    break;
                case 5: // bone sword (prospector hunt)
                    fields[MinDamage] = 40.ToString();
                    fields[MaxDamage] = 65.ToString();
                    fields[Knockback] = 0.6.ToString();
                    fields[Speed] = 2.ToString();
                    fields[Precision] = 0.ToString();
                    fields[Defense] = 0.ToString();
                    fields[Type] = 0.ToString();
                    fields[BaseDropLevel] = (-1).ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                    break;

                // BIS STABBING SWORDS
                case 4: // galaxy sword
                    fields[MinDamage] = 80.ToString();
                    fields[MaxDamage] = 120.ToString();
                    fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 0.ToString();
                    fields[Precision] = 0.ToString();
                    fields[Defense] = 0.ToString();
                    fields[Type] = 0.ToString();
                    fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                    break;
                case 57: // dragontooth cutlass
                    fields[MinDamage] = 165.ToString();
                    fields[MaxDamage] = 200.ToString();
                    fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 0.ToString();
                    fields[Precision] = 1.ToString();
                    fields[Defense] = (-1).ToString();
                    fields[Type] = 0.ToString();
                    fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 2.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 62: // infinity blade
                    fields[MinDamage] = 140.ToString();
                    fields[MaxDamage] = 180.ToString();
                    fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 0.ToString();
                    fields[Precision] = 1.ToString();
                    fields[Defense] = 0.ToString();
                    fields[Type] = 0.ToString();
                    fields[Aoe] = 1.ToString();
                    fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                    break;

                #endregion stabbing swords

                #region daggers

                // BASIC DAGGERS
                case 16: // carving knife
                    fields[MinDamage] = 1.ToString();
                    fields[MaxDamage] = 3.ToString();
                    fields[Knockback] = 0.4.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 0.ToString();
                    fields[Precision] = 0.ToString();
                    fields[Defense] = 0.ToString();
                    fields[BaseDropLevel] = 1.ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 18: // burglar's shank
                    fields[MinDamage] = 10.ToString();
                    fields[MaxDamage] = 15.ToString();
                    fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 0.ToString();
                    fields[Precision] = 0.ToString();
                    fields[Defense] = 0.ToString();
                    fields[BaseDropLevel] = 20.ToString();
                    fields[MinDropLevel] = 5.ToString();
                    fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 22: // wind spire
                    fields[MinDamage] = 36.ToString();
                    fields[MaxDamage] = 42.ToString();
                    fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 0.ToString();
                    fields[Precision] = 0.ToString();
                    fields[Defense] = 0.ToString();
                    fields[BaseDropLevel] = 55.ToString();
                    fields[MinDropLevel] = 30.ToString();
                    fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 17: // iron dirk
                    fields[MinDamage] = 44.ToString();
                    fields[MaxDamage] = 56.ToString();
                    fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 0.ToString();
                    fields[Precision] = 0.ToString();
                    fields[Defense] = 0.ToString();
                    fields[BaseDropLevel] = 55.ToString();
                    fields[MinDropLevel] = 30.ToString();
                    fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;

                // UNIQUE DAGGERS
                case 21: // crystal dagger (mine chest, level 60)
                    fields[MinDamage] = 32.ToString();
                    fields[MaxDamage] = 40.ToString();
                    fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                    fields[Precision] = 0.ToString();
                    fields[Defense] = 0.ToString();
                    fields[BaseDropLevel] = (-1).ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[Aoe] = 1.ToString();
                    fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 19: // shadow dagger (mine chest, level 90)
                    fields[MinDamage] = 40.ToString();
                    fields[MaxDamage] = 52.ToString();
                    fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                    fields[Precision] = 0.ToString();
                    fields[Defense] = (-1).ToString();
                    fields[Speed] = 0.ToString();
                    fields[BaseDropLevel] = (-1).ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[Aoe] = 1.ToString();
                    fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 51: // broken trident (fishing chest)
                    fields[MinDamage] = 40.ToString();
                    fields[MaxDamage] = 45.ToString();
                    fields[Knockback] = 0.4.ToString(CultureInfo.InvariantCulture);
                    fields[Precision] = 1.ToString();
                    fields[Defense] = (-1).ToString();
                    fields[BaseDropLevel] = (-1).ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.4.ToString(CultureInfo.InvariantCulture);
                    break;
                case 20: // elf blade (scavenger hunt)
                    fields[MinDamage] = 34.ToString();
                    fields[MaxDamage] = 42.ToString();
                    fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 1.ToString();
                    fields[Precision] = 1.ToString();
                    fields[Defense] = 0.ToString();
                    fields[BaseDropLevel] = (-1).ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 13: // insect head (quest)
                    fields[MinDamage] = 1.ToString();
                    fields[MaxDamage] = 50.ToString();
                    fields[Knockback] = 0.4.ToString(CultureInfo.InvariantCulture);
                    fields[Precision] = (-1).ToString();
                    fields[Defense] = (-2).ToString();
                    fields[Type] = 1.ToString();
                    fields[BaseDropLevel] = (-1).ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[CritChance] = 0.13.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.3.ToString(CultureInfo.InvariantCulture);
                    break;
                case 61: // iridium needle (quest or drop)
                    fields[MinDamage] = 1.ToString();
                    fields[MaxDamage] = 100.ToString();
                    fields[Knockback] = 0.3.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 1.ToString();
                    fields[Precision] = 2.ToString();
                    fields[Defense] = (-2).ToString();
                    fields[CritChance] = 0.2.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 4.0.ToString(CultureInfo.InvariantCulture);
                    break;

                // BIS DAGGERS
                case 23: // galaxy dagger
                    fields[MinDamage] = 60.ToString();
                    fields[MaxDamage] = 75.ToString();
                    fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 0.ToString();
                    fields[Precision] = 1.ToString();
                    fields[Defense] = 0.ToString();
                    fields[Aoe] = 1.ToString();
                    fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 56: // dwarf dagger
                    fields[MinDamage] = 105.ToString();
                    fields[MaxDamage] = 115.ToString();
                    fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = (-1).ToString();
                    fields[Precision] = 0.ToString();
                    fields[Defense] = 2.ToString();
                    fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 59: // dragontooth shiv
                    fields[MinDamage] = 110.ToString();
                    fields[MaxDamage] = 135.ToString();
                    fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = (-1).ToString();
                    fields[Precision] = 0.ToString();
                    fields[Defense] = (-1).ToString();
                    fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.8.ToString(CultureInfo.InvariantCulture);
                    break;
                case 64: // infinity dagger
                    fields[MinDamage] = 100.ToString();
                    fields[MaxDamage] = 120.ToString();
                    fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = (-1).ToString();
                    fields[Precision] = 1.ToString();
                    fields[Defense] = 0.ToString();
                    fields[Aoe] = 1.ToString();
                    fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;

                #endregion daggers

                #region clubs

                // BASIC CLUBS
                case 24: // wood club
                    fields[MinDamage] = 4.ToString();
                    fields[MaxDamage] = 12.ToString();
                    fields[Knockback] = 1.0.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 0.ToString();
                    fields[Precision] = 0.ToString();
                    fields[Defense] = 0.ToString();
                    fields[BaseDropLevel] = 5.ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[CritChance] = 0.03125.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                    break;
                case 27: // wood mallet
                    fields[MinDamage] = 10.ToString();
                    fields[MaxDamage] = 35.ToString();
                    fields[Knockback] = 1.0.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = (-1).ToString();
                    fields[Precision] = 0.ToString();
                    fields[Defense] = 0.ToString();
                    fields[BaseDropLevel] = 25.ToString();
                    fields[MinDropLevel] = 10.ToString();
                    fields[CritChance] = 0.03125.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 3.0.ToString(CultureInfo.InvariantCulture);
                    break;
                case 26: // lead rod
                    fields[MinDamage] = 25.ToString();
                    fields[MaxDamage] = 70.ToString();
                    fields[Knockback] = 1.1.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = (-3).ToString();
                    fields[Precision] = 0.ToString();
                    fields[Defense] = 0.ToString();
                    fields[BaseDropLevel] = 80.ToString();
                    fields[MinDropLevel] = 40.ToString();
                    fields[CritChance] = 0.03125.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 3.0.ToString(CultureInfo.InvariantCulture);
                    break;
                case 46: // kudgel
                    fields[MinDamage] = 30.ToString();
                    fields[MaxDamage] = 100.ToString();
                    fields[Knockback] = 1.8.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = (-2).ToString();
                    fields[Precision] = 0.ToString();
                    fields[Defense] = 0.ToString();
                    fields[BaseDropLevel] = 100.ToString();
                    fields[MinDropLevel] = 50.ToString();
                    fields[CritChance] = 0.03125.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 3.0.ToString(CultureInfo.InvariantCulture);
                    break;
                case 28: // the slammer
                    fields[MinDamage] = 50.ToString();
                    fields[MaxDamage] = 125.ToString();
                    fields[Knockback] = 1.2.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = (-4).ToString();
                    fields[Defense] = 1.ToString();
                    fields[BaseDropLevel] = 125.ToString();
                    fields[MinDropLevel] = 100.ToString();
                    fields[CritChance] = 0.03125.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 3.2.ToString(CultureInfo.InvariantCulture);
                    break;

                // UNIQUE CLUBS
                case 31: // femur (prospector hunt)
                    fields[MinDamage] = 20.ToString();
                    fields[MaxDamage] = 50.ToString();
                    fields[Knockback] = 1.1.ToString(CultureInfo.InvariantCulture);
                    fields[Precision] = (-1).ToString();
                    fields[Speed] = (-3).ToString();
                    fields[Defense] = 1.ToString();
                    fields[BaseDropLevel] = (-1).ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[CritChance] = 0.03125.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 3.0.ToString(CultureInfo.InvariantCulture);
                    break;

                // BIS CLUBS
                case 29: // galaxy hammer
                    fields[MinDamage] = 60.ToString();
                    fields[MaxDamage] = 200.ToString();
                    fields[Knockback] = 1.0.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 0.ToString();
                    fields[Precision] = 1.ToString();
                    fields[Defense] = 0.ToString();
                    fields[Aoe] = 1.ToString();
                    fields[CritChance] = 0.03125.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 3.0.ToString(CultureInfo.InvariantCulture);
                    break;
                case 55: // dwarf hammer
                    fields[MinDamage] = 140.ToString();
                    fields[MaxDamage] = 280.ToString();
                    fields[Knockback] = 1.0.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = (-2).ToString();
                    fields[Precision] = 1.ToString();
                    fields[Defense] = 2.ToString();
                    fields[BaseDropLevel] = (-1).ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[Aoe] = 0.ToString();
                    fields[CritChance] = 0.03125.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 3.0.ToString(CultureInfo.InvariantCulture);
                    break;
                case 58: // dragontooth club
                    fields[MinDamage] = 120.ToString();
                    fields[MaxDamage] = 360.ToString();
                    fields[Knockback] = 1.0.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = (-1).ToString();
                    fields[Precision] = 0.ToString();
                    fields[Defense] = 0.ToString();
                    fields[Aoe] = 0.ToString();
                    fields[CritChance] = 0.03125.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 4.0.ToString(CultureInfo.InvariantCulture);
                    break;
                case 63: // infinity gavel
                    fields[MinDamage] = 100.ToString();
                    fields[MaxDamage] = 300.ToString();
                    fields[Knockback] = 1.0.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 0.ToString();
                    fields[Precision] = 1.ToString();
                    fields[Defense] = 0.ToString();
                    fields[Aoe] = 1.ToString();
                    fields[CritChance] = 0.03125.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 3.0.ToString(CultureInfo.InvariantCulture);
                    break;

                #endregion clubs

                #region defense swords

                // BASIC DEFENSE SWORDS
                case 0: // rusty sword (removed)
                    fields[MinDamage] = 1.ToString();
                    fields[MaxDamage] = 3.ToString();
                    fields[Knockback] = 0.75.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = (-1).ToString();
                    fields[Precision] = 0.ToString();
                    fields[Defense] = 0.ToString();
                    fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 1: // silver saber
                    fields[MinDamage] = 6.ToString();
                    fields[MaxDamage] = 12.ToString();
                    fields[Knockback] = 0.8.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = (-1).ToString();
                    fields[Precision] = 0.ToString();
                    fields[Defense] = 1.ToString();
                    fields[Type] = 3.ToString();
                    fields[BaseDropLevel] = 10.ToString();
                    fields[MinDropLevel] = 1.ToString();
                    fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                    break;
                case 6: // iron edge
                    fields[MinDamage] = 14.ToString();
                    fields[MaxDamage] = 21.ToString();
                    fields[Knockback] = 0.8.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = (-2).ToString();
                    fields[Defense] = 1.ToString();
                    fields[BaseDropLevel] = 20.ToString();
                    fields[MinDropLevel] = 5.ToString();
                    fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                    break;
                case 10: // claymore
                    fields[MinDamage] = 32.ToString();
                    fields[MaxDamage] = 48.ToString();
                    fields[Knockback] = 0.9.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = (-3).ToString();
                    fields[Defense] = 3.ToString();
                    fields[BaseDropLevel] = 60.ToString();
                    fields[MinDropLevel] = 35.ToString();
                    fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                    break;
                case 52: // tempered broadsword
                    fields[MinDamage] = 44.ToString();
                    fields[MaxDamage] = 62.ToString();
                    fields[Knockback] = 0.85.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = (-2).ToString();
                    fields[Defense] = 2.ToString();
                    fields[BaseDropLevel] = 80.ToString();
                    fields[MinDropLevel] = 50.ToString();
                    fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                    break;

                // UNIQUE DEFENSE SWORDS
                case 48: // yeti's tooth (mine chest, level 70)
                    fields[MinDamage] = 30.ToString();
                    fields[MaxDamage] = 45.ToString();
                    fields[Knockback] = 0.8.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = (-2).ToString();
                    fields[Defense] = 1.ToString();
                    fields[BaseDropLevel] = (-1).ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[Aoe] = 1.ToString();
                    fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                    break;

                case 14: // neptune's glaive (fishing chest)
                    fields[MinDamage] = 50.ToString();
                    fields[MaxDamage] = 85.ToString();
                    fields[Knockback] = 1.0.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = (-2).ToString();
                    fields[Defense] = 2.ToString();
                    fields[Precision] = 1.ToString();
                    fields[BaseDropLevel] = (-1).ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[Aoe] = 1.ToString();
                    fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                    break;
                case 60: // ossified blade (prospector hunt)
                    fields[MinDamage] = 45.ToString();
                    fields[MaxDamage] = 75.ToString();
                    fields[Speed] = (-1).ToString();
                    fields[Defense] = 1.ToString();
                    fields[Precision] = 0.ToString();
                    fields[BaseDropLevel] = (-1).ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                    break;

                // BIS DEFENSE SWORDS
                case 54: // dwarf sword
                    fields[MinDamage] = 145.ToString();
                    fields[MaxDamage] = 165.ToString();
                    fields[Knockback] = 0.85.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = (-2).ToString();
                    fields[Precision] = 1.ToString();
                    fields[Defense] = 3.ToString();
                    fields[Aoe] = 1.ToString();
                    fields[CritChance] = 0.0625.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                    break;

                #endregion defense swords

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
                    fields[CritChance] = 0.03125.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                    break;
                case 41: // seb (club)
                    fields[Knockback] = 1.0.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 0.ToString();
                    fields[Precision] = 0.ToString();
                    fields[CritChance] = 0.03125.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 3.0.ToString(CultureInfo.InvariantCulture);
                    break;

                #endregion bachelor(ette) weapons

                #endregion weapon switch-case
            }

            data[key] = string.Join('/', fields);
        }
    }

    #endregion editor callbacks
}
