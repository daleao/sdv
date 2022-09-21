namespace DaLion.Stardew.Professions.Framework;

#region using directives

using System.Collections.Generic;
using System.Linq;
using Ardalis.SmartEnum;

#endregion using directives

/// <summary>Represents a vanilla skill.</summary>
/// <remarks>
///     Despite including a <see cref="Ardalis.SmartEnum"/> entry for the Luck skill, that skill is treated specially
///     by its own implementation (see <see cref="LuckSkill"/>).
/// </remarks>
public class Skill : SmartEnum<Skill>, ISkill
{
    #region enum entries

    /// <summary>The Farming skill.</summary>
    public static readonly Skill Farming = new("Farming", Farmer.farmingSkill);

    /// <summary>The Fishing skill.</summary>
    public static readonly Skill Fishing = new("Fishing", Farmer.fishingSkill);

    /// <summary>The Foraging skill.</summary>
    public static readonly Skill Foraging = new("Foraging", Farmer.foragingSkill);

    /// <summary>The Mining skill.</summary>
    public static readonly Skill Mining = new("Mining", Farmer.miningSkill);

    /// <summary>The Combat skill.</summary>
    public static readonly Skill Combat = new("Combat", Farmer.combatSkill);

    /// <summary>The Luck skill, if installed.</summary>
    public static readonly Skill Luck = new LuckSkill(ModEntry.LuckSkillApi);

    #endregion enum entries

    /// <summary>Initializes a new instance of the <see cref="Skill"/> class.</summary>
    /// <param name="name">The skill name.</param>
    /// <param name="value">The skill index.</param>
    protected Skill(string name, int value)
        : base(name, value)
    {
        if (value == Farmer.luckSkill)
        {
            this.StringId = null!;
            this.DisplayName = null!;
            return;
        }

        this.StringId = this.Name;
        this.DisplayName = value switch
        {
            Farmer.farmingSkill => Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11604"),
            Farmer.fishingSkill => Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11607"),
            Farmer.foragingSkill => Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11606"),
            Farmer.miningSkill => Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11605"),
            Farmer.combatSkill => Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11608"),
            _ => string.Empty,
        };

        foreach (var pid in Enumerable.Range(value * 6, 6))
        {
            this.Professions.Add(Profession.FromValue(pid));
        }

        this.ProfessionPairs[-1] = new ProfessionPair(this.Professions[0], this.Professions[1], null, 5);
        this.ProfessionPairs[this.Professions[0].Id] =
            new ProfessionPair(this.Professions[2], this.Professions[3], this.Professions[0], 10);
        this.ProfessionPairs[this.Professions[1].Id] =
            new ProfessionPair(this.Professions[4], this.Professions[5], this.Professions[1], 10);
    }

    /// <inheritdoc />
    public string StringId { get; protected set; }

    /// <inheritdoc />
    public string DisplayName { get; protected set; }

    /// <inheritdoc />
    public int CurrentExp => Game1.player.experiencePoints[this.Value];

    /// <inheritdoc />
    public int CurrentLevel => Game1.player.GetUnmodifiedSkillLevel(this.Value);

    /// <inheritdoc />
    public float BaseExperienceMultiplier => ModEntry.Config.BaseSkillExpMultipliers[this.Value];

    /// <inheritdoc />
    public IEnumerable<int> NewLevels => Game1.player.newLevels.Where(p => p.X == this.Value).Select(p => p.Y);

    /// <inheritdoc />
    public IList<IProfession> Professions { get; } = new List<IProfession>();

    /// <inheritdoc />
    public IDictionary<int, ProfessionPair> ProfessionPairs { get; } = new Dictionary<int, ProfessionPair>();

    /// <summary>Get the range of indices corresponding to vanilla skills.</summary>
    /// <returns>A <see cref="IEnumerable{T}"/> of all vanilla skill indices.</returns>
    public static IEnumerable<int> GetRange()
    {
        return Enumerable.Range(0, 5);
    }
}
