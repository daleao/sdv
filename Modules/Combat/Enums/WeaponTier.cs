namespace DaLion.Overhaul.Modules.Combat.Enums;

#region using directives

using System.Collections.Generic;
using Ardalis.SmartEnum;
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

    private static readonly Dictionary<int, WeaponTier> TierByWeapon;

    static WeaponTier()
    {
#pragma warning disable SA1509 // Opening braces should not be preceded by blank line
        TierByWeapon = new Dictionary<int, WeaponTier>
        {
            { ItemIDs.WoodenBlade, Untiered },

            { ItemIDs.SteelSmallsword, Common },
            { ItemIDs.SilverSaber, Common },
            { ItemIDs.CarvingKnife, Common },
            { ItemIDs.WoodClub, Common },
            { ItemIDs.Cutlass, Common },
            { ItemIDs.IronEdge, Common },
            { ItemIDs.BurglarsShank, Common },
            { ItemIDs.WoodMallet, Common },

            { ItemIDs.Rapier, Uncommon },
            { ItemIDs.Claymore, Uncommon },
            { ItemIDs.WindSpire, Uncommon },
            { ItemIDs.LeadRod, Uncommon },
            { ItemIDs.PirateSword, Uncommon },

            { ItemIDs.SteelFalchion, Rare },
            { ItemIDs.TemperedBroadsword, Rare },
            { ItemIDs.IronDirk, Rare },
            { ItemIDs.Kudgel, Rare },
            { ItemIDs.BoneSword, Rare },
            { ItemIDs.Femur, Rare },
            { ItemIDs.CrystalDagger, Rare },
            { ItemIDs.MasterSlingshot, Rare },

            { ItemIDs.TemplarsBlade, Epic },
            { ItemIDs.WickedKris, Epic },
            { ItemIDs.TheSlammer, Epic },
            { ItemIDs.OssifiedBlade, Epic },
            { ItemIDs.ShadowDagger, Epic },
            { ItemIDs.BrokenTrident, Epic },

            { ItemIDs.InsectHead, Mythic },
            { ItemIDs.NeptuneGlaive, Mythic },
            { ItemIDs.YetiTooth, Mythic },
            { ItemIDs.ObsidianEdge, Mythic },
            { ItemIDs.LavaKatana, Mythic },
            { ItemIDs.IridiumNeedle, Mythic },

            { ItemIDs.ElfBlade, Masterwork },
            { ItemIDs.ForestSword, Masterwork },
            { ItemIDs.DwarfSword, Masterwork },
            { ItemIDs.DwarfHammer, Masterwork },
            { ItemIDs.DwarfDagger, Masterwork },
            { ItemIDs.DragontoothCutlass, Masterwork },
            { ItemIDs.DragontoothClub, Masterwork },
            { ItemIDs.DragontoothShiv, Masterwork },

            { ItemIDs.DarkSword, Legendary },
            { ItemIDs.HolyBlade, Legendary },
            { ItemIDs.GalaxySword, Legendary },
            { ItemIDs.GalaxyHammer, Legendary },
            { ItemIDs.GalaxyDagger, Legendary },
            { ItemIDs.GalaxySlingshot, Legendary },
            { ItemIDs.InfinityBlade, Legendary },
            { ItemIDs.InfinityGavel, Legendary },
            { ItemIDs.InfinityDagger, Legendary },
            { ItemIDs.InfinitySlingshot, Legendary },
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

    /// <summary>Gets the corresponding <see cref="WeaponTier"/> for the specified <paramref name="tool"/>.</summary>
    /// <param name="tool">A <see cref="MeleeWeapon"/> or <see cref="Slingshot"/>.</param>
    /// <returns>A <see cref="WeaponTier"/>.</returns>
    public static WeaponTier GetFor(Tool tool)
    {
        return TierByWeapon.TryGetValue(tool.InitialParentTileIndex, out var tier) ? tier : Untiered;
    }
}
