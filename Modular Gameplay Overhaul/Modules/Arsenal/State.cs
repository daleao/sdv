namespace DaLion.Overhaul.Modules.Arsenal;

#region using directives

using DaLion.Overhaul.Modules.Arsenal.Events;

#endregion using directives

/// <summary>The ephemeral runtime state for Arsenal.</summary>
internal sealed class State
{
    private ComboHitStep _hitStep;
    private bool _animating;

    internal ComboHitStep ComboHitStep
    {
        get => this._hitStep;
        set
        {
            Log.D($"{value}");
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
}
