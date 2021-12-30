using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using TheLion.Stardew.Professions.Framework.Events;
using TheLion.Stardew.Professions.Framework.TreasureHunt;

namespace TheLion.Stardew.Professions;

public static class ModState
{
    // super mode private fields
    private static int _index = -1;
    private static bool _isActive;
    private static int _value;

    // treasure hunts
    internal static ProspectorHunt ProspectorHunt { get; set; }
    internal static ScavengerHunt ScavengerHunt { get; set; }

    // profession perks
    internal static int DemolitionistExcitedness { get; set; }
    internal static int SpelunkerLadderStreak { get; set; }
    internal static int SlimeContactTimer { get; set; }
    internal static HashSet<int> MonstersStolenFrom { get; set; }
    internal static Dictionary<GreenSlime, float> PipedSlimeScales { get; set; }
    internal static HashSet<int> AuxiliaryBullets { get; set; }
    internal static HashSet<int> BouncedBullets { get; set; }
    internal static HashSet<int> PiercedBullets { get; set; }

    // super mode properties
    public static bool ShouldShakeSuperModeGauge { get; set; }
    public static float SuperModeGaugeAlpha { get; set; }
    public static Color SuperModeGlowColor { get; set; }
    public static float SuperModeOverlayAlpha { get; set; }
    public static Color SuperModeOverlayColor { get; set; }
    public static string SuperModeSFX { get; set; }
    public static Dictionary<int, HashSet<long>> ActivePeerSuperModes { get; set; } = new();
    public static bool UsedDogStatueToday { get; set; }

    public static int SuperModeIndex
    {
        get => _index;
        set
        {
            if (_index == value) return;
            _index = value;
            SuperModeIndexChanged?.Invoke(value);
        }
    }

    public static int SuperModeGaugeValue
    {
        get => _value;
        set
        {
            if (value == 0)
            {
                _value = 0;
                SuperModeGaugeReturnedToZero?.Invoke();
            }
            else
            {
                if (_value == value) return;

                if (_value == 0) SuperModeGaugeRaisedAboveZero?.Invoke();
                if (value >= SuperModeGaugeMaxValue) SuperModeGaugeFilled?.Invoke();
                _value = Math.Min(value, SuperModeGaugeMaxValue);
            }
        }
    }

    public static int SuperModeGaugeMaxValue =>
        Game1.player.CombatLevel >= 10
            ? Game1.player.CombatLevel * 50
            : 500;

    public static bool IsSuperModeActive
    {
        get => _isActive;
        set
        {
            if (_isActive == value) return;

            if (!value) SuperModeDisabled?.Invoke();
            else SuperModeEnabled?.Invoke();
            _isActive = value;
        }
    }

    // super mode event handlers
    public static event SuperModeGaugeFilledEventHandler SuperModeGaugeFilled;
    public static event SuperModeGaugeRaisedAboveZeroEventHandler SuperModeGaugeRaisedAboveZero;
    public static event SuperModeGaugeReturnedToZeroEventHandler SuperModeGaugeReturnedToZero;
    public static event SuperModeDisabledEventHandler SuperModeDisabled;
    public static event SuperModeEnabledEventHandler SuperModeEnabled;
    public static event SuperModeIndexChangedEventHandler SuperModeIndexChanged;

    static ModState()
    {
        MonstersStolenFrom = new();
        PipedSlimeScales = new();
        AuxiliaryBullets = new();
        BouncedBullets = new();
        PiercedBullets = new();
    }
}