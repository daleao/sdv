global using SCSkill = SpaceCore.Skills.Skill;
global using SCSkills = SpaceCore.Skills;

namespace DaLion.Professions.Framework;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Professions.Framework.Integrations;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using SpaceCore.Interface;

#endregion using directives

/// <summary>Represents a SpaceCore-provided custom skill.</summary>
// ReSharper disable once InconsistentNaming
public sealed class CustomSkill : ISkill
{
    private static readonly Dictionary<SCSkill, CustomSkill> FromSpaceCore = [];

    private readonly SCSkill _scSkill;

    /// <summary>Initializes a new instance of the <see cref="CustomSkill"/> class.</summary>
    /// <param name="id">The unique id of skill.</param>
    private CustomSkill(string id)
    {
        this.StringId = id;
        this._scSkill = SCSkills.GetSkill(id);
        this.DisplayName = this._scSkill.GetName();
        if (this._scSkill.Professions.Count != 6)
        {
            ThrowHelper.ThrowInvalidOperationException(
                $"The custom skill {id} did not provide the expected number of professions.");
        }

        for (var i = 0; i < 6; i++)
        {
            this.Professions.Add(new CustomProfession(this._scSkill.Professions[i], i < 2 ? 5 : 10, this));
        }

        this.ProfessionPairByRoot[this.Professions[0]] =
            new ProfessionPair(this.Professions[2], this.Professions[3], this.Professions[0], 10);
        this.ProfessionPairByRoot[this.Professions[1]] =
            new ProfessionPair(this.Professions[4], this.Professions[5], this.Professions[1], 10);

        this._scSkill.ExperienceCurve = ISkill.ExperienceCurve.Skip(1).ToArray();
        Loaded[id] = this;
        FromSpaceCore[this._scSkill] = this;
        Log.D($"Successfully initialized custom skill {id}");
    }

    /// <summary>Initializes a new instance of the <see cref="CustomSkill"/> class.</summary>
    /// <param name="scSkill">The equivalent <see cref="SCSkill"/>.</param>
    private CustomSkill(SCSkill scSkill)
    {
        this._scSkill = scSkill;
        this.StringId = scSkill.Id;
        this.DisplayName = scSkill.GetName();
        if (scSkill.Professions.Count != 6)
        {
            ThrowHelper.ThrowInvalidOperationException(
                $"The custom skill {scSkill.Id} did not provide the expected number of professions.");
        }

        for (var i = 0; i < 6; i++)
        {
            this.Professions.Add(new CustomProfession(scSkill.Professions[i], i < 2 ? 5 : 10, this));
        }

        this.ProfessionPairByRoot[this.Professions[0]] =
            new ProfessionPair(this.Professions[2], this.Professions[3], this.Professions[0], 10);
        this.ProfessionPairByRoot[this.Professions[1]] =
            new ProfessionPair(this.Professions[4], this.Professions[5], this.Professions[1], 10);

        scSkill.ExperienceCurve = ISkill.ExperienceCurve.Skip(1).ToArray();
        Loaded[this.StringId] = this;
        FromSpaceCore[scSkill] = this;
        Log.D($"Successfully initialized custom skill {this.StringId}");
    }

    /// <inheritdoc />
    public string StringId { get; }

    /// <inheritdoc />
    public int Id => Loaded.Keys.ToList().IndexOf(this.StringId);

    /// <inheritdoc />
    public string DisplayName { get; }

    /// <inheritdoc />
    public int CurrentExp => SCSkills.GetExperienceFor(Game1.player, this.StringId);

    /// <inheritdoc />
    public int CurrentLevel => SCSkills.GetSkillLevel(Game1.player, this.StringId);

    /// <inheritdoc />
    public int MaxLevel => this.CanGainPrestigeLevels() ? 20 : 10;

    /// <inheritdoc />
    public float BaseExperienceMultiplier => Config.Skills.BaseMultipliers.GetValueOrDefault(this.StringId, 1f);

    /// <inheritdoc />
    public IEnumerable<int> NewLevels => SpaceCoreIntegration.Instance!
        .GetNewLevels()
        .Where(pair => pair.Key == this.StringId)
        .Select(pair => pair.Value);

    /// <inheritdoc />
    public IList<IProfession> Professions { get; } = [];

    /// <inheritdoc />
    public IDictionary<IProfession, ProfessionPair> ProfessionPairByRoot { get; } = new Dictionary<IProfession, ProfessionPair>();

    /// <summary>Gets the currently loaded <see cref="CustomSkill"/>s.</summary>
    internal static Dictionary<string, CustomSkill> Loaded { get; } = [];

    /// <summary>Gets the <see cref="CustomSkill"/> equivalent to the specified <see cref="SCSkill"/>.</summary>
    /// <param name="scSkill">The <see cref="SCSkill"/>.</param>
    /// <returns>The equivalent <see cref="CustomSkill"/>.</returns>
    public static CustomSkill? GetFromSpaceCore(SCSkill scSkill)
    {
        return FromSpaceCore.GetValueOrDefault(scSkill);
    }

    /// <inheritdoc />
    public bool Equals(ISkill? other)
    {
        return this.Id == other?.Id;
    }

    /// <inheritdoc />
    public void AddExperience(int amount)
    {
        SCSkills.AddExperience(Game1.player, this.StringId, amount);
    }

    /// <inheritdoc />
    public void SetLevel(int level)
    {
        level = Math.Min(level, this.MaxLevel);
        var diff = ISkill.ExperienceCurve[level] - this.CurrentExp;
        this.AddExperience(diff);
    }

    /// <inheritdoc />
    public void Reset()
    {
        // reset skill level and experience
        this.AddExperience(-this.CurrentExp);

        // reset new levels
        var newLevels = SpaceCoreIntegration.Instance!.GetNewLevels();
        SpaceCoreIntegration.Instance.SetNewLevels(newLevels.Where(pair => pair.Key != this.StringId).ToList());

        // reset recipes
        if (Config.Skills.ForgetRecipesOnSkillReset)
        {
            this.ForgetRecipes();
        }

        // record data
        var resetData = Data.Read(Game1.player, DataKeys.ResetCountBySkill).ParseDictionary<string, int>();
        resetData.AddOrUpdate(this.StringId, 1, (a, b) => a + b);
        Data.Write(Game1.player, DataKeys.ResetCountBySkill, resetData.Stringify());

        Log.D($"{Game1.player.Name}'s {this.DisplayName} skill has been reset.");
    }

    /// <inheritdoc />
    public void ForgetRecipes()
    {
        var player = Game1.player;
        var forgottenRecipesDict = Data.Read(player, DataKeys.ForgottenRecipesDict)
            .ParseDictionary<string, int>();

        HashSet<CraftingRecipe> cookingRecipes = [];
        for (var level = 1; level <= 20; level++)
        {
            cookingRecipes.UnionWith(SkillLevelUpMenu.GetCookingRecipesForLevel(this.StringId, level));
        }

        foreach (var recipe in cookingRecipes)
        {
            var name = recipe.name;
            if (!player.cookingRecipes.TryGetValue(name, out var numberCooked))
            {
                continue;
            }

            forgottenRecipesDict.AddOrUpdate(name, numberCooked, (a, b) => a + b);
            player.cookingRecipes.Remove(name);
        }

        HashSet<CraftingRecipe> craftingRecipes = [];
        for (var level = 1; level <= 20; level++)
        {
            craftingRecipes.UnionWith(SkillLevelUpMenu.GetCraftingRecipesForLevel(this.StringId, level));
        }

        foreach (var recipe in craftingRecipes)
        {
            var name = recipe.name;
            if (!player.craftingRecipes.TryGetValue(name, out var numberCrafted))
            {
                continue;
            }

            forgottenRecipesDict.AddOrUpdate(name, numberCrafted, (a, b) => a + b);
            player.craftingRecipes.Remove(name);
        }

        Data.Write(player, DataKeys.ForgottenRecipesDict, forgottenRecipesDict.Stringify());
    }

    /// <inheritdoc />
    public bool CanGainPrestigeLevels()
    {
        return false;
    }

    /// <inheritdoc />
    public void Revalidate()
    {
        var currentExp = this.CurrentExp;
        if (currentExp > ISkill.LEVEL_10_EXP)
        {
            this.AddExperience(ISkill.LEVEL_10_EXP - currentExp);
        }
    }

    /// <summary>Gets the <see cref="SCSkill"/> equivalent to this <see cref="CustomSkill"/>.</summary>
    /// <returns>The equivalent <see cref="SCSkill"/>.</returns>
    public SCSkill ToSpaceCore()
    {
        return this._scSkill;
    }

    /// <summary>Initializes the custom SpaceCore skill with specified <see cref="string"/> <paramref name="id"/>.</summary>
    /// <param name="id">The <see cref="string"/> ID of the custom SpaceCore skill.</param>
    internal static void Initialize(string id)
    {
        _ = new CustomSkill(id);
    }

    /// <summary>Initializes the custom SpaceCore skill from the <see cref="SCSkill"/> instance of the custom SpaceCore skill.</summary>
    /// <param name="scSkill">The <see cref="SCSkill"/> instance the custom SpaceCore skill.</param>
    internal static void Initialize(SCSkill scSkill)
    {
        _ = new CustomSkill(scSkill);
    }
}
