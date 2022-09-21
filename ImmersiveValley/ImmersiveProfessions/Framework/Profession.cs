namespace DaLion.Stardew.Professions.Framework;

#region using directives

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Ardalis.SmartEnum;
using DaLion.Common.Extensions;
using static System.String;

#endregion using directives

/// <summary>Represents a vanilla profession.</summary>
/// <remarks>
///     Includes unused <see cref="Ardalis.SmartEnum"/> entries for professions offered by the
///     <see cref="LuckSkill"/> as a fail-safe, since those are handled as <see cref="CustomProfession"/>s.
/// </remarks>
public sealed class Profession : SmartEnum<Profession>, IProfession
{
    #region enum entries

    /// <summary>The Rancher profession, available at <see cref="Skill.Farming"/> level 5.</summary>
    public static readonly Profession Rancher = new("Rancher", Farmer.rancher, 5);

    /// <summary>The Harvester profession, available at <see cref="Skill.Farming"/> level 5.</summary>
    public static readonly Profession Harvester = new("Harvester", Farmer.tiller, 5);

    /// <summary>The Breeder profession, available at <see cref="Skill.Farming"/> level 10.</summary>
    public static readonly Profession Breeder = new("Breeder", Farmer.butcher, 10);

    /// <summary>The Producer profession, available at <see cref="Skill.Farming"/> level 10.</summary>
    public static readonly Profession Producer = new("Producer", Farmer.shepherd, 10);

    /// <summary>The Artisan profession, available at <see cref="Skill.Farming"/> level 10.</summary>
    public static readonly Profession Artisan = new("Artisan", Farmer.artisan, 10);

    /// <summary>The Agriculturist profession, available at <see cref="Skill.Farming"/> level 10.</summary>
    public static readonly Profession Agriculturist = new("Agriculturist", Farmer.agriculturist, 10);

    /// <summary>The Fisher profession, available at <see cref="Skill.Fishing"/> level 5.</summary>
    public static readonly Profession Fisher = new("Fisher", Farmer.fisher, 5);

    /// <summary>The Trapper profession, available at <see cref="Skill.Fishing"/> level 5.</summary>
    public static readonly Profession Trapper = new("Trapper", Farmer.trapper, 5);

    /// <summary>The Angler profession, available at <see cref="Skill.Fishing"/> level 10.</summary>
    public static readonly Profession Angler = new("Angler", Farmer.angler, 10);

    /// <summary>The Aquarist profession, available at <see cref="Skill.Fishing"/> level 10.</summary>
    public static readonly Profession Aquarist = new("Aquarist", Farmer.pirate, 10);

    /// <summary>The Luremaster profession, available at <see cref="Skill.Fishing"/> level 10.</summary>
    public static readonly Profession Luremaster = new("Luremaster", Farmer.baitmaster, 10);

    /// <summary>The Conservationist profession, available at <see cref="Skill.Fishing"/> level 10.</summary>
    public static readonly Profession Conservationist = new("Conservationist", Farmer.mariner, 10);

    /// <summary>The Lumberjack profession, available at <see cref="Skill.Foraging"/> level 5.</summary>
    public static readonly Profession Lumberjack = new("Lumberjack", Farmer.forester, 5);

    /// <summary>The Forager profession, available at <see cref="Skill.Foraging"/> level 5.</summary>
    public static readonly Profession Forager = new("Forager", Farmer.gatherer, 5);

    /// <summary>The Arborist profession, available at <see cref="Skill.Foraging"/> level 10.</summary>
    public static readonly Profession Arborist = new("Arborist", Farmer.lumberjack, 10);

    /// <summary>The Trapper profession, available at <see cref="Skill.Foraging"/> level 10.</summary>
    public static readonly Profession Tapper = new("Tapper", Farmer.tapper, 10);

    /// <summary>The Ecologist profession, available at <see cref="Skill.Foraging"/> level 10.</summary>
    public static readonly Profession Ecologist = new("Ecologist", Farmer.botanist, 10);

    /// <summary>The Scavenger profession, available at <see cref="Skill.Foraging"/> level 10.</summary>
    public static readonly Profession Scavenger = new("Scavenger", Farmer.tracker, 10);

    /// <summary>The Miner profession, available at <see cref="Skill.Mining"/> level 5.</summary>
    public static readonly Profession Miner = new("Miner", Farmer.miner, 5);

    /// <summary>The Blaster profession, available at <see cref="Skill.Mining"/> level 5.</summary>
    public static readonly Profession Blaster = new("Blaster", Farmer.geologist, 5);

    /// <summary>The Spelunker profession, available at <see cref="Skill.Mining"/> level 10.</summary>
    public static readonly Profession Spelunker = new("Spelunker", Farmer.blacksmith, 10);

    /// <summary>The Prospector profession, available at <see cref="Skill.Mining"/> level 10.</summary>
    public static readonly Profession Prospector = new("Prospector", Farmer.burrower, 10);

    /// <summary>The Demolitionist profession, available at <see cref="Skill.Mining"/> level 10.</summary>
    public static readonly Profession Demolitionist = new("Demolitionist", Farmer.excavator, 10);

    /// <summary>The Gemologist profession, available at <see cref="Skill.Mining"/> level 10.</summary>
    public static readonly Profession Gemologist = new("Gemologist", Farmer.gemologist, 10);

    /// <summary>The Fighter profession, available at <see cref="Skill.Combat"/> level 5.</summary>
    public static readonly Profession Fighter = new("Fighter", Farmer.fighter, 5);

    /// <summary>The Rascal profession, available at <see cref="Skill.Combat"/> level 5.</summary>
    public static readonly Profession Rascal = new("Rascal", Farmer.scout, 5);

    /// <summary>The Brute profession, available at <see cref="Skill.Combat"/> level 10.</summary>
    public static readonly Profession Brute = new("Brute", Farmer.brute, 10);

    /// <summary>The Poacher profession, available at <see cref="Skill.Combat"/> level 10.</summary>
    public static readonly Profession Poacher = new("Poacher", Farmer.defender, 10);

    /// <summary>The Piper profession, available at <see cref="Skill.Combat"/> level 10.</summary>
    public static readonly Profession Piper = new("Piper", Farmer.acrobat, 10);

    /// <summary>The Desperado profession, available at <see cref="Skill.Combat"/> level 10.</summary>
    public static readonly Profession Desperado = new("Desperado", Farmer.desperado, 10);

    #endregion enum entries

    /// <summary>Initializes a new instance of the <see cref="Profession"/> class.</summary>
    /// <param name="name">The profession name.</param>
    /// <param name="value">The profession index.</param>
    /// <param name="level">The level at which the profession is offered (either 5 or 10).</param>
    public Profession(string name, int value, int level)
        : base(name, value)
    {
        this.Level = level;
    }

    /// <inheritdoc />
    public string StringId => this.Name;

    /// <inheritdoc />
    public string DisplayName =>
        ModEntry.i18n.Get(this.Name.ToLower() + ".title." + (Game1.player.IsMale ? "male" : "female"));

    /// <inheritdoc />
    public int Id => this.Value;

    /// <inheritdoc />
    public int Level { get; }

    /// <inheritdoc />
    public ISkill Skill => Framework.Skill.FromValue(this.Value / 6);

    /// <summary>Get the <see cref="Profession"/> with the specified localized name.</summary>
    /// <param name="name">A localized profession name.</param>
    /// <param name="ignoreCase">Whether to ignore capitalization.</param>
    /// <param name="result">The corresponding profession.</param>
    /// <returns><see langword="true"/> if a matching profession was found, otherwise <see langword="false"/>.</returns>
    public static bool TryFromLocalizedName(string name, bool ignoreCase, [NotNullWhen(true)] out Profession? result)
    {
        var stringComparison = ignoreCase
            ? StringComparison.InvariantCultureIgnoreCase
            : StringComparison.InvariantCulture;
        result = null;
        if (string.Equals(name, Rancher.DisplayName.TrimAll(), stringComparison))
        {
            result = Rancher;
        }
        else if (string.Equals(name, Harvester.DisplayName))
        {
            result = Harvester;
        }
        else if (string.Equals(name, Breeder.DisplayName))
        {
            result = Breeder;
        }
        else if (string.Equals(name, Producer.DisplayName))
        {
            result = Producer;
        }
        else if (string.Equals(name, Artisan.DisplayName))
        {
            result = Artisan;
        }
        else if (string.Equals(name, Agriculturist.DisplayName))
        {
            result = Agriculturist;
        }
        else if (string.Equals(name, Fisher.DisplayName))
        {
            result = Fisher;
        }
        else if (string.Equals(name, Trapper.DisplayName))
        {
            result = Trapper;
        }
        else if (string.Equals(name, Angler.DisplayName))
        {
            result = Angler;
        }
        else if (string.Equals(name, Aquarist.DisplayName))
        {
            result = Aquarist;
        }
        else if (string.Equals(name, Luremaster.DisplayName))
        {
            result = Luremaster;
        }
        else if (string.Equals(name, Conservationist.DisplayName))
        {
            result = Conservationist;
        }
        else if (string.Equals(name, Lumberjack.DisplayName))
        {
            result = Lumberjack;
        }
        else if (string.Equals(name, Forager.DisplayName))
        {
            result = Forager;
        }
        else if (string.Equals(name, Arborist.DisplayName))
        {
            result = Arborist;
        }
        else if (string.Equals(name, Tapper.DisplayName))
        {
            result = Tapper;
        }
        else if (string.Equals(name, Ecologist.DisplayName))
        {
            result = Ecologist;
        }
        else if (string.Equals(name, Scavenger.DisplayName))
        {
            result = Scavenger;
        }
        else if (string.Equals(name, Miner.DisplayName))
        {
            result = Miner;
        }
        else if (string.Equals(name, Blaster.DisplayName))
        {
            result = Blaster;
        }
        else if (string.Equals(name, Spelunker.DisplayName))
        {
            result = Spelunker;
        }
        else if (string.Equals(name, Prospector.DisplayName))
        {
            result = Prospector;
        }
        else if (string.Equals(name, Demolitionist.DisplayName))
        {
            result = Demolitionist;
        }
        else if (string.Equals(name, Gemologist.DisplayName))
        {
            result = Gemologist;
        }
        else if (string.Equals(name, Fighter.DisplayName))
        {
            result = Fighter;
        }
        else if (string.Equals(name, Rascal.DisplayName))
        {
            result = Rascal;
        }
        else if (string.Equals(name, Brute.DisplayName))
        {
            result = Brute;
        }
        else if (string.Equals(name, Poacher.DisplayName))
        {
            result = Poacher;
        }
        else if (string.Equals(name, Piper.DisplayName))
        {
            result = Piper;
        }
        else if (string.Equals(name, Desperado.DisplayName))
        {
            result = Desperado;
        }

        return result is not null;
    }

    /// <summary>Get the range of indices corresponding to all vanilla, plus optionally Luck Skill, professions.</summary>
    /// <param name="includeLuck">Whether to include Luck Skill professions.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of all vanilla, plus optionally Luck Skill, profession indices.</returns>
    public static IEnumerable<int> GetRange(bool includeLuck = false)
    {
        var range = Enumerable.Range(0, 30);
        return includeLuck ? range.Concat(Enumerable.Range(30, 6)) : range;
    }

    /// <inheritdoc />
    public string GetDescription(bool prestiged = false)
    {
        return ModEntry.i18n.Get(this.Name.ToLowerInvariant() + ".desc" + (prestiged ? ".prestiged" : Empty));
    }
}
