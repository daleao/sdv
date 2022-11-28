namespace DaLion.Ligo.Modules.Professions;

#region using directives

using System.Collections.Generic;
using System.Linq;
using Ardalis.SmartEnum;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;

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

    /// <summary>The Luck skill, if loaded.</summary>
    public static readonly Skill Luck = new LuckSkill();

    #endregion enum entries

    /// <summary>Initializes a new instance of the <see cref="Skill"/> class.</summary>
    /// <param name="name">The skill name.</param>
    /// <param name="value">The skill index.</param>
    protected Skill(string name, int value)
        : base(name, value)
    {
        if (value == Farmer.luckSkill)
        {
            this.StringId = null!; // set in child class
            this.DisplayName = null!; // set in child class
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
    public virtual int MaxLevel => ModEntry.Config.Professions.EnablePrestige && ((ISkill)this).PrestigeLevel >= 4 ? 20 : 10;

    /// <inheritdoc />
    public float BaseExperienceMultiplier => ModEntry.Config.Professions.BaseSkillExpMultipliers[this.Value];

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

    /// <inheritdoc />
    public void AddExperience(int amount)
    {
        Game1.player.experiencePoints[this] = Math.Min(this.CurrentExp + amount, ISkill.ExperienceByLevel[this.MaxLevel]);
    }

    /// <summary>Sets the experience points for this skill.</summary>
    /// <param name="experience">The new amount of experience points.</param>
    public void SetExperience(int experience)
    {
        Game1.player.experiencePoints[this] = experience;
    }

    /// <inheritdoc />
    public void SetLevel(int level)
    {
        level = Math.Min(level, this.MaxLevel);
        var farmer = Game1.player;
        this
            .When(Farming).Then(() => farmer.farmingLevel.Value = level)
            .When(Fishing).Then(() => farmer.fishingLevel.Value = level)
            .When(Foraging).Then(() => farmer.foragingLevel.Value = level)
            .When(Mining).Then(() => farmer.miningLevel.Value = level)
            .When(Combat).Then(() => farmer.combatLevel.Value = level)
            .When(Luck).Then(() => farmer.luckLevel.Value = level);
    }

    /// <inheritdoc />
    public void Reset()
    {
        var farmer = Game1.player;

        // reset skill level
        this
            .When(Farming).Then(() => farmer.farmingLevel.Value = 0)
            .When(Fishing).Then(() => farmer.fishingLevel.Value = 0)
            .When(Foraging).Then(() => farmer.foragingLevel.Value = 0)
            .When(Mining).Then(() => farmer.miningLevel.Value = 0)
            .When(Combat).Then(() => farmer.combatLevel.Value = 0)
            .When(Luck).Then(() => farmer.luckLevel.Value = 0);

        // reset new levels
        var toRemove = farmer.newLevels.Where(p => p.X == this);
        foreach (var item in toRemove)
        {
            farmer.newLevels.Remove(item);
        }

        // reset skill experience
        farmer.experiencePoints[this] = 0;

        // forget recipes
        if (ModEntry.Config.Professions.ForgetRecipes && this < Luck)
        {
            this.ForgetRecipes();
        }

        // revalidate health
        if (this == Farmer.combatSkill)
        {
            LevelUpMenu.RevalidateHealth(farmer);
        }
    }

    /// <inheritdoc />
    public void ForgetRecipes(bool saveForRecovery = true)
    {
        if (this == Farmer.luckSkill)
        {
            return;
        }

        var farmer = Game1.player;
        var forgottenRecipesDict = farmer.Read(DataFields.ForgottenRecipesDict)
            .ParseDictionary<string, int>();

        // remove associated crafting recipes
        var craftingRecipes =
            farmer.craftingRecipes.Keys.ToDictionary(
                key => key,
                key => farmer.craftingRecipes[key]);
        foreach (var (key, value) in CraftingRecipe.craftingRecipes)
        {
            if (!value.Split('/')[4].Contains(this.StringId) || !craftingRecipes.ContainsKey(key))
            {
                continue;
            }

            if (saveForRecovery)
            {
                if (!forgottenRecipesDict.TryAdd(key, craftingRecipes[key]))
                {
                    forgottenRecipesDict[key] += craftingRecipes[key];
                }
            }

            farmer.craftingRecipes.Remove(key);
        }

        // remove associated cooking recipes
        var cookingRecipes =
            farmer.cookingRecipes.Keys.ToDictionary(
                key => key,
                key => farmer.cookingRecipes[key]);
        foreach (var (key, value) in CraftingRecipe.cookingRecipes)
        {
            if (!value.Split('/')[3].Contains(this.StringId) || !cookingRecipes.ContainsKey(key))
            {
                continue;
            }

            if (saveForRecovery)
            {
                if (!forgottenRecipesDict.TryAdd(key, cookingRecipes[key]))
                {
                    forgottenRecipesDict[key] += cookingRecipes[key];
                }
            }

            farmer.cookingRecipes.Remove(key);
        }

        if (saveForRecovery)
        {
            farmer.Write(DataFields.ForgottenRecipesDict, forgottenRecipesDict.Stringify());
        }
    }

    /// <inheritdoc />
    public virtual void Revalidate()
    {
        if (this.CurrentLevel > this.MaxLevel)
        {
            this.SetLevel(this.MaxLevel);
        }

        if (this.CurrentLevel == this.MaxLevel && this.CurrentExp > ISkill.ExperienceByLevel[this.MaxLevel])
        {
            this.SetExperience(ISkill.ExperienceByLevel[this.MaxLevel]);
            return;
        }

        var expectedLevel = 0;
        var level = 1;
        while (level <= this.MaxLevel && this.CurrentExp >= ISkill.ExperienceByLevel[level++])
        {
            expectedLevel++;
        }

        if (this.CurrentLevel == expectedLevel)
        {
            return;
        }

        var farmer = Game1.player;
        if (this.CurrentLevel < expectedLevel)
        {
            for (var levelUp = this.CurrentLevel + 1; levelUp <= expectedLevel; levelUp++)
            {
                var point = new Point(this, levelUp);
                if (!farmer.newLevels.Contains(point))
                {
                    farmer.newLevels.Add(point);
                }
            }

            this.SetLevel(expectedLevel);
        }
        else
        {
            this.SetExperience(ISkill.ExperienceByLevel[expectedLevel]);
        }
    }
}
