namespace DaLion.Ligo.Modules.Arsenal.Weapons;

#region using directives

using StardewValley.Tools;

#endregion using directives

/// <summary>The hit steps of a <see cref="MeleeWeapon"/> combo.</summary>
public enum ComboHitStep
{
    /// <summary>Not currently attacking.</summary>
    Idle,

    /// <summary>The first hit of the combo.</summary>
    FirstHit,

    /// <summary>The second hit of the combo.</summary>
    SecondHit,

    /// <summary>The third hit of the combo.</summary>
    ThirdHit,

    /// <summary>The fourth hit of the combo.</summary>
    FourthHit,

    /// <summary>The infinity-th hit of the combo.</summary>
    Infinite = int.MaxValue,
}
