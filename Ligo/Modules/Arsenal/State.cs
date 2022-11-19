namespace DaLion.Ligo.Modules.Arsenal;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Events;

#endregion using directives

/// <summary>Holds the runtime state variables of the Arsenal module.</summary>
internal sealed class State
{
    private bool _isAnimating;
    private ComboHitStep _hitStep = ComboHitStep.Idle;

    // slingshots
    internal int SlingshotSpecialCooldown { get; set; }

    // weapons
    internal int SecondsOutOfCombat { get; set; }

    internal int WeaponSwingCooldown { get; set; }

    internal bool IsFarmerAnimating
    {
        get => this._isAnimating;

        set
        {
            if (value)
            {
                ModEntry.Events.Disable<ComboResetUpdateTickedEvent>();
            }
            else
            {
                ModEntry.Events.Enable<ComboResetUpdateTickedEvent>();
            }

            this._isAnimating = value;
        }
    }

    internal ComboHitStep ComboHitStep
    {
        get => this._hitStep;
        set
        {
            Log.D($"{value}");
            this._hitStep = value;
        }
    }
}
