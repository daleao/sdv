namespace DaLion.Arsenal.Framework.Enums;

#region using directives

using System.Collections.Generic;
using Ardalis.SmartEnum;
using DaLion.Shared.Constants;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

/// <summary>The tier of a <see cref="MeleeWeapon"/> or <see cref="Slingshot"/>.</summary>
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

    static WeaponTier()
    {
        TierByWeapon = new Dictionary<string, WeaponTier>
        {
            { QualifiedWeaponIds.RustySword, Untiered },
            { QualifiedWeaponIds.WoodenBlade, Untiered },

            { QualifiedWeaponIds.SteelSmallsword, Common },
            { QualifiedWeaponIds.SilverSaber, Common },
            { QualifiedWeaponIds.CarvingKnife, Common },
            { QualifiedWeaponIds.WoodClub, Common },
            { QualifiedWeaponIds.Cutlass, Common },
            { QualifiedWeaponIds.IronEdge, Common },
            { QualifiedWeaponIds.BurglarsShank, Common },
            { QualifiedWeaponIds.WoodMallet, Common },

            { QualifiedWeaponIds.Rapier, Uncommon },
            { QualifiedWeaponIds.Claymore, Uncommon },
            { QualifiedWeaponIds.WindSpire, Uncommon },
            { QualifiedWeaponIds.LeadRod, Uncommon },
            { QualifiedWeaponIds.PiratesSword, Uncommon },

            { QualifiedWeaponIds.SteelFalchion, Rare },
            { QualifiedWeaponIds.TemperedBroadsword, Rare },
            { QualifiedWeaponIds.IronDirk, Rare },
            { QualifiedWeaponIds.Kudgel, Rare },
            { QualifiedWeaponIds.BoneSword, Rare },
            { QualifiedWeaponIds.Femur, Rare },
            { QualifiedWeaponIds.CrystalDagger, Rare },
            { QualifiedWeaponIds.MasterSlingshot, Rare },

            { QualifiedWeaponIds.TemplarsBlade, Epic },
            { QualifiedWeaponIds.WickedKris, Epic },
            { QualifiedWeaponIds.TheSlammer, Epic },
            { QualifiedWeaponIds.OssifiedBlade, Epic },
            { QualifiedWeaponIds.ShadowDagger, Epic },
            { QualifiedWeaponIds.BrokenTrident, Epic },

            { QualifiedWeaponIds.InsectHead, Mythic },
            { QualifiedWeaponIds.NeptuneGlaive, Mythic },
            { QualifiedWeaponIds.YetiTooth, Mythic },
            { QualifiedWeaponIds.ObsidianEdge, Mythic },
            { QualifiedWeaponIds.LavaKatana, Mythic },
            { QualifiedWeaponIds.IridiumNeedle, Mythic },

            { QualifiedWeaponIds.ElfBlade, Masterwork },
            { QualifiedWeaponIds.ForestSword, Masterwork },
            { QualifiedWeaponIds.DwarfSword, Masterwork },
            { QualifiedWeaponIds.DwarfHammer, Masterwork },
            { QualifiedWeaponIds.DwarfDagger, Masterwork },
            { QualifiedWeaponIds.DragontoothCutlass, Masterwork },
            { QualifiedWeaponIds.DragontoothClub, Masterwork },
            { QualifiedWeaponIds.DragontoothShiv, Masterwork },

            { QualifiedWeaponIds.DarkSword, Legendary },
            { QualifiedWeaponIds.HolyBlade, Legendary },
            { QualifiedWeaponIds.GalaxySword, Legendary },
            { QualifiedWeaponIds.GalaxyHammer, Legendary },
            { QualifiedWeaponIds.GalaxyDagger, Legendary },
            { QualifiedWeaponIds.GalaxySlingshot, Legendary },
            { QualifiedWeaponIds.InfinityBlade, Legendary },
            { QualifiedWeaponIds.InfinityGavel, Legendary },
            { QualifiedWeaponIds.InfinityDagger, Legendary },
            { QualifiedWeaponIds.InfinitySlingshot, Legendary },
        };
#pragma warning restore SA1509 // Opening braces should not be preceded by blank line
    }

    /// <summary>Initializes a new instance of the <see cref="WeaponTier"/> class.</summary>
    /// <param name="name">The tier name.</param>
    /// <param name="value">The tier value.</param>
    private WeaponTier(string name, int value)
        : base(name, value)
    {
        this.Color = value >= 0 ? Config.ColorByTier[this.Value] : Game1.textColor;
        this.Price = value switch
        {
            1 => 400,
            2 => 900,
            3 => 1600,
            4 => 4900,
            5 => 8100,
            6 => 0,
            _ => 50,
        };
    }

    /// <summary>Gets the title color of a weapon at this tier, <see href="https://tvtropes.org/pmwiki/pmwiki.php/Main/ColourCodedForYourConvenience">for your convenience</see>.</summary>
    public Color Color { get; }

    /// <summary>Gets the sell price of a weapon at this tier.</summary>
    public int Price { get; }

    /// <summary>Gets a lookup dictionary for a weapon's tier given its sheet index.</summary>
    internal static Dictionary<string, WeaponTier> TierByWeapon { get; }

    /// <summary>Gets the corresponding <see cref="WeaponTier"/> for the specified <paramref name="tool"/>.</summary>
    /// <param name="tool">A <see cref="MeleeWeapon"/> or <see cref="Slingshot"/>.</param>
    /// <returns>A <see cref="WeaponTier"/>.</returns>
    public static WeaponTier GetFor(Tool tool)
    {
        return TierByWeapon.TryGetValue(tool.QualifiedItemId, out var tier) ? tier : Untiered;
    }
}
