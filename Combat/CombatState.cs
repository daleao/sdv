namespace DaLion.Combat;

#region using directives

using DaLion.Combat.Framework.Events;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>The ephemeral runtime state for Tools.</summary>
internal sealed class CombatState
{
    private ComboHitStep _queuedStep;
    private ComboHitStep _currentStep;
    private bool _animating;

    internal bool HoldingWeaponSwing { get; set; }

    internal ComboHitStep QueuedHitStep
    {
        get => this._queuedStep;
        set
        {
            Log.D($"[Combo]: Queued {value}");
            this._queuedStep = value;
        }
    }

    internal ComboHitStep CurrentHitStep
    {
        get => this._currentStep;
        set
        {
            Log.D($"[Combo]: Doing {value}");
            this._currentStep = value;
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

    internal Vector2 DriftVelocity { get; set; }
}
