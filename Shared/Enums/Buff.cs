namespace DaLion.Shared.Enums;

#region using directives

using NetEscapades.EnumGenerators;

#endregion using directives

/// <summary>A named <see cref="StardewValley.Buff"/> applied on a <see cref="Farmer"/>.</summary>
[EnumExtensions]
public enum Buff
{
    /// <summary>The Burnt debuff, caused by being hit by a Magma Sparker.</summary>
    Burnt = 12,

    /// <summary>Full. Implementation unknown.</summary>
    Full = 6,

    /// <summary>Burnt. Implementation unknown.</summary>
    Quenched = 7,

    /// <summary>The Tipsy debuff, caused by consuming alcohol.</summary>
    Tipsy = 17,

    /// <summary>The Slimed debuff, causing by being hit by a Slime.</summary>
    Slimed = 13,

    /// <summary>The Spooked debuff. Implementation unknown.</summary>
    Spooked = 18,

    /// <summary>The Jinxed debuff, caused by being hit by a Shadow Shaman's fireball.</summary>
    Jinxed = 14,

    /// <summary>The Frozen debuff, caused by being hit by a Skeleton Mage.</summary>
    Frozen = 19,

    /// <summary>The Warrior Energy buff, granted by killing an enemy while wearing a Warrior Ring.</summary>
    WarriorEnergy = 20,

    /// <summary>The Yoba's Blessing buff, granted by being hit while wearing a Ring of Yoba.</summary>
    YobasBlessing = 21,

    /// <summary>The Adrenaline Rush buff, granted by killing an enemy while wearing a Savage Ring.</summary>
    AdrenalineRush = 22,

    /// <summary>The Oil Of Garlic buff, granted by consuming Oil of Garlic.</summary>
    OilOfGarlic = 23,

    /// <summary>The Monster Musk buff, granted by consuming Monster Musk.</summary>
    MonsterMusk = 24,

    /// <summary>The Nauseated debuff, caused by being hit by a Putrid Ghost.</summary>
    Nauseated = 25,

    /// <summary>The Darkness debuff, caused by being hit by a Shadow Sniper.</summary>
    Darkness = 26,

    /// <summary>The Weakness debuff, caused by being hit by a Blue Squid's orb.</summary>
    Weakness = 27,

    /// <summary>The Squid Ink Raviola buff, granted by consuming Squid Ink Ravioli.</summary>
    SquidInkRavioli = 28,
}
