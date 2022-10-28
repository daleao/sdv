namespace DaLion.Redux.Arsenal;

#region using directives

using System.Collections.Generic;
using DaLion.Redux.Arsenal.Weapons;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Holds the runtime state variables of the Arsenal module.</summary>
internal sealed class State
{
    // arsenal
    internal List<Monster> KnockbackImmuneMonsters { get; } = new();

    // slingshots
    internal int SlingshotCooldown { get; set; }

    // weapons
    internal int EnergizeStacks { get; set; } = -1;

    internal int WeaponSwingCooldown { get; set; }

    internal ComboHitStep ComboHitStep { get; set; } = ComboHitStep.Idle;
}
