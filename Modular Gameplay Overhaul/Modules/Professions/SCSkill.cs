namespace DaLion.Overhaul.Modules.Professions;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Overhaul.Modules.Professions.Integrations;
using DaLion.Shared.Classes;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using StardewValley;

#endregion using directives

/// <summary>Represents a SpaceCore-provided custom skill.</summary>
// ReSharper disable once InconsistentNaming
public sealed class SCSkill : ISkill
{
    private static readonly BiMap<SCSkill, SpaceCore.Skills.Skill> SpaceCoreMap = new();

    /// <summary>Initializes a new instance of the <see cref="SCSkill"/> class.</summary>
    /// <param name="id">The unique id of skill.</param>
    internal SCSkill(string id)
    {
        this.StringId = id;

        var skill = SpaceCore.Skills.GetSkill(id);
        this.DisplayName = skill.GetName();
        if (skill.Professions.Count != 6)
        {
            ThrowHelper.ThrowInvalidOperationException(
                $"The custom skill {id} did not provide the expected number of professions.");
        }

        for (var i = 0; i < 6; i++)
        {
            this.Professions.Add(new SCProfession(skill.Professions[i], i < 2 ? 5 : 10, this));
        }

        this.ProfessionPairs[-1] = new ProfessionPair(this.Professions[0], this.Professions[1], null, 5);
        this.ProfessionPairs[this.Professions[0].Id] =
            new ProfessionPair(this.Professions[2], this.Professions[3], this.Professions[0], 10);
        this.ProfessionPairs[this.Professions[1].Id] =
            new ProfessionPair(this.Professions[4], this.Professions[5], this.Professions[1], 10);

        for (var i = 0; i < 6; i++)
        {
            var profession = this.Professions[i];
            SCProfession.Loaded[profession.Id] = (SCProfession)profession;
        }

        SpaceCoreMap.TryAdd(this, skill);
    }

    /// <inheritdoc />
    public string StringId { get; }

    /// <inheritdoc />
    public string DisplayName { get; }

    /// <inheritdoc />
    public int CurrentExp => SpaceCore.Skills.GetExperienceFor(Game1.player, this.StringId);

    /// <inheritdoc />
    public int CurrentLevel => SpaceCore.Skills.GetSkillLevel(Game1.player, this.StringId);

    /// <inheritdoc />
    public int MaxLevel =>
        this.CanPrestige && ProfessionsModule.Config.EnablePrestige && ((ISkill)this).PrestigeLevel >= 4 ? 20 : 10;

    /// <inheritdoc />
    public float BaseExperienceMultiplier =>
        ProfessionsModule.Config.CustomSkillExpMultipliers.TryGetValue(this.StringId, out var multiplier)
            ? multiplier
            : 1f;

    /// <inheritdoc />
    public IEnumerable<int> NewLevels => Reflector
        .GetStaticFieldGetter<List<KeyValuePair<string, int>>>(typeof(SpaceCore.Skills), "NewLevels")
        .Invoke()
        .Where(pair => pair.Key == this.StringId)
        .Select(pair => pair.Value);

    /// <inheritdoc />
    public IList<IProfession> Professions { get; } = new List<IProfession>();

    /// <inheritdoc />
    public IDictionary<int, ProfessionPair> ProfessionPairs { get; } = new Dictionary<int, ProfessionPair>();

    /// <summary>Gets the currently loaded <see cref="SCSkill"/>s.</summary>
    /// <remarks>The value type is <see cref="ISkill"/> because this also includes <see cref="LuckSkill"/>, which is not a SpaceCore skill.</remarks>
    internal static Dictionary<string, ISkill> Loaded { get; } = new();

    /// <summary>Gets or sets a value indicating whether this skill can gain prestige levels.</summary>
    internal bool CanPrestige { get; set; } = false;

    /// <summary>Gets the <see cref="SCSkill"/> equivalent to the specified <see cref="SpaceCore.Skills.Skill"/>.</summary>
    /// <param name="skill">The <see cref="SpaceCore.Skills.Skill"/>.</param>
    /// <returns>The equivalent <see cref="SCSkill"/>.</returns>
    public static SCSkill? FromSpaceCore(SpaceCore.Skills.Skill skill)
    {
        return SpaceCoreMap.TryGetReverse(skill, out var scProfession) ? scProfession : null;
    }

    /// <summary>Gets the <see cref="SpaceCore.Skills.Skill"/> equivalent to this <see cref="SCSkill"/>.</summary>
    /// <returns>The equivalent <see cref="SpaceCore.Skills.Skill"/>.</returns>
    public SpaceCore.Skills.Skill? ToSpaceCore()
    {
        return SpaceCoreMap.TryGetForward(this, out var skill) ? skill : null;
    }

    /// <inheritdoc />
    public void AddExperience(int amount)
    {
        SpaceCore.Skills.AddExperience(Game1.player, this.StringId, amount);
    }

    /// <inheritdoc />
    public void SetLevel(int level)
    {
        level = Math.Min(level, this.CanPrestige ? 20 : 10);
        var diff = ISkill.ExperienceByLevel[level] - this.CurrentExp;
        this.AddExperience(diff);
    }

    /// <inheritdoc />
    public void Reset()
    {
        // reset skill level and experience
        this.AddExperience(-this.CurrentExp);

        // reset new levels
        var newLevels = Reflector
            .GetStaticFieldGetter<List<KeyValuePair<string, int>>>(typeof(SpaceCore.Skills), "NewLevels")
            .Invoke();
        Reflector
            .GetStaticFieldSetter<List<KeyValuePair<string, int>>>(typeof(SpaceCore.Skills), "NewLevels")
            .Invoke(newLevels.Where(pair => pair.Key != this.StringId).ToList());

        // reset recipes
        if (ProfessionsModule.Config.ForgetRecipes && this.StringId == "blueberry.LoveOfCooking.CookingSkill")
        {
            this.ForgetRecipes();
        }

        Log.D($"[Prestige]: {Game1.player.Name}'s {this.DisplayName} skill has been reset.");
    }

    /// <inheritdoc />
    public void ForgetRecipes(bool saveForRecovery = true)
    {
        if (this.StringId != "blueberry.LoveOfCooking.CookingSkill" || LoveOfCookingIntegration.Instance?.IsLoaded != true)
        {
            return;
        }

        var farmer = Game1.player;
        var forgottenRecipesDict = farmer.Read(DataFields.ForgottenRecipesDict)
            .ParseDictionary<string, int>();

        // remove associated cooking recipes
        var cookingRecipes = LoveOfCookingIntegration.Instance.ModApi!
            .GetAllLevelUpRecipes().Values
            .SelectMany(r => r)
            .Select(r => "blueberry.cac." + r)
            .ToHashSet();
        var knownCookingRecipes = farmer.cookingRecipes.Keys
            .Where(key => cookingRecipes.Contains(key))
            .ToDictionary(key => key, key => farmer.cookingRecipes[key]);
        foreach (var (key, value) in knownCookingRecipes)
        {
            if (saveForRecovery && !forgottenRecipesDict.TryAdd(key, value))
            {
                forgottenRecipesDict[key] += value;
            }

            farmer.cookingRecipes.Remove(key);
        }

        if (saveForRecovery)
        {
            farmer.Write(DataFields.ForgottenRecipesDict, forgottenRecipesDict.Stringify());
        }
    }

    /// <inheritdoc />
    public void Revalidate()
    {
        var currentExp = this.CurrentExp;
        if (currentExp > Constants.ExpAtLevel10)
        {
            this.AddExperience(Constants.ExpAtLevel10 - currentExp);
        }
    }
}
