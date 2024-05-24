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
    private IconStyle _prestigeIconStyle = IconStyle.MetallicGold;

    #region dropdown enums

    /// <summary>The style used for Skills profession icons.</summary>
    public enum IconStyle
    {
        /// <summary>The original, high-contrast metallic gold style.</summary>
        MetallicGold,

        /// <summary>Posister's cleaner, hand-colored gold style.</summary>
        PosisterGold,
    }

    #endregion dropdown enums

    /// <summary>Gets a value indicating whether to allow Masteries Breaks to be used in-game.</summary>
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

    /// <summary>Gets the mod key used to activate the Masteries Break.</summary>
    [JsonProperty]
    [GMCMSection("prfs.limit_break")]
    [GMCMPriority(101)]
    public KeybindList LimitBreakKey { get; internal set; } = KeybindList.Parse("LeftShift, LeftShoulder");

    /// <summary>Gets a value indicating whether the Masteries Break is activated by holding the <see cref="LimitBreakKey"/>, as opposed to simply pressing.</summary>
    [JsonProperty]
    [GMCMSection("prfs.limit_break")]
    [GMCMPriority(102)]
    public bool HoldKeyToLimitBreak { get; internal set; } = true;

    /// <summary>Gets how long the <see cref="LimitBreakKey"/> should be held to activate the Masteries Break, in milliseconds.</summary>
    [JsonProperty]
    [GMCMSection("prfs.limit_break")]
    [GMCMPriority(103)]
    [GMCMRange(250, 2000)]
    [GMCMStep(50)]
    public uint HoldDelayMilliseconds { get; internal set; } = 250;

    /// <summary>
    ///     Gets the rate at which one builds the Masteries gauge. Increase this if you feel the gauge raises too
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
    ///     Gets the rate at which the Masteries gauge depletes during LimitBreak. Decrease this to make the Masteries Break last
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

    /// <summary>Gets monetary cost of changing the chosen Masteries Break. Set to 0 to change for free.</summary>
    [JsonProperty]
    [GMCMSection("prfs.limit_break")]
    [GMCMPriority(106)]
    [GMCMRange(0, 100000)]
    [GMCMStep(1000)]
    public uint LimitRespecCost { get; internal set; } = 0;

    /// <summary>Gets the offset that should be applied to the Masteries Gauge's position.</summary>
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
    [GMCMRange(1000, 10000)]
    [GMCMStep(500)]
    public uint ExpPerPrestigeLevel { get; internal set; } = 5000;

    /// <summary>Gets the monetary cost of respecing prestige profession choices for a skill. Set to 0 to respec for free.</summary>
    [JsonProperty]
    [GMCMSection("prfs.prestige")]
    [GMCMPriority(201)]
    [GMCMRange(0, 100000)]
    [GMCMStep(1000)]
    public uint PrestigeRespecCost { get; internal set; } = 20000;

    /// <summary>
    ///     Gets the style of the sprite used for Skills profession variants. Accepted values: "MetallicGold", "PosisterGold".
    /// </summary>
    [JsonProperty]
    [GMCMSection("prfs.prestige")]
    [GMCMPriority(203)]
    public IconStyle PrestigeIconStyle
    {
        get => this._prestigeIconStyle;
        internal set
        {
            if (value == this._prestigeIconStyle)
            {
                return;
            }

            this._prestigeIconStyle = value;
            ModHelper.GameContent.InvalidateCache($"{UniqueId}/ProfessionIcons");
            ModHelper.GameContent.InvalidateCacheAndLocalized("LooseSprites/Cursors");
        }
    }
}
