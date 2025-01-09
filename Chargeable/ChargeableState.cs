namespace DaLion.Chargeable;

#region using directives

using System.Collections.Generic;

#endregion using directives

/// <summary>The ephemeral runtime state for Tools.</summary>
internal sealed class ChargeableState
{
    internal List<Shockwave> Shockwaves { get; } = new();

    internal bool ShockwaveHitting { get; set; }
}
