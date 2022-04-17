namespace DaLion.Stardew.Arsenal.Framework.AssetEditors;

#region using directives

using System;
using System.Globalization;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Edits all game assets required for arsenal rebalancing.</summary>
public class RebalancedArsenalEditor : IAssetEditor
{
    /// <inheritdoc />
    public bool CanEdit<T>(IAssetInfo asset)
    {
        return asset.AssetNameEquals(PathUtilities.NormalizeAssetName("Data/weapons")) ||
               asset.AssetNameEquals(PathUtilities.NormalizeAssetName("Data/Boots"));
    }

    /// <inheritdoc />
    public void Edit<T>(IAssetData asset)
    {
        if (asset.AssetNameEquals(PathUtilities.NormalizeAssetName("Data/weapons")))
        {
            if (!ModEntry.Config.RebalancedWeapons) return;

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
                        fields[2] = 2.ToString();
                        fields[3] = 5.ToString();
                        fields[9] = 1.ToString();
                        fields[12] = 0.04.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 1.4.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 11: // steal smallsword
                        fields[5] = 0.ToString();
                        fields[9] = 5.ToString();
                        fields[12] = 0.04.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 44: // cutlass
                        fields[5] = 0.ToString();
                        fields[9] = 20.ToString();
                        fields[10] = 5.ToString();
                        fields[12] = 0.04.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 49: // rapier
                        fields[5] = 8.ToString();
                        fields[6] = 6.ToString();
                        fields[9] = 60.ToString();
                        fields[10] = 35.ToString();
                        fields[11] = 2.ToString();
                        fields[12] = 0.04.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 1.4.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 50: // steel falchion
                        fields[5] = 4.ToString();
                        fields[6] = 0.ToString();
                        fields[9] = 80.ToString();
                        fields[10] = 55.ToString();
                        fields[12] = 0.04.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 1.8.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 15: // forest sword
                        fields[2] = 23.ToString();
                        fields[3] = 35.ToString();
                        fields[5] = 2.ToString();
                        fields[6] = 2.ToString();
                        fields[7] = 0.ToString();
                        fields[11] = 2.ToString();
                        fields[12] = 0.04.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 43: // pirate's sword
                        fields[2] = 21.ToString();
                        fields[3] = 33.ToString();
                        fields[5] = 0.ToString();
                        fields[8] = 0.ToString();
                        fields[9] = (-1).ToString();
                        fields[12] = 0.04.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 13: // insect head
                        fields[2] = 1.ToString();
                        fields[3] = 50.ToString();
                        fields[4] = 0.8.ToString(CultureInfo.InvariantCulture);
                        fields[6] = 12.ToString();
                        fields[12] = 0.12.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 1.4.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 5: // bone sword
                        fields[5] = 4.ToString();
                        fields[6] = 0.ToString();
                        fields[9] = (-1).ToString();
                        fields[10] = (-1).ToString();
                        fields[12] = 0.04.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 2: // dark sword
                        fields[2] = 50.ToString();
                        fields[3] = 65.ToString();
                        fields[4] = 1.ToString();
                        fields[5] = 0.ToString();
                        fields[13] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 8: // obsidian edge
                        fields[2] = 56.ToString();
                        fields[3] = 64.ToString();
                        fields[5] = 0.ToString();
                        fields[6] = 4.ToString();
                        fields[9] = 135.ToString();
                        fields[10] = 100.ToString();
                        fields[12] = 0.08.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 9: // lava katana
                        fields[2] = 75.ToString();
                        fields[3] = 90.ToString();
                        fields[4] = 0.8.ToString(CultureInfo.InvariantCulture);
                        fields[5] = 2.ToString();
                        fields[6] = 2.ToString();
                        fields[7] = 0.ToString();
                        fields[12] = 0.06.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 1.8.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 4: // galaxy sword
                        fields[2] = 80.ToString();
                        fields[3] = 95.ToString();
                        fields[5] = 2.ToString();
                        fields[7] = 2.ToString();
                        fields[11] = 2.ToString();
                        fields[12] = 0.04.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 57: // dragontooth cutlass
                        fields[2] = 145.ToString();
                        fields[3] = 175.ToString();
                        fields[12] = 0.04.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 2.ToString();
                        break;
                    case 62: // infinity blade
                        fields[2] = 140.ToString();
                        fields[3] = 160.ToString();
                        fields[5] = 2.ToString();
                        fields[11] = 2.ToString();
                        fields[12] = 0.04.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;

                    // DAGGERS
                    case 16: // carving knife
                        fields[9] = 1.ToString();
                        fields[12] = 0.08.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 22: // wind spire
                        fields[2] = 2.ToString();
                        fields[4] = 0.3.ToString(CultureInfo.InvariantCulture);
                        fields[5] = 1.ToString();
                        fields[12] = 0.09.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 3.2.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 17: // iron dirk
                        fields[2] = 5.ToString();
                        fields[3] = 8.ToString();
                        fields[9] = 25.ToString();
                        fields[10] = 10.ToString();
                        fields[12] = 0.08.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 18: // burglar's shank
                        fields[4] = 0.3.ToString(CultureInfo.InvariantCulture);
                        fields[5] = 1.ToString();
                        fields[6] = 0.ToString();
                        fields[9] = 45.ToString();
                        fields[10] = 20.ToString();
                        fields[12] = 0.08.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 4.ToString();
                        break;
                    case 21: // crystal dagger
                        fields[2] = 16.ToString();
                        fields[3] = 20.ToString();
                        fields[4] = 0.6.ToString(CultureInfo.InvariantCulture);
                        fields[6] = 0.ToString();
                        fields[12] = 0.1.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 3.ToString();
                        break;
                    case 20: // elf blade
                        fields[2] = 13.ToString();
                        fields[3] = 18.ToString();
                        fields[4] = 0.6.ToString(CultureInfo.InvariantCulture);
                        fields[5] = 2.ToString();
                        fields[6] = 2.ToString();
                        fields[12] = 0.12.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 19: // shadow blade
                        fields[2] = 24.ToString();
                        fields[3] = 28.ToString();
                        fields[4] = 0.4.ToString(CultureInfo.InvariantCulture);
                        fields[5] = 1.ToString();
                        fields[9] = (-1).ToString();
                        fields[10] = (-1).ToString();
                        fields[11] = 2.ToString();
                        fields[12] = 0.08.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 51: // broken trident
                        fields[2] = 16.ToString();
                        fields[3] = 22.ToString();
                        fields[4] = 0.6.ToString(CultureInfo.InvariantCulture);
                        fields[6] = 2.ToString();
                        fields[12] = 0.06.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 45: // wicked kriss
                        fields[2] = 28.ToString();
                        fields[3] = 32.ToString();
                        fields[4] = 0.4.ToString(CultureInfo.InvariantCulture);
                        fields[6] = 6.ToString();
                        fields[9] = 110.ToString();
                        fields[10] = 85.ToString();
                        fields[11] = 0.ToString();
                        fields[12] = 0.2.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 4.ToString();
                        break;
                    case 61: // iridium needle
                        fields[2] = 31.ToString();
                        fields[3] = 33.ToString();
                        fields[4] = 0.3.ToString(CultureInfo.InvariantCulture);
                        fields[5] = 4.ToString();
                        fields[6] = 10.ToString();
                        fields[7] = (-2).ToString();
                        fields[12] = 0.333.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 23: // galaxy dagger
                        fields[2] = 40.ToString();
                        fields[3] = 45.ToString();
                        fields[4] = 0.5.ToString(CultureInfo.InvariantCulture);
                        fields[5] = 3.ToString();
                        fields[11] = 1.ToString();
                        fields[12] = 0.1.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 4.ToString();
                        break;
                    case 56: // dwarf dagger
                        fields[2] = 60.ToString();
                        fields[3] = 65.ToString();
                        fields[5] = (-2).ToString();
                        fields[12] = 0.08.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 59: // dragontooth shiv
                        fields[2] = 78.ToString();
                        fields[3] = 83.ToString();
                        fields[4] = 0.8.ToString(CultureInfo.InvariantCulture);
                        fields[12] = 0.1.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 64: // infinity dagger
                        fields[2] = 75.ToString();
                        fields[3] = 80.ToString();
                        fields[4] = 0.6.ToString(CultureInfo.InvariantCulture);
                        fields[5] = 3.ToString();
                        fields[11] = 1.ToString();
                        fields[12] = 0.12.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 4.ToString();
                        break;

                    // CLUBS
                    case 31: // femur
                        fields[2] = 10.ToString();
                        fields[3] = 40.ToString();
                        fields[4] = 1.6.ToString(CultureInfo.InvariantCulture);
                        fields[5] = (-16).ToString();
                        fields[7] = 6.ToString();
                        fields[9] = (-1).ToString();
                        fields[13] = 2.ToString();
                        break;
                    case 24: // wood club
                        fields[2] = 3.ToString();
                        fields[3] = 12.ToString();
                        fields[4] = 1.4.ToString(CultureInfo.InvariantCulture);
                        fields[5] = (-14).ToString();
                        fields[7] = 4.ToString();
                        fields[9] = 3.ToString();
                        fields[13] = 1.8.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 27: // wood mallet
                        fields[2] = 5.ToString();
                        fields[3] = 26.ToString();
                        fields[5] = (-10).ToString();
                        fields[6] = 0.ToString();
                        fields[7] = 3.ToString();
                        fields[9] = 25.ToString();
                        fields[10] = 10.ToString();
                        fields[13] = 1.8.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 26: // lead rod
                        fields[2] = 20.ToString();
                        fields[3] = 60.ToString();
                        fields[4] = 2.2.ToString(CultureInfo.InvariantCulture);
                        fields[5] = (-28).ToString();
                        fields[7] = 14.ToString();
                        fields[9] = 65.ToString();
                        fields[10] = 40.ToString();
                        fields[13] = 2.ToString();
                        break;
                    case 46: // kudgel
                        fields[2] = 30.ToString();
                        fields[3] = 70.ToString();
                        fields[4] = 1.8.ToString(CultureInfo.InvariantCulture);
                        fields[5] = (-24).ToString();
                        fields[7] = 8.ToString();
                        fields[9] = 80.ToString();
                        fields[10] = 60.ToString();
                        fields[13] = 2.2.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 28: // the slammer
                        fields[2] = 40.ToString();
                        fields[3] = 105.ToString();
                        fields[4] = 2.4.ToString(CultureInfo.InvariantCulture);
                        fields[5] = (-32).ToString();
                        fields[7] = 12.ToString();
                        fields[9] = (-1).ToString();
                        fields[10] = (-1).ToString();
                        fields[13] = 2.ToString();
                        break;
                    case 29: // galaxy hammer
                        fields[2] = 60.ToString();
                        fields[3] = 120.ToString();
                        fields[4] = 2.ToString();
                        fields[5] = (-14).ToString();
                        fields[7] = 10.ToString();
                        fields[11] = 3.ToString();
                        fields[13] = 2.ToString();
                        break;
                    case 55: // dwarf hammer
                        fields[2] = 140.ToString();
                        fields[3] = 180.ToString();
                        fields[4] = 2.2.ToString(CultureInfo.InvariantCulture);
                        fields[5] = (-24).ToString();
                        fields[7] = 20.ToString();
                        fields[13] = 2.ToString();
                        break;
                    case 58: // dragontooth club
                        fields[2] = 100.ToString();
                        fields[3] = 215.ToString();
                        fields[4] = 2.ToString();
                        fields[5] = (-16).ToString();
                        fields[7] = 12.ToString();
                        fields[13] = 3.ToString();
                        break;
                    case 63: // infinity gavel
                        fields[2] = 120.ToString();
                        fields[3] = 200.ToString();
                        fields[4] = 2.ToString();
                        fields[5] = (-12).ToString();
                        fields[7] = 15.ToString();
                        fields[11] = 3.ToString();
                        fields[13] = 2.ToString();
                        break;

                    // DEFENSE SWORDS
                    case 0: // rusty sword
                        fields[2] = 3.ToString();
                        fields[3] = 7.ToString();
                        fields[5] = (-1).ToString();
                        fields[7] = 1.ToString();
                        fields[12] = 0.04.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 1: // silver smallsword
                        fields[2] = 4.ToString();
                        fields[3] = 8.ToString();
                        fields[5] = (-1).ToString();
                        fields[6] = 0.ToString();
                        fields[7] = 2.ToString();
                        fields[8] = 3.ToString();
                        fields[9] = 25.ToString();
                        fields[10] = 10.ToString();
                        fields[12] = 0.04.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 6: // silver saber
                        fields[2] = 12.ToString();
                        fields[3] = 25.ToString();
                        fields[7] = 2.ToString();
                        fields[9] = 40.ToString();
                        fields[10] = 15.ToString();
                        fields[12] = 0.04.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 3: // holy blade
                        fields[2] = 45.ToString();
                        fields[3] = 60.ToString();
                        fields[5] = (-2).ToString();
                        fields[6] = 2.ToString();
                        fields[7] = 4.ToString();
                        fields[11] = 2.ToString();
                        fields[12] = 0.04.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 14: // neptune's glaive
                        fields[2] = 42.ToString();
                        fields[3] = 68.ToString();
                        fields[4] = 1.2.ToString(CultureInfo.InvariantCulture);
                        fields[5] = (-2).ToString();
                        fields[6] = 0.ToString();
                        fields[12] = 0.04.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 10: // claymore
                        fields[2] = 24.ToString();
                        fields[3] = 48.ToString();
                        fields[4] = 1.3.ToString(CultureInfo.InvariantCulture);
                        fields[7] = 8.ToString();
                        fields[9] = 75.ToString();
                        fields[10] = 50.ToString();
                        fields[12] = 0.04.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 7: // templar's blade
                        fields[2] = 44.ToString();
                        fields[3] = 62.ToString();
                        fields[5] = (-4).ToString();
                        fields[7] = 4.ToString();
                        fields[9] = 100.ToString();
                        fields[10] = 70.ToString();
                        fields[12] = 0.04.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 48: // yeti's tooth
                        fields[2] = 26.ToString();
                        fields[3] = 42.ToString();
                        fields[4] = 1.2.ToString(CultureInfo.InvariantCulture);
                        fields[8] = 3.ToString();
                        fields[9] = 60.ToString();
                        fields[10] = 40.ToString();
                        fields[11] = 2.ToString();
                        fields[12] = 0.04.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 1.6.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 60: // ossified blade
                        fields[2] = 48.ToString();
                        fields[3] = 68.ToString();
                        fields[7] = 3.ToString();
                        fields[8] = 3.ToString();
                        fields[9] = (-1).ToString();
                        fields[10] = (-1).ToString();
                        fields[12] = 0.04.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 52: // tempered broadsword
                        fields[2] = 60.ToString();
                        fields[3] = 80.ToString();
                        fields[7] = 6.ToString();
                        fields[9] = 120.ToString();
                        fields[12] = 0.04.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 1.55.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 54: // dwarf sword
                        fields[2] = 120.ToString();
                        fields[3] = 140.ToString();
                        fields[5] = (-4).ToString();
                        fields[7] = 8.ToString();
                        fields[8] = 3.ToString();
                        fields[12] = 0.04.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;

                    // BACHELOR(ETTE) WEAPONS
                    case 40: // abby
                        fields[4] = 0.4.ToString(CultureInfo.InvariantCulture);
                        fields[5] = 4.ToString();
                        fields[6] = 4.ToString();
                        fields[12] = 0.08.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 42: // haley
                        fields[5] = 0.ToString();
                        fields[12] = 0.04.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 39: // leah
                        fields[5] = 0.ToString();
                        fields[12] = 0.08.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 36: // maru
                        fields[5] = 0.ToString();
                        fields[7] = 1.ToString();
                        break;
                    case 38: // penny
                        fields[5] = 0.ToString();
                        fields[8] = 3.ToString();
                        fields[12] = 0.04.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 25: // alex
                        fields[4] = 1.4.ToString(CultureInfo.InvariantCulture);
                        fields[5] = (-4).ToString();
                        break;
                    case 35: // eliott
                        fields[4] = 0.3.ToString(CultureInfo.InvariantCulture);
                        fields[5] = 0.ToString();
                        fields[6] = 2.ToString();
                        fields[7] = (-2).ToString();
                        fields[12] = 0.25.ToString(CultureInfo.InvariantCulture);
                        fields[13] = 2.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 37: // harvey
                        fields[4] = 1.2.ToString(CultureInfo.InvariantCulture);
                        fields[5] = 0.ToString();
                        fields[7] = 1.ToString();
                        fields[13] = 2.ToString();
                        break;
                    case 30: // sam
                        fields[5] = (-12).ToString();
                        fields[7] = 2.ToString();
                        fields[13] = 2.ToString();
                        break;
                    case 41: // seb
                        fields[4] = 1.2.ToString(CultureInfo.InvariantCulture);
                        fields[5] = (-8).ToString();
                        fields[7] = 4.ToString();
                        fields[13] = 2.ToString();
                        break;

                    // SLINGSHOTS
                    case 32: // regular
                        fields[12] = 0.ToString();
                        fields[13] = 1.ToString();
                        break;
                    case 33: // master
                        fields[12] = 0.ToString();
                        fields[13] = 1.ToString();
                        break;
                    case 34: // galaxy
                        fields[12] = 0.ToString();
                        fields[13] = 1.ToString();
                        break;

                    // SCYTHES
                    case 47: // regular
                        fields[11] = 6.ToString();
                        fields[13] = 1.5.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 53: // golden
                        fields[2] = 10.ToString();
                        fields[11] = 12.ToString();
                        fields[13] = 2.ToString();
                        break;
                
                    #endregion weapon switch-case
                }

                data[key] = string.Join('/', fields);
            }           
        }
        else if (asset.AssetNameEquals(PathUtilities.NormalizeAssetName("Data/Boots")))
        {
            if (!ModEntry.Config.RebalancedWeapons) return;

            var data = asset.AsDictionary<int, string>().Data;
            var keys = data.Keys;
            foreach (var key in keys)
            {
                var fields = data[key].Split('/');
                switch (key)
                {
                    #region footwear switch-case



                    #endregion footwear switch-case
                }

                data[key] = string.Join('/', fields);
            }
        }
        else
        {
            throw new InvalidOperationException($"Unexpected asset {asset.AssetName}.");
        }
    }
}