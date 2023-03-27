namespace DaLion.Overhaul.Modules.Weapons;

#region using directives

using DaLion.Overhaul.Modules.Weapons.Events;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

/// <summary>The runtime state for Weapon variables.</summary>
internal sealed class State
{
    private ComboHitStep _hitQueued;
    private ComboHitStep _hitStep;
    private bool _animating;

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

    internal Vector2 DriftVelocity { get; set; }

    internal MeleeWeapon? AutoSelectableWeapon { get; set; }

    internal double ContainerDropAccumulator { get; set; }

    internal double MonsterDropAccumulator { get; set; }
}
