namespace DaLion.Overhaul.Modules.Professions.Configs;

#region using directives

using System.Collections.Generic;
using DaLion.Overhaul.Modules.Core.ConfigMenu;
using DaLion.Shared.Integrations.GMCM.Attributes;
using Newtonsoft.Json;

#endregion using directives

/// <summary>The user-configurable settings for PRFS.</summary>
public sealed class ExperienceConfig
{
    private readonly Dictionary<string, float> _skillExpMultipliers = new()
    {
        { "Farming", 1f },
        { "Fishing", 1f },
        { "Foraging", 1f },
        { "Mining", 1f },
        { "Combat", 1f },
        { "Luck", 1f },
        { "blueberry.LoveOfCooking.CookingSkill", 1f },
        { "spacechase0.Cooking", 1f },
        { "spacechase0.Magic", 1f },
        { "drbirbdev.Binning", 1f },
        { "drbirbdev.Socializing", 1f },
        { "moonslime.Excavation", 1f },
    };

    /// <summary>Gets a multiplier that will be applied to all skill experience gained from the start of the game.</summary>
    /// <remarks>The order is Farming, Fishing, Foraging, Mining, Combat and Luck (if installed).</remarks>
    [JsonProperty]
    [GMCMPriority(300)]
    [GMCMRange(0.2f, 2f)]
    [GMCMOverride(typeof(GenericModConfigMenu), "ProfessionConfigSkillExpMultipliersOverride")]
    public Dictionary<string, float> Multipliers
    {
        get => this._skillExpMultipliers;
        internal set
        {
            if (value == this._skillExpMultipliers)
            {
                return;
            }

            foreach (var pair in value)
            {
                this._skillExpMultipliers[pair.Key] = Math.Abs(pair.Value);
            }
        }
    }
}
