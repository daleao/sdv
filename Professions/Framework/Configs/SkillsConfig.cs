namespace DaLion.Professions.Framework.Configs;

#region using directives

using DaLion.Shared.Integrations.GMCM.Attributes;
using Newtonsoft.Json;

#endregion using directives

/// <summary>The Skill-related settings for PRFS.</summary>
public sealed class SkillsConfig
{
    private readonly Dictionary<string, float> _baseSkillExpMultipliers = new()
    {
        { "Farming", 1f },
        { "Fishing", 1f },
        { "Foraging", 1f },
        { "Mining", 1f },
        { "Combat", 1f },
    };

    private float _skillResetCostMultiplier = 1f;
    private float _skillExpMultiplierPerReset = 1.25f;

    /// <summary>Gets a value indicating whether the player can reset their skills to gain a new profession.</summary>
    [JsonProperty]
    [GMCMSection("prfs.skill_reset")]
    [GMCMPriority(100)]
    public bool EnableSkillReset { get; internal set; } = true;

    /// <summary>Gets the base skill reset cost multiplier. Set to 0 to reset for free.</summary>
    [JsonProperty]
    [GMCMSection("prfs.skill_reset")]
    [GMCMPriority(101)]
    [GMCMRange(0f, 3f)]
    public float SkillResetCostMultiplier
    {
        get => this._skillResetCostMultiplier;
        internal set
        {
            this._skillResetCostMultiplier = Math.Abs(value);
        }
    }

    /// <summary>Gets a value indicating whether resetting a skill also clears all corresponding recipes.</summary>
    [JsonProperty]
    [GMCMSection("prfs.skill_reset")]
    [GMCMPriority(102)]
    public bool ForgetRecipesOnSkillReset { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the player can use the Statue of Uncertainty more than once per day.</summary>
    [JsonProperty]
    [GMCMSection("prfs.skill_reset")]
    [GMCMPriority(103)]
    public bool AllowMultipleResets { get; internal set; } = false;

    /// <summary>Gets a multiplier applied to a skill's experience gain after a respective skill reset. Negative values mean it becomes harder to regain those levels.</summary>
    [JsonProperty]
    [GMCMSection("prfs.skill_reset")]
    [GMCMPriority(104)]
    [GMCMRange(0.5f, 2f)]
    [GMCMStep(0.05f)]
    public float SkillExpMultiplierPerReset
    {
        get => this._skillExpMultiplierPerReset;
        internal set
        {
            this._skillExpMultiplierPerReset = Math.Max(value, 0.5f);
        }
    }

    /// <summary>Gets a multiplier that will be applied to all skill experience gained from the start of the game.</summary>
    [JsonProperty]
    [GMCMSection("prfs.skill_exp")]
    [GMCMPriority(200)]
    [GMCMRange(0.5f, 2f)]
    [GMCMStep(0.05f)]
    [GMCMOverride(typeof(ProfessionsConfigMenu), "SkillExpMultipliersOverride")]
    public Dictionary<string, float> BaseMultipliers
    {
        get => this._baseSkillExpMultipliers;
        internal set
        {
            if (value == this._baseSkillExpMultipliers)
            {
                return;
            }

            foreach (var pair in value)
            {
                this._baseSkillExpMultipliers[pair.Key] = Math.Abs(pair.Value);
            }
        }
    }
}
