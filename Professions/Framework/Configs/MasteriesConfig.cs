namespace DaLion.Professions.Framework.Configs;

#region using directives

using DaLion.Professions.Framework.Events.Player.Warped;
using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Integrations.GMCM.Attributes;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The Mastery-related settings for PRFS.</summary>
public sealed class MasteriesConfig
{
    private bool _enableLimitBreaks = true;
    private double _limitGainFactor = 1d;
    private double _limitDrainFactor = 1d;
    private uint _expPerPrestigeLevel = 5000;
    private GoldPalette _goldSpritePalette = GoldPalette.SiliconGold;

    #region dropdown enums

    /// <summary>A palette used for golden icons and sprites.</summary>
    public enum GoldPalette
    {
        /// <summary>Gold palette made by silicon.</summary>
        SiliconGold,

        /// <summary>Rose-gold palette made by KawaiiMuski.</summary>
        KawaiiRoseGold,
    }

    #endregion dropdown enums

    /// <summary>Gets or sets a value indicating whether to prevent the player from purchasing a skill Mastery if there are professions left to acquire.</summary>
    [JsonProperty]
    [GMCMPriority(1)]
    public bool LockMasteryUntilFullReset { get; set; } = true;

    /// <summary>Gets a value indicating whether to allow Limit Breaks to be used in-game.</summary>
    [JsonProperty]
    [GMCMSection("prfs.limit_break")]
    [GMCMPriority(100)]
    public bool EnableLimitBreaks
    {
        get => this._enableLimitBreaks;
        internal set
        {
            if (value == this._enableLimitBreaks)
            {
                return;
            }

            this._enableLimitBreaks = value;
            if (!Context.IsWorldReady || State.LimitBreak is null)
            {
                return;
            }

            switch (value)
            {
                case false:
                    State.LimitBreak.ChargeValue = 0d;
                    EventManager.DisableWithAttribute<LimitEventAttribute>();
                    break;
                case true:
                {
                    if (State.LimitBreak is not null)
                    {
                        EventManager.Enable<LimitWarpedEvent>();
                    }

                    break;
                }
            }
        }
    }

    /// <summary>Gets the mod key used to activate the Limit Break.</summary>
    [JsonProperty]
    [GMCMSection("prfs.limit_break")]
    [GMCMPriority(101)]
    public KeybindList LimitBreakKey { get; internal set; } = KeybindList.Parse("LeftShift, LeftShoulder");

    /// <summary>Gets a value indicating whether the Limit Break is activated by holding the <see cref="LimitBreakKey"/>, as opposed to simply pressing.</summary>
    [JsonProperty]
    [GMCMSection("prfs.limit_break")]
    [GMCMPriority(102)]
    public bool HoldKeyToLimitBreak { get; internal set; } = true;

    /// <summary>Gets how long the <see cref="LimitBreakKey"/> should be held to activate the Limit Break, in milliseconds.</summary>
    [JsonProperty]
    [GMCMSection("prfs.limit_break")]
    [GMCMPriority(103)]
    [GMCMRange(250, 2000, 50)]
    public uint HoldDelayMilliseconds { get; internal set; } = 250;

    /// <summary>
    ///     Gets the rate at which one builds the Limit gauge. Increase this if you feel the gauge raises too
    ///     slowly.
    /// </summary>
    [JsonProperty]
    [GMCMSection("prfs.limit_break")]
    [GMCMPriority(104)]
    [GMCMRange(0.25f, 4f)]
    public double LimitGainFactor
    {
        get => this._limitGainFactor;
        internal set
        {
            this._limitGainFactor = Math.Abs(value);
        }
    }

    /// <summary>
    ///     Gets the rate at which the Limit gauge depletes during LimitBreak. Decrease this to make the Limit Break last
    ///     longer.
    /// </summary>
    [JsonProperty]
    [GMCMSection("prfs.limit_break")]
    [GMCMPriority(105)]
    [GMCMRange(0.25f, 4f)]
    public double LimitDrainFactor
    {
        get => this._limitDrainFactor;
        internal set
        {
            this._limitDrainFactor = Math.Abs(value);
        }
    }

    /// <summary>Gets monetary cost of changing the chosen Limit Break. Set to 0 to change for free.</summary>
    [JsonProperty]
    [GMCMSection("prfs.limit_break")]
    [GMCMPriority(106)]
    [GMCMRange(0, 100000, 1000)]
    public uint LimitRespecCost { get; internal set; } = 0;

    /// <summary>Gets the offset that should be applied to the Limit Gauge's position.</summary>
    [JsonProperty]
    [GMCMSection("prfs.limit_break")]
    [GMCMPriority(107)]
    [GMCMDefaultVector2(0f, 0f)]
    public Vector2 LimitGaugeOffset { get; internal set; } = Vector2.Zero;

    /// <summary>Gets a value indicating whether the player can gain levels up to 20 and choose Prestiged professions.</summary>
    [JsonProperty]
    [GMCMSection("prfs.prestige")]
    [GMCMPriority(200)]
    public bool EnablePrestigeLevels { get; internal set; } = true;

    /// <summary>Gets how much skill experience is required for each level up beyond 10.</summary>
    [JsonProperty]
    [GMCMSection("prfs.prestige")]
    [GMCMPriority(202)]
    [GMCMRange(1000, 10000, 500)]
    public uint ExpPerPrestigeLevel
    {
        get => this._expPerPrestigeLevel;
        internal set
        {
            var oldValue = this._expPerPrestigeLevel;
            this._expPerPrestigeLevel = value;
            if (!Context.IsWorldReady)
            {
                return;
            }

            if (VanillaSkill.List.Any(skill => skill.CurrentLevel > 10) ||
                CustomSkill.Loaded.Values.Any(skill => skill.CurrentLevel > 10))
            {
                this._expPerPrestigeLevel = oldValue;
                Log.W(
                    "You cannot change the experience points required for a prestige level after having gained prestige levels. Please make sure all skills have been reset to level 10 before attempting to change this setting.");
                return;
            }

            for (var i = 1; i <= 10; i++)
            {
                ISkill.ExperienceCurve[i + 10] = ISkill.LEVEL_10_EXP + (int)(value * i);
            }
        }
    }

    /// <summary>Gets the monetary cost of respecing prestige profession choices for a skill. Set to 0 to respec for free.</summary>
    [JsonProperty]
    [GMCMSection("prfs.prestige")]
    [GMCMPriority(201)]
    [GMCMRange(0, 100000, 1000)]
    public uint PrestigeRespecCost { get; internal set; } = 20000;

    /// <summary>
    ///     Gets the style of the sprite used for mastered skill icons, prestige profession icons and the special Gold Slime. Accepted values: "SiliconGold", "KawaiiRoseGold".
    /// </summary>
    [JsonProperty]
    [GMCMSection("prfs.prestige")]
    [GMCMPriority(204)]
    public GoldPalette GoldSpritePalette
    {
        get => this._goldSpritePalette;
        internal set
        {
            if (value == this._goldSpritePalette)
            {
                return;
            }

            this._goldSpritePalette = value;
            ModHelper.GameContent.InvalidateCache($"{UniqueId}_MasteredSkillIcons");
            ModHelper.GameContent.InvalidateCache($"{UniqueId}_ProfessionIcons");
            ModHelper.GameContent.InvalidateCache($"{UniqueId}_GoldSlime");
            ModHelper.GameContent.InvalidateCacheAndLocalized("LooseSprites/Cursors");
        }
    }
}
