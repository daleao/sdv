namespace DaLion.Overhaul.Modules.Professions;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Ardalis.SmartEnum;
using DaLion.Shared.Extensions;
using Extensions;
using Microsoft.Xna.Framework;
using static System.String;

#endregion using directives

/// <summary>Represents a vanilla profession.</summary>
/// <remarks>
///     Includes unused <see cref="Ardalis.SmartEnum"/> entries for professions offered by the
///     <see cref="LuckSkill"/> as a fail-safe, since those are handled as <see cref="CustomProfession"/>s.
/// </remarks>
public sealed class VanillaProfession : SmartEnum<VanillaProfession>, IProfession
{
    #region enum entries

    /// <summary>The Rancher profession, available at <see cref="VanillaSkill.Farming"/> level 5.</summary>
    public static readonly VanillaProfession Rancher = new("Rancher", Farmer.rancher, 5);

    /// <summary>The Harvester profession, available at <see cref="VanillaSkill.Farming"/> level 5.</summary>
    public static readonly VanillaProfession Harvester = new("Harvester", Farmer.tiller, 5);

    /// <summary>The Breeder profession, available at <see cref="VanillaSkill.Farming"/> level 10.</summary>
    public static readonly VanillaProfession Breeder = new("Breeder", Farmer.butcher, 10);

    /// <summary>The Producer profession, available at <see cref="VanillaSkill.Farming"/> level 10.</summary>
    public static readonly VanillaProfession Producer = new("Producer", Farmer.shepherd, 10);

    /// <summary>The Artisan profession, available at <see cref="VanillaSkill.Farming"/> level 10.</summary>
    public static readonly VanillaProfession Artisan = new("Artisan", Farmer.artisan, 10);

    /// <summary>The Agriculturist profession, available at <see cref="VanillaSkill.Farming"/> level 10.</summary>
    public static readonly VanillaProfession Agriculturist = new("Agriculturist", Farmer.agriculturist, 10);

    /// <summary>The Fisher profession, available at <see cref="VanillaSkill.Fishing"/> level 5.</summary>
    public static readonly VanillaProfession Fisher = new("Fisher", Farmer.fisher, 5);

    /// <summary>The Trapper profession, available at <see cref="VanillaSkill.Fishing"/> level 5.</summary>
    public static readonly VanillaProfession Trapper = new("Trapper", Farmer.trapper, 5);

    /// <summary>The Angler profession, available at <see cref="VanillaSkill.Fishing"/> level 10.</summary>
    public static readonly VanillaProfession Angler = new("Angler", Farmer.angler, 10);

    /// <summary>The Aquarist profession, available at <see cref="VanillaSkill.Fishing"/> level 10.</summary>
    public static readonly VanillaProfession Aquarist = new("Aquarist", Farmer.pirate, 10);

    /// <summary>The Luremaster profession, available at <see cref="VanillaSkill.Fishing"/> level 10.</summary>
    public static readonly VanillaProfession Luremaster = new("Luremaster", Farmer.baitmaster, 10);

    /// <summary>The Conservationist profession, available at <see cref="VanillaSkill.Fishing"/> level 10.</summary>
    public static readonly VanillaProfession Conservationist = new("Conservationist", Farmer.mariner, 10);

    /// <summary>The Lumberjack profession, available at <see cref="VanillaSkill.Foraging"/> level 5.</summary>
    public static readonly VanillaProfession Lumberjack = new("Lumberjack", Farmer.forester, 5);

    /// <summary>The Forager profession, available at <see cref="VanillaSkill.Foraging"/> level 5.</summary>
    public static readonly VanillaProfession Forager = new("Forager", Farmer.gatherer, 5);

    /// <summary>The Arborist profession, available at <see cref="VanillaSkill.Foraging"/> level 10.</summary>
    public static readonly VanillaProfession Arborist = new("Arborist", Farmer.lumberjack, 10);

    /// <summary>The Trapper profession, available at <see cref="VanillaSkill.Foraging"/> level 10.</summary>
    public static readonly VanillaProfession Tapper = new("Tapper", Farmer.tapper, 10);

    /// <summary>The Ecologist profession, available at <see cref="VanillaSkill.Foraging"/> level 10.</summary>
    public static readonly VanillaProfession Ecologist = new("Ecologist", Farmer.botanist, 10);

    /// <summary>The Scavenger profession, available at <see cref="VanillaSkill.Foraging"/> level 10.</summary>
    public static readonly VanillaProfession Scavenger = new("Scavenger", Farmer.tracker, 10);

    /// <summary>The Miner profession, available at <see cref="VanillaSkill.Mining"/> level 5.</summary>
    public static readonly VanillaProfession Miner = new("Miner", Farmer.miner, 5);

    /// <summary>The Blaster profession, available at <see cref="VanillaSkill.Mining"/> level 5.</summary>
    public static readonly VanillaProfession Blaster = new("Blaster", Farmer.geologist, 5);

    /// <summary>The Spelunker profession, available at <see cref="VanillaSkill.Mining"/> level 10.</summary>
    public static readonly VanillaProfession Spelunker = new("Spelunker", Farmer.blacksmith, 10);

    /// <summary>The Prospector profession, available at <see cref="VanillaSkill.Mining"/> level 10.</summary>
    public static readonly VanillaProfession Prospector = new("Prospector", Farmer.burrower, 10);

    /// <summary>The Demolitionist profession, available at <see cref="VanillaSkill.Mining"/> level 10.</summary>
    public static readonly VanillaProfession Demolitionist = new("Demolitionist", Farmer.excavator, 10);

    /// <summary>The Gemologist profession, available at <see cref="VanillaSkill.Mining"/> level 10.</summary>
    public static readonly VanillaProfession Gemologist = new("Gemologist", Farmer.gemologist, 10);

    /// <summary>The Fighter profession, available at <see cref="VanillaSkill.Combat"/> level 5.</summary>
    public static readonly VanillaProfession Fighter = new("Fighter", Farmer.fighter, 5);

    /// <summary>The Rascal profession, available at <see cref="VanillaSkill.Combat"/> level 5.</summary>
    public static readonly VanillaProfession Rascal = new("Rascal", Farmer.scout, 5);

    /// <summary>The Brute profession, available at <see cref="VanillaSkill.Combat"/> level 10.</summary>
    public static readonly VanillaProfession Brute = new("Brute", Farmer.brute, 10);

    /// <summary>The Poacher profession, available at <see cref="VanillaSkill.Combat"/> level 10.</summary>
    public static readonly VanillaProfession Poacher = new("Poacher", Farmer.defender, 10);

    /// <summary>The Piper profession, available at <see cref="VanillaSkill.Combat"/> level 10.</summary>
    public static readonly VanillaProfession Piper = new("Piper", Farmer.acrobat, 10);

    /// <summary>The Desperado profession, available at <see cref="VanillaSkill.Combat"/> level 10.</summary>
    public static readonly VanillaProfession Desperado = new("Desperado", Farmer.desperado, 10);

    #endregion enum entries

    #region buff indices

    internal const int BruteRageSheetIndex = 36;
    internal const int DesperadoQuickshotSheetIndex = 39;
    internal const int SpelunkerStreakSheetIndex = 40;
    internal const int DemolitionistExcitedSheetIndex = 41;

    #endregion buff indices

    /// <summary>Initializes a new instance of the <see cref="VanillaProfession"/> class.</summary>
    /// <param name="name">The profession name.</param>
    /// <param name="value">The profession index.</param>
    /// <param name="level">The level at which the profession is offered (either 5 or 10).</param>
    private VanillaProfession(string name, int value, int level)
        : base(name, value)
    {
        this.Level = level;
        this.SourceSheetRect = new Rectangle((value % 6) * 16, (value / 6) * 16, 16, 16);
        this.TargetSheetRect = new Rectangle((value % 6) * 16, ((value / 6) * 16) + 624, 16, 16);
    }

    /// <inheritdoc />
    public string StringId => this.Name;

    /// <inheritdoc />
    public int Id => this.Value;

    /// <inheritdoc />
    public string Title => this.GetTitle(this.IsPrestiged);

    /// <inheritdoc />
    public string Description => this.GetDescription(this.IsPrestiged);

    /// <summary>Gets a <see cref="Rectangle"/> representing the coordinates of the <see cref="VanillaProfession"/>'s icon in the mod's Professions spritesheet.</summary>
    public Rectangle SourceSheetRect { get; }

    /// <summary>Gets a <see cref="Rectangle"/> representing the coordinates of the <see cref="VanillaProfession"/>'s icon in the vanilla Cursors spritesheet.</summary>
    public Rectangle TargetSheetRect { get; }

    /// <inheritdoc />
    public int Level { get; }

    /// <inheritdoc />
    public ISkill Skill => Professions.VanillaSkill.FromValue(this.Value / 6);

    /// <summary>Whether the local player has prestiged this <see cref="VanillaProfession"/>.</summary>
    public bool IsPrestiged => Game1.player.HasProfession(this, true);

    /// <summary>Gets the <see cref="VanillaProfession"/> with the specified localized name.</summary>
    /// <param name="name">A localized profession name.</param>
    /// <param name="ignoreCase">Whether to ignore capitalization.</param>
    /// <param name="result">The corresponding profession.</param>
    /// <returns><see langword="true"/> if a matching profession was found, otherwise <see langword="false"/>.</returns>
    public static bool TryFromLocalizedName(string name, bool ignoreCase, [NotNullWhen(true)] out VanillaProfession? result)
    {
        var stringComparison = ignoreCase
            ? StringComparison.InvariantCultureIgnoreCase
            : StringComparison.InvariantCulture;
        result = null;
        if (string.Equals(name, Rancher.Title.TrimAll(), stringComparison))
        {
            result = Rancher;
        }
        else if (string.Equals(name, Harvester.Title.TrimAll(), stringComparison))
        {
            result = Harvester;
        }
        else if (string.Equals(name, Breeder.Title.TrimAll(), stringComparison))
        {
            result = Breeder;
        }
        else if (string.Equals(name, Producer.Title.TrimAll(), stringComparison))
        {
            result = Producer;
        }
        else if (string.Equals(name, Artisan.Title.TrimAll(), stringComparison))
        {
            result = Artisan;
        }
        else if (string.Equals(name, Agriculturist.Title.TrimAll(), stringComparison))
        {
            result = Agriculturist;
        }
        else if (string.Equals(name, Fisher.Title.TrimAll(), stringComparison))
        {
            result = Fisher;
        }
        else if (string.Equals(name, Trapper.Title.TrimAll(), stringComparison))
        {
            result = Trapper;
        }
        else if (string.Equals(name, Angler.Title.TrimAll(), stringComparison))
        {
            result = Angler;
        }
        else if (string.Equals(name, Aquarist.Title.TrimAll(), stringComparison))
        {
            result = Aquarist;
        }
        else if (string.Equals(name, Luremaster.Title.TrimAll(), stringComparison))
        {
            result = Luremaster;
        }
        else if (string.Equals(name, Conservationist.Title.TrimAll(), stringComparison))
        {
            result = Conservationist;
        }
        else if (string.Equals(name, Lumberjack.Title.TrimAll(), stringComparison))
        {
            result = Lumberjack;
        }
        else if (string.Equals(name, Forager.Title.TrimAll(), stringComparison))
        {
            result = Forager;
        }
        else if (string.Equals(name, Arborist.Title.TrimAll(), stringComparison))
        {
            result = Arborist;
        }
        else if (string.Equals(name, Tapper.Title.TrimAll(), stringComparison))
        {
            result = Tapper;
        }
        else if (string.Equals(name, Ecologist.Title.TrimAll(), stringComparison))
        {
            result = Ecologist;
        }
        else if (string.Equals(name, Scavenger.Title.TrimAll(), stringComparison))
        {
            result = Scavenger;
        }
        else if (string.Equals(name, Miner.Title.TrimAll(), stringComparison))
        {
            result = Miner;
        }
        else if (string.Equals(name, Blaster.Title.TrimAll(), stringComparison))
        {
            result = Blaster;
        }
        else if (string.Equals(name, Spelunker.Title.TrimAll(), stringComparison))
        {
            result = Spelunker;
        }
        else if (string.Equals(name, Prospector.Title.TrimAll(), stringComparison))
        {
            result = Prospector;
        }
        else if (string.Equals(name, Demolitionist.Title.TrimAll(), stringComparison))
        {
            result = Demolitionist;
        }
        else if (string.Equals(name, Gemologist.Title.TrimAll(), stringComparison))
        {
            result = Gemologist;
        }
        else if (string.Equals(name, Fighter.Title.TrimAll(), stringComparison))
        {
            result = Fighter;
        }
        else if (string.Equals(name, Rascal.Title.TrimAll(), stringComparison))
        {
            result = Rascal;
        }
        else if (string.Equals(name, Brute.Title.TrimAll(), stringComparison))
        {
            result = Brute;
        }
        else if (string.Equals(name, Poacher.Title.TrimAll(), stringComparison))
        {
            result = Poacher;
        }
        else if (string.Equals(name, Piper.Title.TrimAll(), stringComparison))
        {
            result = Piper;
        }
        else if (string.Equals(name, Desperado.Title.TrimAll(), stringComparison))
        {
            result = Desperado;
        }

        return result is not null;
    }

    /// <summary>Enumerate the range of indices corresponding to all vanilla professions.</summary>
    /// <param name="prestige">Whether to enumerate prestige professions instead.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of all vanilla profession indices.</returns>
    public static IEnumerable<int> GetRange(bool prestige = false)
    {
        return Enumerable.Range(prestige ? 100 : 0, 30);
    }

    /// <summary>Gets the localized and gendered title for this profession.</summary>
    /// <param name="prestiged">Whether to get the prestiged or normal variant.</param>
    /// <returns>A human-readable <see cref="string"/> title for the profession.</returns>
    public string GetTitle(bool? prestiged = null)
    {
        prestiged ??= this.IsPrestiged;
        return LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en
            ? this.Level == 10
                ? _I18n.Get(this.Name.ToLower() + ".title." + (prestiged.Value ? "prestiged." : Empty) +
                            (Game1.player.IsMale ? "male" : "female"))
                : (prestiged.Value ? I18n.Prestiged_Title() : Empty) +
                  _I18n.Get(this.Name.ToLower() + ".title." + (Game1.player.IsMale ? "male" : "female"))
            : _I18n.Get(this.Name.ToLower() + ".title." + (Game1.player.IsMale ? "male" : "female"));
    }

    /// <summary>Gets the localized description text for this profession.</summary>
    /// <param name="prestiged">Whether to get the prestiged or normal variant.</param>
    /// <returns>A human-readable <see cref="string"/> description of the profession.</returns>
    public string GetDescription(bool prestiged = false)
    {
        return _I18n.Get(this.Name.ToLowerInvariant() + ".desc" + (prestiged ? ".prestiged" : Empty));
    }
}
