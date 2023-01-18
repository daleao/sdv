namespace DaLion.Overhaul.Modules.Arsenal;

#region using directives

using DaLion.Overhaul.Modules.Arsenal.Events.Weapons;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>The ephemeral runtime state for Arsenal.</summary>
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

    internal Tool? SelectableArsenal { get; set; }
}
