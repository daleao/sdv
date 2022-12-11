namespace DaLion.Ligo.Modules.Arsenal;

#region using directives

using System.Collections.Generic;
using Ardalis.SmartEnum;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

/// <summary>The tier of a <see cref="MeleeWeapon"/>.</summary>
public sealed class WeaponTier : SmartEnum<WeaponTier>
{
    #region enum values

    /// <summary>The lowest tier, for training weapons or weapons that can be obtained early in the Mines.</summary>
    public static readonly WeaponTier Common = new("Common", 0);

    /// <summary>A mid tier, for weapons that can be found in mid-levels of the Mines or by other activities.</summary>
    public static readonly WeaponTier Uncommon = new("Uncommon", 1);

    /// <summary>A higher tier, for weapons that can be found at the higher levels of the Mines or more rarely by other activities.</summary>
    public static readonly WeaponTier Rare = new("Rare", 2);

    /// <summary>The highest tier, for weapons that can be found beyond the Skull Caverns.</summary>
    public static readonly WeaponTier Epic = new("Epic", 3);

    /// <summary>A special tier, reserved for one-of-a-kind weapons.</summary>
    public static readonly WeaponTier Mythic = new("Mythic", 4);

    /// <summary>A special tier, reserved for crafted weapons.</summary>
    public static readonly WeaponTier Masterwork = new("Masterwork", 5);

    /// <summary>A special tier, reserved for legendary weapons.</summary>
    public static readonly WeaponTier Legendary = new("Legendary", 6);

    /// <summary>Placeholder for weapons that have not been tiered.</summary>
    public static readonly WeaponTier Untiered = new("Untiered", -1);

    #endregion enum values

    private static readonly IReadOnlyDictionary<int, WeaponTier> TierByWeapon;

    static WeaponTier()
    {
#pragma warning disable SA1509 // Opening braces should not be preceded by blank line
        TierByWeapon = new Dictionary<int, WeaponTier>
        {
            { Constants.WoodenBladeIndex, Untiered },

            { Constants.SteelSmallswordIndex, Common },
            { Constants.SilverSaberIndex, Common },
            { Constants.CarvingKnife, Common },
            { Constants.WoodClubIndex, Common },

            { Constants.CutlassIndex, Uncommon },
            { Constants.IronEdgeIndex, Uncommon },
            { Constants.BurglarsShankIndex, Uncommon },
            { Constants.WoodMalletIndex, Uncommon },

            { Constants.RapierIndex, Rare },
            { Constants.ClaymoreIndex, Rare },
            { Constants.WindSpireIndex, Rare },
            { Constants.LeadRodIndex, Rare },
            { Constants.PiratesSwordIndex, Rare },
            { Constants.BoneSwordIndex, Rare },
            { Constants.FemurIndex, Rare },
            { Constants.CrystalDaggerIndex, Rare },

            { Constants.SteelFalchionIndex, Epic },
            { Constants.TemperedBroadswordIndex, Epic },
            { Constants.IronDirkIndex, Epic },
            { Constants.KudgelIndex, Epic },
            { Constants.OssifiedBladeIndex, Epic },
            { Constants.YetiToothIndex, Epic },
            { Constants.ShadowDaggerIndex, Epic },
            { Constants.BrokenTridentIndex, Epic },

            { Constants.TemplarsBladeIndex, Mythic },
            { Constants.WickedKrisIndex, Mythic },
            { Constants.TheSlammerIndex, Mythic },
            { Constants.InsectHeadIndex, Mythic },
            { Constants.NeptunesGlaiveIndex, Mythic },
            { Constants.ObsidianEdgeIndex, Mythic },
            { Constants.LavaKatanaIndex, Mythic },
            { Constants.IridiumNeedleIndex, Mythic },

            { Constants.ElfBladeIndex, Masterwork },
            { Constants.ForestSwordIndex, Masterwork },
            { Constants.DwarfSwordIndex, Masterwork },
            { Constants.DwarfHammerIndex, Masterwork },
            { Constants.DwarfDaggerIndex, Masterwork },
            { Constants.DragontoothCutlassIndex, Masterwork },
            { Constants.DragontoothClubIndex, Masterwork },
            { Constants.DragontoothShivIndex, Masterwork },

            { Constants.DarkSwordIndex, Legendary },
            { Constants.HolyBladeIndex, Legendary },
            { Constants.GalaxySwordIndex, Legendary },
            { Constants.GalaxyHammerIndex, Legendary },
            { Constants.GalaxyDaggerIndex, Legendary },
            { Constants.InfinityBladeIndex, Legendary },
            { Constants.InfinityGavelIndex, Legendary },
            { Constants.InfinityDaggerIndex, Legendary },
        };
#pragma warning restore SA1509 // Opening braces should not be preceded by blank line
    }

    /// <summary>Initializes a new instance of the <see cref="WeaponTier"/> class.</summary>
    /// <param name="name">The tier name.</param>
    /// <param name="value">The tier value.</param>
    private WeaponTier(string name, int value)
        : base(name, value)
    {
        switch (value)
        {
            case 1:
                this.Color = Color.Green;
                this.Price = 400;
                break;
            case 2:
                this.Color = Color.Blue;
                this.Price = 900;
                break;
            case 3:
                this.Color = Color.Purple;
                this.Price = 1600;
                break;
            case 4:
                this.Color = Color.Red;
                this.Price = 4900;
                break;
            case 5:
                this.Color = Color.MonoGameOrange;
                this.Price = 8100;
                break;
            case 6:
                this.Color = Color.White;
                this.Price = 0;
                break;
            default:
                this.Color = Game1.textColor;
                this.Price = 250;
                break;
        }
    }

    /// <summary>Gets the title color of a weapon at this tier, <see href="https://tvtropes.org/pmwiki/pmwiki.php/Main/ColourCodedForYourConvenience">for your convenience</see>.</summary>
    public Color Color { get; }

    /// <summary>Gets the sell price of a weapon at this tier.</summary>
    public int Price { get; }

    /// <summary>Gets the corresponding <see cref="WeaponTier"/> for the specified <paramref name="weapon"/>.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <returns>A <see cref="WeaponTier"/>.</returns>
    public static WeaponTier GetFor(MeleeWeapon weapon)
    {
        return TierByWeapon.TryGetValue(weapon.InitialParentTileIndex, out var tier) ? tier : Untiered;
    }
}
