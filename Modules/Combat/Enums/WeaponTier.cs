namespace DaLion.Overhaul.Modules.Combat.Enums;

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

    private static readonly Dictionary<int, WeaponTier> TierByWeapon;

    static WeaponTier()
    {
#pragma warning disable SA1509 // Opening braces should not be preceded by blank line
        TierByWeapon = new Dictionary<int, WeaponTier>
        {
            { WeaponIds.WoodenBlade, Untiered },

            { WeaponIds.SteelSmallsword, Common },
            { WeaponIds.SilverSaber, Common },
            { WeaponIds.CarvingKnife, Common },
            { WeaponIds.WoodClub, Common },
            { WeaponIds.Cutlass, Common },
            { WeaponIds.IronEdge, Common },
            { WeaponIds.BurglarsShank, Common },
            { WeaponIds.WoodMallet, Common },

            { WeaponIds.Rapier, Uncommon },
            { WeaponIds.Claymore, Uncommon },
            { WeaponIds.WindSpire, Uncommon },
            { WeaponIds.LeadRod, Uncommon },
            { WeaponIds.PiratesSword, Uncommon },

            { WeaponIds.SteelFalchion, Rare },
            { WeaponIds.TemperedBroadsword, Rare },
            { WeaponIds.IronDirk, Rare },
            { WeaponIds.Kudgel, Rare },
            { WeaponIds.BoneSword, Rare },
            { WeaponIds.Femur, Rare },
            { WeaponIds.CrystalDagger, Rare },
            { WeaponIds.MasterSlingshot, Rare },

            { WeaponIds.TemplarsBlade, Epic },
            { WeaponIds.WickedKris, Epic },
            { WeaponIds.TheSlammer, Epic },
            { WeaponIds.OssifiedBlade, Epic },
            { WeaponIds.ShadowDagger, Epic },
            { WeaponIds.BrokenTrident, Epic },

            { WeaponIds.InsectHead, Mythic },
            { WeaponIds.NeptuneGlaive, Mythic },
            { WeaponIds.YetiTooth, Mythic },
            { WeaponIds.ObsidianEdge, Mythic },
            { WeaponIds.LavaKatana, Mythic },
            { WeaponIds.IridiumNeedle, Mythic },

            { WeaponIds.ElfBlade, Masterwork },
            { WeaponIds.ForestSword, Masterwork },
            { WeaponIds.DwarfSword, Masterwork },
            { WeaponIds.DwarfHammer, Masterwork },
            { WeaponIds.DwarfDagger, Masterwork },
            { WeaponIds.DragontoothCutlass, Masterwork },
            { WeaponIds.DragontoothClub, Masterwork },
            { WeaponIds.DragontoothShiv, Masterwork },

            { WeaponIds.DarkSword, Legendary },
            { WeaponIds.HolyBlade, Legendary },
            { WeaponIds.GalaxySword, Legendary },
            { WeaponIds.GalaxyHammer, Legendary },
            { WeaponIds.GalaxyDagger, Legendary },
            { WeaponIds.GalaxySlingshot, Legendary },
            { WeaponIds.InfinityBlade, Legendary },
            { WeaponIds.InfinityGavel, Legendary },
            { WeaponIds.InfinityDagger, Legendary },
            { WeaponIds.InfinitySlingshot, Legendary },
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
                this.Color = CombatModule.Config.UncommonTierColor;
                this.Price = 400;
                break;
            case 2:
                this.Color = CombatModule.Config.RareTierColor;
                this.Price = 900;
                break;
            case 3:
                this.Color = CombatModule.Config.EpicTierColor;
                this.Price = 1600;
                break;
            case 4:
                this.Color = CombatModule.Config.MythicTierColor;
                this.Price = 4900;
                break;
            case 5:
                this.Color = CombatModule.Config.MasterworkTierColor;
                this.Price = 8100;
                break;
            case 6:
                this.Color = CombatModule.Config.LegendaryTierColor;
                this.Price = 0;
                break;
            default:
                this.Color = CombatModule.Config.CommonTierColor;
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
