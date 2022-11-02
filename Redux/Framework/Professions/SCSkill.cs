namespace DaLion.Redux.Framework.Professions;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using StardewValley;
using static SpaceCore.Skills;

#endregion using directives

/// <summary>Represents a SpaceCore-provided custom skill.</summary>
public sealed class SCSkill : ISkill
{
    /// <summary>Initializes a new instance of the <see cref="SCSkill"/> class.</summary>
    /// <param name="id">The unique id of skill.</param>
    internal SCSkill(string id)
    {
        this.StringId = id;

        var instance = GetSkill(id);
        this.DisplayName = instance.GetName();
        var i = 0;
        foreach (var profession in instance.Professions)
        {
            this.Professions.Add(
                new SCProfession(
                    profession.GetVanillaId(),
                    profession.Id,
                    profession.GetName,
                    profession.GetDescription,
                    null,
                    i++ < 2 ? 5 : 10,
                    this));
        }

        if (this.Professions.Count != 6)
        {
            ThrowHelper.ThrowInvalidOperationException(
                $"The custom skill {id} did not provide the expected number of professions.");
        }

        this.ProfessionPairs[-1] = new ProfessionPair(this.Professions[0], this.Professions[1], null, 5);
        this.ProfessionPairs[this.Professions[0].Id] =
            new ProfessionPair(this.Professions[2], this.Professions[3], this.Professions[0], 10);
        this.ProfessionPairs[this.Professions[1].Id] =
            new ProfessionPair(this.Professions[4], this.Professions[5], this.Professions[1], 10);
    }

    /// <inheritdoc />
    public string StringId { get; }

    /// <inheritdoc />
    public string DisplayName { get; }

    /// <inheritdoc />
    public int CurrentExp => GetExperienceFor(Game1.player, this.StringId);

    /// <inheritdoc />
    public int CurrentLevel => GetSkillLevel(Game1.player, this.StringId);

    /// <inheritdoc />
    public int MaxLevel =>
        this.CanPrestige && ModEntry.Config.Professions.EnablePrestige && ((ISkill)this).PrestigeLevel >= 4 ? 20 : 10;

    /// <inheritdoc />
    public float BaseExperienceMultiplier =>
        ModEntry.Config.Professions.CustomSkillExpMultipliers.TryGetValue(this.StringId, out var multiplier)
            ? multiplier
            : 1f;

    /// <inheritdoc />
    public IEnumerable<int> NewLevels => ModEntry.Reflector
        .GetStaticFieldGetter<List<KeyValuePair<string, int>>>(typeof(SpaceCore.Skills), "NewLevels")
        .Invoke()
        .Where(pair => pair.Key == this.StringId).Select(pair => pair.Value);

    /// <inheritdoc />
    public IList<IProfession> Professions { get; } = new List<IProfession>();

    /// <inheritdoc />
    public IDictionary<int, ProfessionPair> ProfessionPairs { get; } = new Dictionary<int, ProfessionPair>();

    /// <summary>Gets the currently loaded <see cref="SCSkill"/>s.</summary>
    /// <remarks>The value type is <see cref="ISkill"/> because this also includes <see cref="LuckSkill"/>, which is not a SpaceCore skill.</remarks>
    internal static Dictionary<string, ISkill> Loaded { get; } = new();

    /// <summary>Gets or sets a value indicating whether this skill can gain prestige levels.</summary>
    internal bool CanPrestige { get; set; } = false;

    /// <inheritdoc />
    public void AddExperience(int amount)
    {
        SpaceCore.Skills.AddExperience(Game1.player, this.StringId, amount);
    }

    /// <inheritdoc />
    public void SetLevel(int level)
    {
        level = Math.Min(level, 10);
        var diff = ISkill.ExperienceByLevel[level] - this.CurrentExp;
        this.AddExperience(diff);
    }

    /// <inheritdoc />
    public void Reset()
    {
        var farmer = Game1.player;

        // reset skill level and experience
        this.AddExperience(-this.CurrentExp);

        // reset new levels
        var newLevels = ModEntry.Reflector
            .GetStaticFieldGetter<List<KeyValuePair<string, int>>>(typeof(SpaceCore.Skills), "NewLevels")
            .Invoke();
        ModEntry.Reflector
            .GetStaticFieldSetter<List<KeyValuePair<string, int>>>(typeof(SpaceCore.Skills), "NewLevels")
            .Invoke(newLevels.Where(pair => pair.Key != this.StringId).ToList());

        // reset recipes
        if (ModEntry.Config.Professions.ForgetRecipes && this.StringId == "blueberry.LoveOfCooking.CookingSkill")
        {
            this.ForgetRecipes();
        }

        Log.D($"{farmer.Name}'s {this.DisplayName} skill has been reset.");
    }

    /// <inheritdoc />
    public void ForgetRecipes(bool saveForRecovery = true)
    {
        if (this.StringId != "blueberry.LoveOfCooking.CookingSkill")
        {
            return;
        }

        var farmer = Game1.player;
        var forgottenRecipesDict = farmer.Read(DataFields.ForgottenRecipesDict)
            .ParseDictionary<string, int>();

        // remove associated cooking recipes
        var cookingRecipes = Framework.Integrations.CookingSkillApi!
            .GetAllLevelUpRecipes().Values
            .SelectMany(r => r)
            .Select(r => "blueberry.cac." + r)
            .ToList();
        var knownCookingRecipes = farmer.cookingRecipes.Keys.Where(key => key.IsIn(cookingRecipes)).ToDictionary(
            key => key,
            key => farmer.cookingRecipes[key]);
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
