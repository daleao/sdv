namespace DaLion.Chargeable;

#region using directives

using System.Collections.Generic;
using DaLion.Chargeable.Framework;

#endregion using directives

/// <summary>The ephemeral runtime state for Tools.</summary>
internal sealed class ModState
{
    internal List<Shockwave> Shockwaves { get; } = new();
}
