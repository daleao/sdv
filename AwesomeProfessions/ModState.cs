using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using TheLion.Stardew.Professions.Framework.Events.Custom.SuperMode;
using TheLion.Stardew.Professions.Framework.TreasureHunt;

namespace TheLion.Stardew.Professions;

public class ModState
{
    // super mode private fields
    private int _index = -1;

    private bool _isActive;
    private int _gauge;

    // treasure hunts
    internal ProspectorHunt ProspectorHunt { get; set; }

    internal ScavengerHunt ScavengerHunt { get; set; }

    // profession perks
    internal int DemolitionistExcitedness { get; set; }

    internal int SpelunkerLadderStreak { get; set; }
    internal int SlimeContactTimer { get; set; }
    internal HashSet<int> MonstersStolenFrom { get; set; } = new();
    internal HashSet<int> AuxiliaryBullets { get; set; } = new();
    internal HashSet<int> BouncedBullets { get; set; } = new();
    internal HashSet<int> PiercedBullets { get; set; } = new();
    internal Dictionary<GreenSlime, float> PipedSlimeScales { get; set; } = new();

    // super mode properties
    public bool ShouldShakeSuperModeGauge { get; set; }

    public float SuperModeGaugeAlpha { get; set; }
    public Color SuperModeGlowColor { get; set; }
    public float SuperModeOverlayAlpha { get; set; }
    public Color SuperModeOverlayColor { get; set; }
    public string SuperModeSFX { get; set; }
    public Dictionary<int, HashSet<long>> ActivePeerSuperModes { get; set; } = new();
    public bool UsedDogStatueToday { get; set; }

    public int SuperModeIndex
    {
        get => _index;
        set
        {
            if (_index == value) return;
            _index = value;
            SuperModeIndexChanged?.Invoke(value);
        }
    }

    public int SuperModeGaugeValue
    {
        get => _gauge;
        set
        {
            if (value == 0)
            {
                _gauge = 0;
                SuperModeGaugeReturnedToZero?.Invoke();
            }
            else
            {
                if (_gauge == value) return;

                if (_gauge == 0) SuperModeGaugeRaisedAboveZero?.Invoke();
                if (value >= SuperModeGaugeMaxValue) SuperModeGaugeFilled?.Invoke();
                _gauge = Math.Min(value, SuperModeGaugeMaxValue);
            }
        }
    }

    public int SuperModeGaugeMaxValue =>
        Game1.player.CombatLevel >= 10
            ? Game1.player.CombatLevel * 50
            : 500;

    public bool IsSuperModeActive
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
    public event SuperModeGaugeFilledEventHandler SuperModeGaugeFilled;

    public event SuperModeGaugeRaisedAboveZeroEventHandler SuperModeGaugeRaisedAboveZero;

    public event SuperModeGaugeReturnedToZeroEventHandler SuperModeGaugeReturnedToZero;

    public event SuperModeDisabledEventHandler SuperModeDisabled;

    public event SuperModeEnabledEventHandler SuperModeEnabled;

    public event SuperModeIndexChangedEventHandler SuperModeIndexChanged;
}