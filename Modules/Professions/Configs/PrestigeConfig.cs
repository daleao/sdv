namespace DaLion.Overhaul.Modules.Professions.Configs;

#region using directives

using DaLion.Overhaul.Modules.Professions.Events.GameLoop.DayStarted;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Integrations.GMCM.Attributes;
using Newtonsoft.Json;

#endregion using directives

/// <summary>The user-configurable settings for PRFS.</summary>
public sealed class PrestigeConfig
{
    private float _skillResetCostMultiplier = 1f;
    private float _expBonusPerSkillReset = 0.1f;
    private PrestigeMode _prestigeMode = PrestigeMode.Standard;
    private RibbonStyle _ribbonStyle = RibbonStyle.StackedStars;
    private IconStyle _iconStyle = IconStyle.MetallicGold;

    #region dropdown enums

    /// <summary>Determines the conditions for unlocking prestige levels.</summary>
    public enum PrestigeMode
    {
        /// <summary>None. All prestige features disabled.</summary>
        None,

        /// <summary>Standard mode, where Prestige for each skill is unlocked individually by resetting the respective skill 3 times and acquiring all 10th-level professions within that skill.</summary>
        Standard,

        /// <summary>Challenge mode, where Prestige for all skills is unlocked simultaneously only after resetting every skill 3 times and acquiring all 10th-level professions across all skills.</summary>
        Challenge,

        /// <summary>Streamlined mode, where Prestige is available immediately without conditions. But the player can no longer aggregate all professions.</summary>
        Streamlined,

        /// <summary>Capped at level 10. Prestige levels and professions are disabled. But the player can still aggregate all professions via skill resetting.</summary>
        Capped,
    }

    /// <summary>The style used to indicate Skill Reset progression.</summary>
    public enum RibbonStyle
    {
        /// <summary>Use stacked quality star icons, one per reset level.</summary>
        StackedStars,

        /// <summary>Use Generation 3 Pokemon contest ribbons.</summary>
        Gen3Ribbons,

        /// <summary>Use Generation 4 Pokemon contest ribbons.</summary>
        Gen4Ribbons,
    }

    /// <summary>The style used for Prestige profession icons.</summary>
    public enum IconStyle
    {
        /// <summary>The original, high-contrast metallic gold style.</summary>
        MetallicGold,

        /// <summary>Posister's cleaner, hand-colored gold style.</summary>
        PosisterGold,
    }

    #endregion dropdown enums

    /// <summary>Gets a value which determines the paradigms of the prestige system.</summary>
    [JsonProperty]
    [GMCMPriority(200)]
    public PrestigeMode Mode
    {
        get => this._prestigeMode;
        internal set
        {
            this._prestigeMode = value;
            if (!Context.IsWorldReady)
            {
                return;
            }

            if (ProfessionsModule.EnablePrestigeLevels)
            {
                EventManager.Enable<PrestigeAchievementDayStartedEvent>();
            }
            else
            {
                EventManager.Disable<PrestigeAchievementDayStartedEvent>();
            }
        }
    }

    /// <summary>Gets the base skill reset cost multiplier. Set to 0 to reset for free.</summary>
    [JsonProperty]
    [GMCMPriority(201)]
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
    [GMCMPriority(202)]
    public bool ForgetRecipesOnSkillReset { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the player can use the Statue of Prestige more than once per day.</summary>
    [JsonProperty]
    [GMCMPriority(203)]
    public bool AllowMultipleResets { get; internal set; } = false;

    /// <summary>Gets a percentage bonus applied to a skill's experience gain after a respective skill prestige. Negative values mean it becomes harder to regain those levels.</summary>
    [JsonProperty]
    [GMCMPriority(204)]
    [GMCMRange(-0.5f, 1f)]
    public float ExpBonusPerSkillReset
    {
        get => this._expBonusPerSkillReset;
        internal set
        {
            this._expBonusPerSkillReset = Math.Max(value, -0.5f);
        }
    }

    /// <summary>Gets monetary cost of respecing prestige profession choices for a skill. Set to 0 to respec for free.</summary>
    [JsonProperty]
    [GMCMPriority(205)]
    [GMCMRange(0, 100000)]
    [GMCMInterval(1000)]
    public uint PrestigeRespecCost { get; internal set; } = 20000;

    /// <summary>
    ///     Gets the style of the sprite that appears next to skill bars. Accepted values: "StackedStars", "Gen3Ribbons",
    ///     "Gen4Ribbons".
    /// </summary>
    [JsonProperty]
    [GMCMPriority(206)]
    public RibbonStyle Ribbon
    {
        get => this._ribbonStyle;
        internal set
        {
            if (value == this._ribbonStyle)
            {
                return;
            }

            this._ribbonStyle = value;
            ModHelper.GameContent.InvalidateCache($"{Manifest.UniqueID}/PrestigeRibbons");
        }
    }

    /// <summary>
    ///     Gets the style of the sprite used for Prestige profession variants. Accepted values: "MetallicGold", "PosisterGold".
    /// </summary>
    [JsonProperty]
    [GMCMPriority(206)]
    public IconStyle Icon
    {
        get => this._iconStyle;
        internal set
        {
            if (value == this._iconStyle)
            {
                return;
            }

            this._iconStyle = value;
            ModHelper.GameContent.InvalidateCache($"{Manifest.UniqueID}/PrestigeIcons");
            ModHelper.GameContent.InvalidateCacheAndLocalized("LooseSprites/Cursors");
        }
    }

    /// <summary>Gets how much skill experience is required for each level up beyond 10.</summary>
    [JsonProperty]
    [GMCMPriority(207)]
    [GMCMRange(1000, 10000)]
    [GMCMInterval(500)]
    public uint ExpPerPrestigeLevel { get; internal set; } = 5000;

    /// <summary>Gets a value indicating whether to add full skill mastery (level 20) as a requirement for perfection.</summary>
    [JsonProperty]
    [GMCMPriority(208)]
    public bool IsPerfectionRequirement { get; internal set; } = true;
}
