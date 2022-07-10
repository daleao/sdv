namespace DaLion.Stardew.Professions.Framework;

#region using directives

using Ardalis.SmartEnum;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Represents a vanilla skill.</summary>
/// <remarks>Despite including a <see cref="SmartEnum"/> entry for the Luck skill, that skill is treated specially by its own implementation (see <see cref="LuckSkill"/>).</remarks>
public class Skill : SmartEnum<Skill>, ISkill
{
    #region enum entries

    public static readonly Skill Farming = new("Farming", Farmer.farmingSkill);
    public static readonly Skill Fishing = new("Fishing", Farmer.fishingSkill);
    public static readonly Skill Foraging = new("Foraging", Farmer.foragingSkill);
    public static readonly Skill Mining = new("Mining", Farmer.miningSkill);
    public static readonly Skill Combat = new("Combat", Farmer.combatSkill);

    public static readonly Skill Luck = new LuckSkill(ModEntry.LuckSkillApi);

    #endregion enum entries

    /// <inheritdoc />
    public string StringId { get; protected set; }

    /// <inheritdoc />
    public string DisplayName { get; protected set; }

    /// <inheritdoc />
    public int CurrentExp => Game1.player.experiencePoints[Value];

    /// <inheritdoc />
    public int CurrentLevel => Game1.player.GetUnmodifiedSkillLevel(Value);

    /// <inheritdoc />
    public IEnumerable<int> NewLevels => Game1.player.newLevels.Where(p => p.X == Value).Select(p => p.Y);

    /// <inheritdoc />
    public IList<IProfession> Professions { get; } = new List<IProfession>();

    /// <inheritdoc />
    public IDictionary<int, ProfessionPair> ProfessionPairs { get; } = new Dictionary<int, ProfessionPair>();

    /// <summary>Construct an instance.</summary>
    /// <param name="name">The skill name.</param>
    /// <param name="value">The skill index.</param>
    protected Skill(string name, int value) : base(name, value)
    {
        if (value == Farmer.luckSkill)
        {
            StringId = null!;
            DisplayName = null!;
            return;
        }

        StringId = Name;
#pragma warning disable CS8509
        DisplayName = value switch
#pragma warning restore CS8509
        {
            Farmer.farmingSkill => Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11604"),
            Farmer.fishingSkill => Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11607"),
            Farmer.foragingSkill => Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11606"),
            Farmer.miningSkill => Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11605"),
            Farmer.combatSkill => Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11608"),
        };

        foreach (var pid in Enumerable.Range(value * 6, 6))
            Professions.Add(Profession.FromValue(pid));
        ProfessionPairs[-1] = new(Professions[0], Professions[1], null, 5);
        ProfessionPairs[Professions[0].Id] = new(Professions[2], Professions[3], Professions[0], 10);
        ProfessionPairs[Professions[1].Id] = new(Professions[4], Professions[5], Professions[1], 10);

    }

    /// <summary>Get the range of indices corresponding to vanilla professions.</summary>
    public static IEnumerable<int> GetRange() => Enumerable.Range(0, 5);
}