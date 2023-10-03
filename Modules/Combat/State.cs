namespace DaLion.Overhaul.Modules.Combat;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicked;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Tools;

#endregion using directives

/// <summary>The runtime state variables for CMBT.</summary>
internal sealed class State
{
    private ComboHitStep _hitQueued;
    private ComboHitStep _hitStep;
    private bool _animating;
    private int _warriorKillCount;

    internal bool HoldingWeaponSwing { get; set; }

    internal ComboHitStep ComboHitQueued
    {
        get => this._hitQueued;
        set
        {
            Log.D($"[Combo]: Queued {value}");
            this._hitQueued = value;
        }
    }

    internal ComboHitStep ComboHitStep
    {
        get => this._hitStep;
        set
        {
            Log.D($"[Combo]: Doing {value}");
            this._hitStep = value;
        }
    }

    internal int ComboCooldown { get; set; }

    internal bool FarmerAnimating
    {
        get => this._animating;
        set
        {
            if (value)
            {
                EventManager.Disable<ComboResetUpdateTickedEvent>();
            }
            else
            {
                EventManager.Enable<ComboResetUpdateTickedEvent>();
            }

            this._animating = value;
        }
    }

    internal int SlingshotCooldown { get; set; }

    internal float SlingshotAddedScale { get; set; }

    internal MeleeWeapon? AutoSelectableMelee { get; set; }

    internal Slingshot? AutoSelectableRanged { get; set; }

    internal Vector2 DriftVelocity { get; set; }

    internal double ContainerDropAccumulator { get; set; }

    internal double MonsterDropAccumulator { get; set; }

    internal Monster? HoveredEnemy { get; set; }

    internal HeroQuest? HeroQuest { get; set; }

    internal bool DidPrayToday { get; set; }

    internal bool SpokeWithWizardToday { get; set; }

    internal bool DidArtfulParry { get; set; }

    internal bool GatlingModeEngaged { get; set; }

    internal int DoublePressTimer { get; set; }

    internal int WarriorKillCount
    {
        get
        {
            return this._warriorKillCount;
        }

        set
        {
            this._warriorKillCount = Math.Min(value, 20);
        }
    }

    internal int SavageExcitedness { get; set; }

    internal int YobaShieldHealth { get; set; } = -1;

    internal bool CanReceiveYobaShield { get; set; } = true;
}
