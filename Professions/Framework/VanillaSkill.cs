global using Skill = DaLion.Professions.Framework.VanillaSkill;

namespace DaLion.Professions.Framework;

#region using directives

using System.Collections.Generic;
using System.Linq;
using Ardalis.SmartEnum;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Constants;
using StardewValley.Menus;

#endregion using directives

/// <summary>Represents a vanilla skill.</summary>
public sealed class VanillaSkill : SmartEnum<Skill>, ISkill
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

    #endregion enum entries

    /// <summary>Initializes a new instance of the <see cref="Skill"/> class.</summary>
    /// <param name="name">The skill name.</param>
    /// <param name="value">The skill index.</param>
    private VanillaSkill(string name, int value)
        : base(name, value)
    {
        this.Professions = [];
        this.DisplayName = value switch
        {
            Farmer.farmingSkill => Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11604"),
            Farmer.fishingSkill => Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11607"),
            Farmer.foragingSkill => Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11606"),
            Farmer.miningSkill => Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11605"),
            Farmer.combatSkill => Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11608"),
            _ => string.Empty,
        };

        for (var i = 0; i < 6; i++)
        {
            this.Professions.Add(Profession.FromValue((value * 6) + i));
        }

        this.ProfessionPairByRoot[this.Professions[0]] =
            new ProfessionPair(this.Professions[2], this.Professions[3], this.Professions[0], 10);
        this.ProfessionPairByRoot[this.Professions[1]] =
            new ProfessionPair(this.Professions[4], this.Professions[5], this.Professions[1], 10);

        this.SourceSheetRect = new Rectangle(value * 10, 0, 10, 10);
        this.TargetSheetRect = value switch
        {
            Farmer.farmingSkill => new Rectangle(10, 428, 10, 10),
            Farmer.fishingSkill => new Rectangle(20, 428, 10, 10),
            Farmer.foragingSkill => new Rectangle(60, 428, 10, 10),
            Farmer.miningSkill => new Rectangle(30, 428, 10, 10),
            Farmer.combatSkill => new Rectangle(120, 428, 10, 10),
            _ => Rectangle.Empty,
        };
    }

    /// <inheritdoc />
    public string StringId => this.Name;

    /// <inheritdoc />
    public int Id => this.Value;

    /// <inheritdoc />
    public string DisplayName { get; protected set; }

    /// <inheritdoc />
    public int CurrentExp => Game1.player.experiencePoints[this.Value];

    /// <inheritdoc />
    public int CurrentLevel => Game1.player.GetUnmodifiedSkillLevel(this.Value);

    /// <inheritdoc />
    public int MaxLevel => this.CanGainPrestigeLevels() ? 20 : 10;

    /// <inheritdoc />
    public float BaseExperienceMultiplier => Config.Skills.BaseMultipliers[this.Name];

    /// <inheritdoc />
    public IEnumerable<int> NewLevels =>
        Game1.player.newLevels
            .Where(p => p.X == this.Value)
            .Select(p => p.Y);

    /// <inheritdoc />
    public IList<IProfession> Professions { get; }

    /// <inheritdoc />
    public IDictionary<IProfession, ProfessionPair> ProfessionPairByRoot { get; } = new Dictionary<IProfession, ProfessionPair>();

    /// <summary>Gets a <see cref="Rectangle"/> representing the coordinates of the <see cref="Skill"/>'s icon in the mod's Skills spritesheet.</summary>
    public Rectangle SourceSheetRect { get; }

    /// <summary>Gets a <see cref="Rectangle"/> representing the coordinates of the <see cref="Skill"/>'s icon in the vanilla Cursors spritesheet.</summary>
    public Rectangle TargetSheetRect { get; }

    /// <summary>Enumerates all skills in order by which they appear in the <see cref="SkillsPage"/> menu.</summary>
    /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Skill"/>.</returns>
    public static IEnumerable<Skill> OrderedBySkillsPage()
    {
        yield return Farming;
        yield return Mining;
        yield return Foraging;
        yield return Fishing;
        yield return Combat;
    }

    /// <inheritdoc />
    public bool Equals(ISkill? other)
    {
        return this.Id == other?.Id;
    }

    /// <inheritdoc />
    public void AddExperience(int amount)
    {
        Game1.player.experiencePoints[this] = Math.Min(this.CurrentExp + amount, ISkill.ExperienceCurve[this.MaxLevel]);
    }

    /// <inheritdoc />
    public void SetLevel(int level)
    {
        level = Math.Min(level, this.MaxLevel);
        var player = Game1.player;
        for (var l = this.CurrentLevel + 1; l <= level; l++)
        {
            var point = new Point(this.Value, l);
            if (!player.newLevels.Contains(point))
            {
                player.newLevels.Add(point);
            }
        }

        this
            .When(Farming).Then(() => player.farmingLevel.Value = level)
            .When(Fishing).Then(() => player.fishingLevel.Value = level)
            .When(Foraging).Then(() => player.foragingLevel.Value = level)
            .When(Mining).Then(() => player.miningLevel.Value = level)
            .When(Combat).Then(() => player.combatLevel.Value = level);
        player.experiencePoints[this] =
            Math.Max(Game1.player.experiencePoints[this], ISkill.ExperienceCurve[level]);
    }

    /// <inheritdoc />
    public bool CanReset()
    {
        return !this.CanGainPrestigeLevels() && ISkill.CanReset(this);
    }

    /// <inheritdoc />
    public void Reset()
    {
        var player = Game1.player;

        // reset skill level
        this
            .When(Farming).Then(() => player.farmingLevel.Value = 0)
            .When(Fishing).Then(() => player.fishingLevel.Value = 0)
            .When(Foraging).Then(() => player.foragingLevel.Value = 0)
            .When(Mining).Then(() => player.miningLevel.Value = 0)
            .When(Combat).Then(() => player.combatLevel.Value = 0);

        // reset new levels
        for (var i = 0; i < player.newLevels.Count; i++)
        {
            var level = player.newLevels[i];
            if (level.X == this.Value)
            {
                player.newLevels.Remove(level);
            }
        }

        // reset skill experience
        player.experiencePoints[this] = 0;

        // forget recipes
        if (Config.Skills.ForgetRecipesOnSkillReset)
        {
            this.ForgetRecipes();
        }

        // revalidate health if necessary
        if (this.Value == Farmer.combatSkill)
        {
            LevelUpMenu.RevalidateHealth(player);
        }

        // record data
        var resetData = Data.Read(player, DataKeys.ResetCountBySkill).ParseDictionary<string, int>();
        resetData.AddOrUpdate(this.StringId, 1, (a, b) => a + b);
        Data.Write(player, DataKeys.ResetCountBySkill, resetData.Stringify());
    }

    /// <inheritdoc />
    public void ForgetRecipes()
    {
        if (this.Value == Farmer.luckSkill)
        {
            return;
        }

        var player = Game1.player;
        var forgottenRecipesDict = Data.Read(player, DataKeys.ForgottenRecipesDict)
            .ParseDictionary<string, int>();

        var knownCookingRecipes =
            player.cookingRecipes.Keys.ToDictionary(
                key => key,
                key => player.cookingRecipes[key]);
        foreach (var (key, value) in CraftingRecipe.cookingRecipes)
        {
            if (!value.SplitWithoutAllocation('/')[3].Contains(this.StringId, StringComparison.Ordinal) ||
                !knownCookingRecipes.TryGetValue(key, value: out var recipe))
            {
                continue;
            }

            forgottenRecipesDict.AddOrUpdate(key, recipe, (a, b) => a + b);
            player.cookingRecipes.Remove(key);
        }

        var knownCraftingRecipes =
            player.craftingRecipes.Keys.ToDictionary(
                key => key,
                key => player.craftingRecipes[key]);
        foreach (var (key, value) in CraftingRecipe.craftingRecipes)
        {
            if (!value.SplitWithoutAllocation('/')[4].Contains(this.StringId, StringComparison.Ordinal) ||
                !knownCraftingRecipes.TryGetValue(key, out var recipe))
            {
                continue;
            }

            forgottenRecipesDict.AddOrUpdate(key, recipe, (a, b) => a + b);
            player.craftingRecipes.Remove(key);
        }

        Data.Write(player, DataKeys.ForgottenRecipesDict, forgottenRecipesDict.Stringify());
    }

    /// <summary>Gets whether this skill has been Mastered.</summary>
    /// <returns><see langword="true"/> if the local player has purchased Mastery for this skill, otherwise <see langword="false"/>.</returns>
    public bool IsMastered()
    {
        return Game1.player.stats.Get(StatKeys.Mastery(this)) > 0;
    }

    /// <inheritdoc />
    public bool CanGainPrestigeLevels()
    {
        return ShouldEnablePrestigeLevels && this.IsMastered();
    }

    /// <inheritdoc />
    public void Revalidate()
    {
        var maxLevel = this.MaxLevel;
        if (this.CurrentLevel > maxLevel)
        {
            this.SetLevel(this.MaxLevel);
        }

        if (this.CurrentLevel == maxLevel && this.CurrentExp > ISkill.ExperienceCurve[maxLevel])
        {
            this.SetExperience(ISkill.ExperienceCurve[maxLevel]);
            return;
        }

        var expectedLevel = 0;
        var l = 1;
        while (l <= maxLevel && this.CurrentExp >= ISkill.ExperienceCurve[l])
        {
            l++;
            expectedLevel++;
        }

        if (this.CurrentLevel == expectedLevel)
        {
            Log.D($"{this} skill's level has the expected value.");
            return;
        }

        var farmer = Game1.player;
        if (this.CurrentLevel < expectedLevel)
        {
            Log.W(
                $"{this} skill's ({this.CurrentLevel}) is below the expected value ({expectedLevel}). New levels will be added to correct the difference.");
            for (var levelUp = this.CurrentLevel + 1; levelUp <= expectedLevel; levelUp++)
            {
                var point = new Point(this, levelUp);
                if (!farmer.newLevels.Contains(point))
                {
                    farmer.newLevels.Add(point);
                }
            }
        }
        else
        {
            Log.W(
                $"{this} skill's current level ({this.CurrentLevel}) is above the expected value ({expectedLevel}). The current level and experience will be downgraded to correct the difference.");
            this.SetExperience(ISkill.ExperienceCurve[expectedLevel]);
        }

        this.SetLevel(expectedLevel);
    }

    /// <summary>Sets the experience points for this skill.</summary>
    /// <param name="experience">The new amount of experience points.</param>
    private void SetExperience(int experience)
    {
        Game1.player.experiencePoints[this] = experience;
    }
}
