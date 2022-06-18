namespace DaLion.Stardew.Professions.Framework;

#region using directives

using static System.String;
using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.SmartEnum;
using StardewValley;

using Common.Extensions;

#endregion using directives

/// <summary>Represents a vanilla profession.</summary>
/// <remarks>Includes unused <see cref="SmartEnum"/> entries for professions offered by the <see cref="LuckSkill"/> as a fail-safe, since those are handled as <see cref="CustomProfession"/>s.</remarks>
public class Profession : SmartEnum<Profession>, IProfession
{
    #region enum entries

    public static readonly Profession Rancher = new("Rancher", Farmer.rancher, 5);
    public static readonly Profession Harvester = new("Harvester", Farmer.tiller, 5);
    public static readonly Profession Breeder = new("Breeder", Farmer.butcher, 10);
    public static readonly Profession Producer = new("Producer", Farmer.shepherd, 10);
    public static readonly Profession Artisan = new("Artisan", Farmer.artisan, 10);
    public static readonly Profession Agriculturist = new("Agriculturist", Farmer.agriculturist, 10);
    public static readonly Profession Fisher = new("Fisher", Farmer.fisher, 5);
    public static readonly Profession Trapper = new("Trapper", Farmer.trapper, 5);
    public static readonly Profession Angler = new("Angler", Farmer.angler, 10);
    public static readonly Profession Aquarist = new("Aquarist", Farmer.pirate, 10);
    public static readonly Profession Luremaster = new("Luremaster", Farmer.baitmaster, 10);
    public static readonly Profession Conservationist = new("Conservationist", Farmer.mariner, 10);
    public static readonly Profession Lumberjack = new("Lumberjack", Farmer.forester, 5);
    public static readonly Profession Forager = new("Forager", Farmer.gatherer, 5);
    public static readonly Profession Arborist = new("Arborist", Farmer.lumberjack, 10);
    public static readonly Profession Tapper = new("Tapper", Farmer.tapper, 10);
    public static readonly Profession Ecologist = new ("Ecologist", Farmer.botanist, 10);
    public static readonly Profession Scavenger = new("Scavenger", Farmer.tracker, 10);
    public static readonly Profession Miner = new("Miner", Farmer.miner, 5);
    public static readonly Profession Blaster = new("Blaster", Farmer.geologist, 5);
    public static readonly Profession Spelunker = new("Spelunker", Farmer.blacksmith, 10);
    public static readonly Profession Prospector = new("Prospector", Farmer.burrower, 10);
    public static readonly Profession Demolitionist = new("Demolitionist", Farmer.excavator, 10);
    public static readonly Profession Gemologist = new("Gemologist", Farmer.gemologist, 10);
    public static readonly Profession Fighter = new("Fighter", Farmer.fighter, 5);
    public static readonly Profession Rascal = new("Rascal", Farmer.scout, 5);
    public static readonly Profession Brute = new("Brute", Farmer.brute, 10);
    public static readonly Profession Poacher = new("Poacher", Farmer.defender, 10);
    public static readonly Profession Piper = new("Piper", Farmer.acrobat, 10);
    public static readonly Profession Desperado = new("Desperado", Farmer.desperado, 10);

    public static readonly Profession Luck_T1A = new("Fortunate", 30, 5);
    public static readonly Profession Luck_T1B = new("PopularHelper", 31, 5);
    public static readonly Profession Luck_T2A1 = new("Lucky", 32, 10);
    public static readonly Profession Luck_T2A2 = new("UnUnlucky", 33, 10);
    public static readonly Profession Luck_T2B1 = new("ShootingStar", 34, 10);
    public static readonly Profession Luck_T2B2 = new("SpiritChild", 35, 10);

    #endregion enum entries

    /// <inheritdoc />
    public string StringId => Name;

    /// <inheritdoc />
    public int Id => Value;

    /// <inheritdoc />
    public int Level { get; }

    /// <inheritdoc />
    public ISkill Skill => Framework.Skill.FromValue(Value / 6);

    /// <summary>Construct an instance.</summary>
    /// <param name="name">The profession name.</param>
    /// <param name="value">The profession index.</param>
    /// <param name="level">The level at which the profession is offered (either 5 or 10).</param>
    public Profession(string name, int value, int level) : base(name, value)
    {
        Level = level;
    }

    /// <inheritdoc />
    public string GetDisplayName(bool male = true)
    {
        return ModEntry.i18n.Get(Name.ToLowerInvariant() + ".name" + (male ? ".male" : ".female"));
    }

    /// <inheritdoc />
    public string GetDescription(bool prestiged = false)
    {
        return ModEntry.i18n.Get(Name.ToLowerInvariant() + ".desc" + (prestiged ? ".prestiged" : Empty));
    }

    /// <summary>Get the profession corresponding to the specified localized name.</summary>
    /// <param name="name">A localized profession name.</param>
    /// <param name="ignoreCase">Whether to ignore capitalization.</param>
    public static bool TryFromLocalizedName(string name, bool ignoreCase, out Profession result)
    {
        var stringComparison = ignoreCase
            ? StringComparison.InvariantCultureIgnoreCase
            : StringComparison.InvariantCulture;
        result = null;
        if (string.Equals(name, Rancher.GetDisplayName(true).TrimAll(), stringComparison) ||
            string.Equals(name, Rancher.GetDisplayName(false).TrimAll(), stringComparison)) result = Rancher;
        if (string.Equals(name, Harvester.GetDisplayName(true).TrimAll(), stringComparison) ||
            string.Equals(name, Harvester.GetDisplayName(false).TrimAll(), stringComparison)) result = Harvester;
        if (string.Equals(name, Breeder.GetDisplayName(true).TrimAll(), stringComparison) ||
            string.Equals(name, Breeder.GetDisplayName(false).TrimAll(), stringComparison)) result = Breeder;
        if (string.Equals(name, Producer.GetDisplayName(true).TrimAll(), stringComparison) ||
            string.Equals(name, Producer.GetDisplayName(false).TrimAll(), stringComparison)) result = Producer;
        if (string.Equals(name, Artisan.GetDisplayName(true).TrimAll(), stringComparison) ||
            string.Equals(name, Artisan.GetDisplayName(false).TrimAll(), stringComparison)) result = Artisan;
        if (string.Equals(name, Agriculturist.GetDisplayName(true).TrimAll(), stringComparison) ||
            string.Equals(name, Agriculturist.GetDisplayName(false).TrimAll(), stringComparison)) result = Agriculturist;
        if (string.Equals(name, Fisher.GetDisplayName(true).TrimAll(), stringComparison) ||
            string.Equals(name, Fisher.GetDisplayName(false).TrimAll(), stringComparison)) result = Fisher;
        if (string.Equals(name, Trapper.GetDisplayName(true).TrimAll(), stringComparison) ||
            string.Equals(name, Trapper.GetDisplayName(false).TrimAll(), stringComparison)) result = Trapper;
        if (string.Equals(name, Angler.GetDisplayName(true).TrimAll(), stringComparison) ||
            string.Equals(name, Angler.GetDisplayName(false).TrimAll(), stringComparison)) result = Angler;
        if (string.Equals(name, Aquarist.GetDisplayName(true).TrimAll(), stringComparison) ||
            string.Equals(name, Aquarist.GetDisplayName(false).TrimAll(), stringComparison)) result = Aquarist;
        if (string.Equals(name, Luremaster.GetDisplayName(true).TrimAll(), stringComparison) ||
            string.Equals(name, Luremaster.GetDisplayName(false).TrimAll(), stringComparison)) result = Luremaster;
        if (string.Equals(name, Conservationist.GetDisplayName(true).TrimAll(), stringComparison) ||
            string.Equals(name, Conservationist.GetDisplayName(false).TrimAll(), stringComparison)) result = Conservationist;
        if (string.Equals(name, Lumberjack.GetDisplayName(true).TrimAll(), stringComparison) ||
            string.Equals(name, Lumberjack.GetDisplayName(false).TrimAll(), stringComparison)) result = Lumberjack;
        if (string.Equals(name, Forager.GetDisplayName(true).TrimAll(), stringComparison) ||
            string.Equals(name, Forager.GetDisplayName(false).TrimAll(), stringComparison)) result = Forager;
        if (string.Equals(name, Arborist.GetDisplayName(true).TrimAll(), stringComparison) ||
            string.Equals(name, Arborist.GetDisplayName(false).TrimAll(), stringComparison)) result = Arborist;
        if (string.Equals(name, Tapper.GetDisplayName(true).TrimAll(), stringComparison) ||
            string.Equals(name, Tapper.GetDisplayName(false).TrimAll(), stringComparison)) result = Tapper;
        if (string.Equals(name, Ecologist.GetDisplayName(true).TrimAll(), stringComparison) ||
            string.Equals(name, Ecologist.GetDisplayName(false).TrimAll(), stringComparison)) result = Ecologist;
        if (string.Equals(name, Scavenger.GetDisplayName(true).TrimAll(), stringComparison) ||
            string.Equals(name, Scavenger.GetDisplayName(false).TrimAll(), stringComparison)) result = Scavenger;
        if (string.Equals(name, Miner.GetDisplayName(true).TrimAll(), stringComparison) ||
            string.Equals(name, Miner.GetDisplayName(false).TrimAll(), stringComparison)) result = Miner;
        if (string.Equals(name, Blaster.GetDisplayName(true).TrimAll(), stringComparison) ||
            string.Equals(name, Blaster.GetDisplayName(false).TrimAll(), stringComparison)) result = Blaster;
        if (string.Equals(name, Spelunker.GetDisplayName(true).TrimAll(), stringComparison) ||
            string.Equals(name, Spelunker.GetDisplayName(false).TrimAll(), stringComparison)) result = Spelunker;
        if (string.Equals(name, Prospector.GetDisplayName(true).TrimAll(), stringComparison) ||
            string.Equals(name, Prospector.GetDisplayName(false).TrimAll(), stringComparison)) result = Prospector;
        if (string.Equals(name, Demolitionist.GetDisplayName(true).TrimAll(), stringComparison) ||
            string.Equals(name, Demolitionist.GetDisplayName(false).TrimAll(), stringComparison)) result = Demolitionist;
        if (string.Equals(name, Gemologist.GetDisplayName(true).TrimAll(), stringComparison) ||
            string.Equals(name, Gemologist.GetDisplayName(false).TrimAll(), stringComparison)) result = Gemologist;
        if (string.Equals(name, Fighter.GetDisplayName(true).TrimAll(), stringComparison) ||
            string.Equals(name, Fighter.GetDisplayName(false).TrimAll(), stringComparison)) result = Fighter;
        if (string.Equals(name, Rascal.GetDisplayName(true).TrimAll(), stringComparison) ||
            string.Equals(name, Rascal.GetDisplayName(false).TrimAll(), stringComparison)) result = Rascal;
        if (string.Equals(name, Brute.GetDisplayName(true).TrimAll(), stringComparison) ||
            string.Equals(name, Brute.GetDisplayName(false).TrimAll(), stringComparison)) result = Brute;
        if (string.Equals(name, Poacher.GetDisplayName(true).TrimAll(), stringComparison) ||
            string.Equals(name, Poacher.GetDisplayName(false).TrimAll(), stringComparison)) result = Poacher;
        if (string.Equals(name, Piper.GetDisplayName(true).TrimAll(), stringComparison) ||
            string.Equals(name, Piper.GetDisplayName(false).TrimAll(), stringComparison)) result = Piper;
        if (string.Equals(name, Desperado.GetDisplayName(true).TrimAll(), stringComparison) ||
            string.Equals(name, Desperado.GetDisplayName(false).TrimAll(), stringComparison)) result = Desperado;
        return result is not null;
    }

    /// <summary>Get the range of indices corresponding to all professions.</summary>
    public static IEnumerable<int> GetRange(bool includeLuck = false)
    {
        var range = Enumerable.Range(0, 30);
        return includeLuck ? range.Concat(Enumerable.Range(30, 6)) : range;
    }
}