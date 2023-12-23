namespace DaLion.Overhaul.Modules.Professions.Configs;

#region using directives

using DaLion.Overhaul.Modules.Professions.Events.Display.RenderingHud;
using DaLion.Overhaul.Modules.Professions.Events.Player.Warped;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Ultimates;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Integrations.GMCM.Attributes;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The user-configurable settings for PRFS.</summary>
public sealed class LimitBreakConfig
{
    private bool _enableLimitBreaks = true;
    private double _limitGainFactor = 1f;
    private double _limitDrainFactor = 1f;

    /// <summary>Gets a value indicating whether to allow Limit Breaks to be used in-game.</summary>
    [JsonProperty]
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
            if (!Context.IsWorldReady || Game1.player.Get_Ultimate() is not { } ultimate)
            {
                return;
            }

            switch (value)
            {
                case false:
                    ultimate.ChargeValue = 0d;
                    EventManager.DisableWithAttribute<UltimateEventAttribute>();
                    break;
                case true:
                {
                    Game1.player.RevalidateUltimate();
                    EventManager.Enable<UltimateWarpedEvent>();
                    if (Game1.currentLocation.IsDungeon())
                    {
                        EventManager.Enable<UltimateMeterRenderingHudEvent>();
                    }

                    break;
                }
            }
        }
    }

    /// <summary>Gets the mod key used to activate the Limit Break.</summary>
    [JsonProperty]
    [GMCMPriority(101)]
    public KeybindList LimitBreakKey { get; internal set; } = KeybindList.Parse("LeftShift, LeftShoulder");

    /// <summary>Gets a value indicating whether the Limit Break is activated by holding the <see cref="LimitBreakKey"/>, as opposed to simply pressing.</summary>
    [JsonProperty]
    [GMCMPriority(102)]
    public bool HoldKeyToLimitBreak { get; internal set; } = true;

    /// <summary>Gets how long the <see cref="LimitBreakKey"/> should be held to activate the Limit Break, in milliseconds.</summary>
    [JsonProperty]
    [GMCMPriority(103)]
    [GMCMRange(250, 2000)]
    [GMCMInterval(50)]
    public uint HoldDelayMilliseconds { get; internal set; } = 250;

    /// <summary>
    ///     Gets the rate at which one builds the Limit gauge. Increase this if you feel the gauge raises too
    ///     slowly.
    /// </summary>
    [JsonProperty]
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
    ///     Gets the rate at which the Limit gauge depletes during Ultimate. Decrease this to make the Limit Break last
    ///     longer.
    /// </summary>
    [JsonProperty]
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
    [GMCMPriority(106)]
    [GMCMRange(0, 100000)]
    [GMCMInterval(1000)]
    public uint LimitRespecCost { get; internal set; } = 0;

    /// <summary>Gets the offset that should be applied to the Limit Gauge's position.</summary>
    [JsonProperty]
    [GMCMPriority(107)]
    [GMCMDefaultVector2(0f, 0f)]
    public Vector2 LimitGaugeOffset { get; internal set; } = Vector2.Zero;
}
