namespace DaLion.Overhaul.Modules.Professions;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Integrations;
using DaLion.Shared.Classes;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using StardewValley;
using SCSkill = SpaceCore.Skills.Skill;

#endregion using directives

/// <summary>Represents a SpaceCore-provided custom skill.</summary>
// ReSharper disable once InconsistentNaming
public sealed class CustomSkill : ISkill
{
    private static readonly BiMap<CustomSkill, SCSkill> SpaceCoreMap = new();

    private bool _canPrestige;

    /// <summary>Initializes a new instance of the <see cref="CustomSkill"/> class.</summary>
    /// <param name="id">The unique id of skill.</param>
    internal CustomSkill(string id)
    {
        this.StringId = id;

        var scSkill = SpaceCore.Skills.GetSkill(id);
        this.DisplayName = scSkill.GetName();
        if (scSkill.Professions.Count != 6)
        {
            ThrowHelper.ThrowInvalidOperationException(
                $"The custom skill {id} did not provide the expected number of professions.");
        }

        for (var i = 0; i < 6; i++)
        {
            this.Professions.Add(new CustomProfession(scSkill.Professions[i], i < 2 ? 5 : 10, this));
        }

        this.ProfessionPairs[-1] = new ProfessionPair(this.Professions[0], this.Professions[1], null, 5);
        this.ProfessionPairs[this.Professions[0].Id] =
            new ProfessionPair(this.Professions[2], this.Professions[3], this.Professions[0], 10);
        this.ProfessionPairs[this.Professions[1].Id] =
            new ProfessionPair(this.Professions[4], this.Professions[5], this.Professions[1], 10);

        for (var i = 0; i < 6; i++)
        {
            var profession = this.Professions[i];
            CustomProfession.Loaded[profession.Id] = (CustomProfession)profession;
        }

        scSkill.ExperienceCurve = ISkill.ExperienceCurve.Skip(1).ToArray();
        SpaceCoreMap.TryAdd(this, scSkill);
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
    public int MaxLevel => this.CanGainPrestigeLevels() ? 20 : 10;

    /// <inheritdoc />
    public float BaseExperienceMultiplier =>
        ProfessionsModule.Config.SkillExpMultipliers.TryGetValue(this.StringId, out var multiplier)
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

    /// <summary>Gets the currently loaded <see cref="CustomSkill"/>s.</summary>
    /// <remarks>The value type is <see cref="ISkill"/> because this also includes <see cref="LuckSkill"/>, which is not a SpaceCore skill.</remarks>
    internal static Dictionary<string, ISkill> Loaded { get; } = new();

    /// <summary>Gets the <see cref="CustomSkill"/> equivalent to the specified <see cref="SCSkill"/>.</summary>
    /// <param name="skill">The <see cref="SCSkill"/>.</param>
    /// <returns>The equivalent <see cref="CustomSkill"/>.</returns>
    public static CustomSkill? FromSpaceCore(SCSkill skill)
    {
        return SpaceCoreMap.TryGetReverse(skill, out var scProfession) ? scProfession : null;
    }

    /// <summary>Gets the <see cref="SCSkill"/> equivalent to this <see cref="CustomSkill"/>.</summary>
    /// <returns>The equivalent <see cref="SCSkill"/>.</returns>
    public SCSkill? ToSpaceCore()
    {
        return SpaceCoreMap.TryGetForward(this, out var skill) ? skill : null;
    }

    /// <summary>Determines whether this skill can gain Prestige Levels.</summary>
    /// <returns><see langword="true"/> if the local player meets all Prestige conditions, otherwise <see langword="false"/>.</returns>
    public bool CanGainPrestigeLevels()
    {
        return this._canPrestige && (ProfessionsModule.Config.PrestigeProgressionMode == ProfessionConfig.PrestigeMode.Streamlined ||
               (ProfessionsModule.Config.PrestigeProgressionMode == ProfessionConfig.PrestigeMode.Standard &&
                ((ISkill)this).AcquiredProfessions.Length >= 4) ||
               (ProfessionsModule.Config.PrestigeProgressionMode == ProfessionConfig.PrestigeMode.Challenge &&
                Game1.player.HasAllProfessions()));
    }

    /// <inheritdoc />
    public void AddExperience(int amount)
    {
        SpaceCore.Skills.AddExperience(Game1.player, this.StringId, amount);
    }

    /// <inheritdoc />
    public void SetLevel(int level)
    {
        level = Math.Min(level, this.MaxLevel);
        for (var l = this.CurrentLevel + 1; l <= level; l++)
        {
            Reflector
                .GetStaticFieldGetter<List<KeyValuePair<string, int>>>(typeof(SpaceCore.Skills), "NewLevels")
                .Invoke()
                .Add(new KeyValuePair<string, int>(this.StringId, level));
        }

        var diff = ISkill.ExperienceCurve[level] - this.CurrentExp;
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
        if (ProfessionsModule.Config.ForgetRecipesOnSkillReset && this.StringId == "blueberry.LoveOfCooking.CookingSkill")
        {
            this.ForgetRecipes();
        }

        Log.D($"[Prestige]: {Game1.player.Name}'s {this.DisplayName} skill has been reset.");
    }

    /// <inheritdoc />
    public void ForgetRecipes(bool saveForRecovery = true)
    {
        var farmer = Game1.player;
        var forgottenRecipesDict = farmer.Read(DataKeys.ForgottenRecipesDict)
            .ParseDictionary<string, int>();

        HashSet<string> cookingRecipes;
        if (this.StringId != "blueberry.LoveOfCooking.CookingSkill" || LoveOfCookingIntegration.Instance?.IsLoaded != true)
        {
            cookingRecipes = new HashSet<string>();
            for (var level = 1; level <= 20; level++)
            {
                var levelUpRecipes = this.ToSpaceCore()!.GetSkillLevelUpCookingRecipes(level);
                if (!levelUpRecipes.ContainsKey(level))
                {
                    continue;
                }

                cookingRecipes.UnionWith(levelUpRecipes[level]);
            }
        }
        else
        {
            cookingRecipes = LoveOfCookingIntegration.Instance.ModApi!
                .GetAllLevelUpRecipes().Values
                .SelectMany(r => r)
                .Select(r => "blueberry.cac." + r)
                .ToHashSet();
        }

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

        if (this.StringId != "blueberry.LoveOfCooking.CookingSkill")
        {
            if (saveForRecovery)
            {
                farmer.Write(DataKeys.ForgottenRecipesDict, forgottenRecipesDict.Stringify());
            }

            return;
        }

        var craftingRecipes = new HashSet<string>();
        for (var level = 1; level <= 20; level++)
        {
            var levelUpRecipes = this.ToSpaceCore()!.GetSkillLevelUpCraftingRecipes(level);
            if (!levelUpRecipes.ContainsKey(level))
            {
                continue;
            }

            craftingRecipes.UnionWith(levelUpRecipes[level]);
        }

        var knownCraftingRecipes = farmer.craftingRecipes.Keys
            .Where(key => craftingRecipes.Contains(key))
            .ToDictionary(key => key, key => farmer.craftingRecipes[key]);
        foreach (var (key, value) in knownCraftingRecipes)
        {
            if (saveForRecovery && !forgottenRecipesDict.TryAdd(key, value))
            {
                forgottenRecipesDict[key] += value;
            }

            farmer.cookingRecipes.Remove(key);
        }

        if (saveForRecovery)
        {
            farmer.Write(DataKeys.ForgottenRecipesDict, forgottenRecipesDict.Stringify());
        }
    }

    /// <inheritdoc />
    public void Revalidate()
    {
        var currentExp = this.CurrentExp;
        if (currentExp > ISkill.ExpAtLevel10)
        {
            this.AddExperience(ISkill.ExpAtLevel10 - currentExp);
        }
    }

    /// <summary>Sets the prestige flag for this skill to <see langword="true"/>.</summary>
    internal void RegisterPrestige()
    {
        this._canPrestige = true;
    }
}
