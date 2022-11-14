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
        //AssetEditors["Data/weapons"] = (callback: EditWeaponsData, priority: AssetEditPriority.Late);
        AssetEditors["TileSheets/BuffsIcons"] =
            (Callback: EditBuffsIconsTileSheet, Priority: AssetEditPriority.Default);
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

                // STABBING SWORDS
                case 12: // wooden blade
                    fields[MinDamage] = 2.ToString();
                    fields[MaxDamage] = 5.ToString();
                    fields[Knockback] = 1.ToString();
                    fields[Speed] = 2.ToString();
                    fields[Precision] = 0.ToString();
                    fields[Defense] = 0.ToString();
                    fields[Type] = 0.ToString();
                    fields[BaseDropLevel] = (-1).ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[Aoe] = 0.ToString();
                    fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.2.ToString(CultureInfo.InvariantCulture);
                    break;
                case 11: // steel smallsword
                    fields[MinDamage] = 4.ToString();
                    fields[MaxDamage] = 8.ToString();
                    fields[Knockback] = 1.ToString();
                    fields[Speed] = 0.ToString();
                    fields[Precision] = 0.ToString();
                    fields[Defense] = 0.ToString();
                    fields[Type] = 0.ToString();
                    fields[BaseDropLevel] = 5.ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 44: // cutlass
                    fields[MinDamage] = 9.ToString();
                    fields[MaxDamage] = 17.ToString();
                    fields[Knockback] = 1.ToString();
                    fields[Speed] = 0.ToString();
                    fields[Precision] = 0.ToString();
                    fields[Defense] = 0.ToString();
                    fields[Type] = 0.ToString();
                    fields[BaseDropLevel] = 20.ToString();
                    fields[MinDropLevel] = 5.ToString();
                    fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 49: // rapier
                    fields[MinDamage] = 15.ToString();
                    fields[MaxDamage] = 25.ToString();
                    fields[Knockback] = 0.8.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 8.ToString();
                    fields[Precision] = 8.ToString();
                    fields[Defense] = 0.ToString();
                    fields[Type] = 0.ToString();
                    fields[BaseDropLevel] = 60.ToString();
                    fields[MinDropLevel] = 35.ToString();
                    fields[Aoe] = 0.ToString();
                    fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.35.ToString(CultureInfo.InvariantCulture);
                    break;
                case 50: // steel falchion
                    fields[MinDamage] = 30.ToString();
                    fields[MaxDamage] = 45.ToString();
                    fields[Knockback] = 1.ToString();
                    fields[Speed] = 4.ToString();
                    fields[Precision] = 0.ToString();
                    fields[Defense] = 0.ToString();
                    fields[Type] = 0.ToString();
                    fields[BaseDropLevel] = 80.ToString();
                    fields[MinDropLevel] = 55.ToString();
                    fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.75.ToString(CultureInfo.InvariantCulture);
                    break;
                case 43: // pirate's sword
                    fields[MinDamage] = 37.ToString();
                    fields[MaxDamage] = 55.ToString();
                    fields[Knockback] = 1.ToString();
                    fields[Speed] = 4.ToString();
                    fields[Defense] = 0.ToString();
                    fields[Type] = 0.ToString();
                    fields[BaseDropLevel] = (-1).ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 15: // forest sword
                    fields[MinDamage] = 42.ToString();
                    fields[MaxDamage] = 60.ToString();
                    fields[Knockback] = 1.ToString();
                    fields[Speed] = 2.ToString();
                    fields[Precision] = 2.ToString();
                    fields[Defense] = 0.ToString();
                    fields[Type] = 0.ToString();
                    fields[BaseDropLevel] = (-1).ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[Aoe] = 2.ToString();
                    fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 5: // bone sword
                    fields[MinDamage] = 35.ToString();
                    fields[MaxDamage] = 53.ToString();
                    fields[Knockback] = 1.ToString();
                    fields[Speed] = 4.ToString();
                    fields[Precision] = 0.ToString();
                    fields[Defense] = 0.ToString();
                    fields[Type] = 0.ToString();
                    fields[BaseDropLevel] = (-1).ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.4.ToString(CultureInfo.InvariantCulture);
                    break;
                case 13: // insect head
                    fields[MinDamage] = 1.ToString();
                    fields[MaxDamage] = 50.ToString();
                    fields[Knockback] = 0.6.ToString(CultureInfo.InvariantCulture);
                    fields[Precision] = 12.ToString();
                    fields[Defense] = 0.ToString();
                    fields[Type] = 0.ToString();
                    fields[BaseDropLevel] = (-1).ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[CritChance] = 0.15.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.333.ToString(CultureInfo.InvariantCulture);
                    break;
                case 2: // dark sword
                    fields[Description] = ModEntry.i18n.Get("darkblade.desc");
                    fields[MinDamage] = 50.ToString();
                    fields[MaxDamage] = 65.ToString();
                    fields[Knockback] = 1.ToString();
                    fields[Speed] = 0.ToString();
                    fields[Defense] = 0.ToString();
                    fields[Type] = 0.ToString();
                    fields[BaseDropLevel] = (-1).ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 8: // obsidian edge
                    fields[MinDamage] = 56.ToString();
                    fields[MaxDamage] = 64.ToString();
                    fields[Knockback] = 1.ToString();
                    fields[Speed] = 0.ToString();
                    fields[Precision] = 4.ToString();
                    fields[Defense] = 0.ToString();
                    fields[Type] = 0.ToString();
                    fields[BaseDropLevel] = 135.ToString();
                    fields[MinDropLevel] = 100.ToString();
                    fields[CritChance] = 0.08.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 9: // lava katana
                    fields[MinDamage] = 75.ToString();
                    fields[MaxDamage] = 90.ToString();
                    fields[Knockback] = 0.8.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 2.ToString();
                    fields[Precision] = 2.ToString();
                    fields[Defense] = 0.ToString();
                    fields[Type] = 0.ToString();
                    fields[CritChance] = 0.06.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.8.ToString(CultureInfo.InvariantCulture);
                    break;
                case 4: // galaxy sword
                    fields[MinDamage] = 80.ToString();
                    fields[MaxDamage] = 95.ToString();
                    fields[Knockback] = 1.ToString();
                    fields[Speed] = 2.ToString();
                    fields[Precision] = 0.ToString();
                    fields[Defense] = 2.ToString();
                    fields[Type] = 0.ToString();
                    fields[Aoe] = 2.ToString();
                    fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 57: // dragontooth cutlass
                    fields[MinDamage] = 145.ToString();
                    fields[MaxDamage] = 175.ToString();
                    fields[Knockback] = 1.ToString();
                    fields[Speed] = 0.ToString();
                    fields[Precision] = 2.ToString();
                    fields[Defense] = 0.ToString();
                    fields[Type] = 0.ToString();
                    fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 2.ToString();
                    break;
                case 62: // infinity blade
                    fields[MinDamage] = 140.ToString();
                    fields[MaxDamage] = 160.ToString();
                    fields[Knockback] = 1.ToString();
                    fields[Speed] = 2.ToString();
                    fields[Precision] = 2.ToString();
                    fields[Defense] = 0.ToString();
                    fields[Type] = 0.ToString();
                    fields[Aoe] = 2.ToString();
                    fields[CritChance] = 0.05.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;

                // DAGGERS
                case 16: // carving knife
                    fields[BaseDropLevel] = 1.ToString();
                    fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                    break;
                case 22: // wind spire
                    fields[MinDamage] = 2.ToString();
                    fields[Knockback] = 0.3.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 1.ToString();
                    fields[CritChance] = 0.11.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 3.2.ToString(CultureInfo.InvariantCulture);
                    break;
                case 17: // iron dirk
                    fields[MinDamage] = 5.ToString();
                    fields[MaxDamage] = 8.ToString();
                    fields[BaseDropLevel] = 25.ToString();
                    fields[MinDropLevel] = 10.ToString();
                    fields[CritChance] = 0.08.ToString(CultureInfo.InvariantCulture);
                    break;
                case 18: // burglar's shank
                    fields[Knockback] = 0.3.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 1.ToString();
                    fields[Precision] = 0.ToString();
                    fields[BaseDropLevel] = 45.ToString();
                    fields[MinDropLevel] = 20.ToString();
                    fields[CritChance] = 0.08.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 4.ToString();
                    break;
                case 21: // crystal dagger
                    fields[MinDamage] = 16.ToString();
                    fields[MaxDamage] = 20.ToString();
                    fields[Knockback] = 0.6.ToString(CultureInfo.InvariantCulture);
                    fields[Precision] = 0.ToString();
                    fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 3.ToString();
                    break;
                case 20: // elf blade
                    fields[MinDamage] = 13.ToString();
                    fields[MaxDamage] = 18.ToString();
                    fields[Knockback] = 0.6.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 2.ToString();
                    fields[Precision] = 2.ToString();
                    fields[CritChance] = 0.12.ToString(CultureInfo.InvariantCulture);
                    break;
                case 19: // shadow blade
                    fields[MinDamage] = 24.ToString();
                    fields[MaxDamage] = 28.ToString();
                    fields[Knockback] = 0.4.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 1.ToString();
                    fields[BaseDropLevel] = (-1).ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[Aoe] = 2.ToString();
                    fields[CritChance] = 0.08.ToString(CultureInfo.InvariantCulture);
                    break;
                case 51: // broken trident
                    fields[MinDamage] = 16.ToString();
                    fields[MaxDamage] = 22.ToString();
                    fields[Knockback] = 0.6.ToString(CultureInfo.InvariantCulture);
                    fields[Precision] = 2.ToString();
                    fields[CritChance] = 0.06.ToString(CultureInfo.InvariantCulture);
                    break;
                case 45: // wicked kriss
                    fields[MinDamage] = 28.ToString();
                    fields[MaxDamage] = 32.ToString();
                    fields[Knockback] = 0.4.ToString(CultureInfo.InvariantCulture);
                    fields[Precision] = 6.ToString();
                    fields[BaseDropLevel] = 110.ToString();
                    fields[MinDropLevel] = 85.ToString();
                    fields[Aoe] = 0.ToString();
                    fields[CritChance] = 0.2.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 4.ToString();
                    break;
                case 61: // iridium needle
                    fields[MinDamage] = 31.ToString();
                    fields[MaxDamage] = 33.ToString();
                    fields[Knockback] = 0.3.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 4.ToString();
                    fields[Precision] = 10.ToString();
                    fields[Defense] = (-2).ToString();
                    fields[CritChance] = 0.333.ToString(CultureInfo.InvariantCulture);
                    break;
                case 23: // galaxy dagger
                    fields[MinDamage] = 40.ToString();
                    fields[MaxDamage] = 45.ToString();
                    fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 3.ToString();
                    fields[Aoe] = 1.ToString();
                    fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 4.ToString();
                    break;
                case 56: // dwarf dagger
                    fields[MinDamage] = 60.ToString();
                    fields[MaxDamage] = 65.ToString();
                    fields[Speed] = (-2).ToString();
                    fields[CritChance] = 0.08.ToString(CultureInfo.InvariantCulture);
                    break;
                case 59: // dragontooth shiv
                    fields[MinDamage] = 78.ToString();
                    fields[MaxDamage] = 83.ToString();
                    fields[Knockback] = 0.8.ToString(CultureInfo.InvariantCulture);
                    fields[CritChance] = 0.1.ToString(CultureInfo.InvariantCulture);
                    break;
                case 64: // infinity dagger
                    fields[MinDamage] = 75.ToString();
                    fields[MaxDamage] = 80.ToString();
                    fields[Knockback] = 0.6.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 3.ToString();
                    fields[Aoe] = 1.ToString();
                    fields[CritChance] = 0.12.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 4.ToString();
                    break;

                // CLUBS
                case 31: // femur
                    fields[MinDamage] = 10.ToString();
                    fields[MaxDamage] = 40.ToString();
                    fields[Knockback] = 1.6.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = (-16).ToString();
                    fields[Defense] = 6.ToString();
                    fields[BaseDropLevel] = (-1).ToString();
                    fields[CritPower] = 2.ToString();
                    break;
                case 24: // wood club
                    fields[MinDamage] = 3.ToString();
                    fields[MaxDamage] = 12.ToString();
                    fields[Knockback] = 1.4.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = (-14).ToString();
                    fields[Defense] = 4.ToString();
                    fields[BaseDropLevel] = 3.ToString();
                    fields[CritPower] = 1.8.ToString(CultureInfo.InvariantCulture);
                    break;
                case 27: // wood mallet
                    fields[MinDamage] = 5.ToString();
                    fields[MaxDamage] = 26.ToString();
                    fields[Speed] = (-10).ToString();
                    fields[Precision] = 0.ToString();
                    fields[Defense] = 3.ToString();
                    fields[BaseDropLevel] = 25.ToString();
                    fields[MinDropLevel] = 10.ToString();
                    fields[CritPower] = 1.8.ToString(CultureInfo.InvariantCulture);
                    break;
                case 26: // lead rod
                    fields[MinDamage] = 20.ToString();
                    fields[MaxDamage] = 60.ToString();
                    fields[Knockback] = 2.2.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = (-28).ToString();
                    fields[Defense] = 14.ToString();
                    fields[BaseDropLevel] = 65.ToString();
                    fields[MinDropLevel] = 40.ToString();
                    fields[CritPower] = 2.ToString();
                    break;
                case 46: // kudgel
                    fields[MinDamage] = 30.ToString();
                    fields[MaxDamage] = 70.ToString();
                    fields[Knockback] = 1.8.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = (-24).ToString();
                    fields[Defense] = 8.ToString();
                    fields[BaseDropLevel] = 80.ToString();
                    fields[MinDropLevel] = 60.ToString();
                    fields[CritPower] = 2.2.ToString(CultureInfo.InvariantCulture);
                    break;
                case 28: // the slammer
                    fields[MinDamage] = 40.ToString();
                    fields[MaxDamage] = 105.ToString();
                    fields[Knockback] = 2.4.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = (-32).ToString();
                    fields[Defense] = 12.ToString();
                    fields[BaseDropLevel] = (-1).ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[CritPower] = 2.ToString();
                    break;
                case 29: // galaxy hammer
                    fields[MinDamage] = 60.ToString();
                    fields[MaxDamage] = 120.ToString();
                    fields[Knockback] = 2.ToString();
                    fields[Speed] = (-14).ToString();
                    fields[Defense] = 10.ToString();
                    fields[Aoe] = 3.ToString();
                    fields[CritPower] = 2.ToString();
                    break;
                case 55: // dwarf hammer
                    fields[MinDamage] = 140.ToString();
                    fields[MaxDamage] = 180.ToString();
                    fields[Knockback] = 2.2.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = (-24).ToString();
                    fields[Defense] = 20.ToString();
                    fields[CritPower] = 2.ToString();
                    break;
                case 58: // dragontooth club
                    fields[MinDamage] = 100.ToString();
                    fields[MaxDamage] = 215.ToString();
                    fields[Knockback] = 2.ToString();
                    fields[Speed] = (-16).ToString();
                    fields[Defense] = 12.ToString();
                    fields[CritPower] = 3.ToString();
                    break;
                case 63: // infinity gavel
                    fields[MinDamage] = 120.ToString();
                    fields[MaxDamage] = 200.ToString();
                    fields[Knockback] = 2.ToString();
                    fields[Speed] = (-12).ToString();
                    fields[Defense] = 15.ToString();
                    fields[Aoe] = 3.ToString();
                    fields[CritPower] = 2.ToString();
                    break;

                // DEFENSE SWORDS
                case 0: // rusty sword
                    fields[MinDamage] = 3.ToString();
                    fields[MaxDamage] = 7.ToString();
                    fields[Speed] = (-1).ToString();
                    fields[Defense] = 1.ToString();
                    fields[CritChance] = 0.04.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 1: // silver saber
                    fields[MinDamage] = 4.ToString();
                    fields[MaxDamage] = 8.ToString();
                    fields[Speed] = (-1).ToString();
                    fields[Precision] = 0.ToString();
                    fields[Defense] = 2.ToString();
                    fields[Type] = 3.ToString();
                    fields[BaseDropLevel] = 25.ToString();
                    fields[MinDropLevel] = 10.ToString();
                    fields[CritChance] = 0.04.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 6: // iron edge
                    fields[MinDamage] = 12.ToString();
                    fields[MaxDamage] = 25.ToString();
                    fields[Defense] = 2.ToString();
                    fields[BaseDropLevel] = 40.ToString();
                    fields[MinDropLevel] = 15.ToString();
                    fields[CritChance] = 0.04.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 3: // holy blade
                    fields[MinDamage] = 45.ToString();
                    fields[MaxDamage] = 60.ToString();
                    fields[Speed] = (-2).ToString();
                    fields[Precision] = 2.ToString();
                    fields[Defense] = 4.ToString();
                    fields[Aoe] = 2.ToString();
                    fields[CritChance] = 0.04.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 14: // neptune's glaive
                    fields[MinDamage] = 42.ToString();
                    fields[MaxDamage] = 68.ToString();
                    fields[Knockback] = 1.2.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = (-2).ToString();
                    fields[Precision] = 0.ToString();
                    fields[CritChance] = 0.04.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 10: // claymore
                    fields[MinDamage] = 24.ToString();
                    fields[MaxDamage] = 48.ToString();
                    fields[Knockback] = 1.3.ToString(CultureInfo.InvariantCulture);
                    fields[Defense] = 8.ToString();
                    fields[BaseDropLevel] = 75.ToString();
                    fields[MinDropLevel] = 50.ToString();
                    fields[CritChance] = 0.04.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 7: // templar's blade
                    fields[MinDamage] = 44.ToString();
                    fields[MaxDamage] = 62.ToString();
                    fields[Speed] = (-4).ToString();
                    fields[Defense] = 4.ToString();
                    fields[BaseDropLevel] = 100.ToString();
                    fields[MinDropLevel] = 70.ToString();
                    fields[CritChance] = 0.04.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 48: // yeti's tooth
                    fields[MinDamage] = 26.ToString();
                    fields[MaxDamage] = 42.ToString();
                    fields[Knockback] = 1.2.ToString(CultureInfo.InvariantCulture);
                    fields[Type] = 3.ToString();
                    fields[BaseDropLevel] = 60.ToString();
                    fields[MinDropLevel] = 40.ToString();
                    fields[Aoe] = 2.ToString();
                    fields[CritChance] = 0.04.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.6.ToString(CultureInfo.InvariantCulture);
                    break;
                case 60: // ossified blade
                    fields[MinDamage] = 48.ToString();
                    fields[MaxDamage] = 68.ToString();
                    fields[Defense] = 3.ToString();
                    fields[Type] = 3.ToString();
                    fields[BaseDropLevel] = (-1).ToString();
                    fields[MinDropLevel] = (-1).ToString();
                    fields[CritChance] = 0.04.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 52: // tempered broadsword
                    fields[MinDamage] = 60.ToString();
                    fields[MaxDamage] = 80.ToString();
                    fields[Defense] = 6.ToString();
                    fields[BaseDropLevel] = 120.ToString();
                    fields[CritChance] = 0.04.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.55.ToString(CultureInfo.InvariantCulture);
                    break;
                case 54: // dwarf sword
                    fields[MinDamage] = 120.ToString();
                    fields[MaxDamage] = 140.ToString();
                    fields[Speed] = (-4).ToString();
                    fields[Defense] = 8.ToString();
                    fields[Type] = 3.ToString();
                    fields[CritChance] = 0.04.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;

                // BACHELOR(ETTE) WEAPONS
                case 40: // abby
                    fields[Knockback] = 0.4.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 4.ToString();
                    fields[Precision] = 4.ToString();
                    fields[CritChance] = 0.08.ToString(CultureInfo.InvariantCulture);
                    break;
                case 42: // haley
                    fields[Speed] = 0.ToString();
                    fields[CritChance] = 0.04.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 39: // leah
                    fields[Speed] = 0.ToString();
                    fields[CritChance] = 0.08.ToString(CultureInfo.InvariantCulture);
                    break;
                case 36: // maru
                    fields[Speed] = 0.ToString();
                    fields[Defense] = 1.ToString();
                    break;
                case 38: // penny
                    fields[Speed] = 0.ToString();
                    fields[Type] = 3.ToString();
                    fields[CritChance] = 0.04.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 25: // alex
                    fields[Knockback] = 1.4.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = (-4).ToString();
                    break;
                case 35: // eliott
                    fields[Knockback] = 0.3.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 0.ToString();
                    fields[Precision] = 2.ToString();
                    fields[Defense] = (-2).ToString();
                    fields[CritChance] = 0.25.ToString(CultureInfo.InvariantCulture);
                    fields[CritPower] = 2.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 37: // harvey
                    fields[Knockback] = 1.2.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = 0.ToString();
                    fields[Defense] = 1.ToString();
                    fields[CritPower] = 2.ToString();
                    break;
                case 30: // sam
                    fields[Speed] = (-12).ToString();
                    fields[Defense] = 2.ToString();
                    fields[CritPower] = 2.ToString();
                    break;
                case 41: // seb
                    fields[Knockback] = 1.2.ToString(CultureInfo.InvariantCulture);
                    fields[Speed] = (-8).ToString();
                    fields[Defense] = 4.ToString();
                    fields[CritPower] = 2.ToString();
                    break;

                // SCYTHES
                case 47: // regular
                    fields[Aoe] = 6.ToString();
                    fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 53: // golden
                    fields[MinDamage] = 10.ToString();
                    fields[Aoe] = 12.ToString();
                    fields[CritPower] = 2.ToString();
                    break;

                #endregion weapon switch-case
            }

            data[key] = string.Join('/', fields);
        }
    }

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

    #endregion editor callbacks
}
