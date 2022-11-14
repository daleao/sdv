namespace DaLion.Ligo.Modules.Arsenal;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Weapons;

#endregion using directives

/// <summary>Holds the runtime state variables of the Arsenal module.</summary>
internal sealed class State
{
    // slingshots
    internal int SlingshotCooldown { get; set; }

    // weapons
    internal int SecondsOutOfCombat { get; set; }

    internal int WeaponSwingCooldown { get; set; }

    internal ComboHitStep ComboHitStep { get; set; } = ComboHitStep.Idle;
}
