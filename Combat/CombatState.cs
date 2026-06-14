namespace DaLion.Combat;

#region using directives

using DaLion.Combat.Framework.Events;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>The ephemeral runtime state for Tools.</summary>
internal sealed class CombatState
{
    internal bool HoldingWeaponSwing { get; set; }

    internal ComboHitStep QueuedHitStep
    {
        get;
        set
        {
            Log.D($"[Combo]: Queued {value}");
            field = value;
        }
    }

    internal ComboHitStep CurrentHitStep
    {
        get;
        set
        {
            Log.D($"[Combo]: Doing {value}");
            field = value;
        }
    }

    internal int ComboCooldown { get; set; }

    internal bool FarmerAnimating
    {
        get;
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

            field = value;
        }
    }

    internal Vector2 DriftVelocity { get; set; }
}
