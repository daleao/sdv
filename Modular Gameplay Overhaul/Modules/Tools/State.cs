namespace DaLion.Overhaul.Modules.Tools;

#region using directives

using System.Collections.Generic;

#endregion using directives

/// <summary>The runtime state for Tool variables.</summary>
internal sealed class State
{
    internal List<Shockwave> Shockwaves { get; } = new();

    internal Dictionary<Type, SelectableTool?> SelectableToolByType { get; } = new();
}
