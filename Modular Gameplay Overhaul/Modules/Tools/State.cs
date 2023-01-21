namespace DaLion.Overhaul.Modules.Tools;

#region using directives

using System.Collections.Generic;

#endregion using directives

/// <summary>The ephemeral runtime state for Tools.</summary>
internal sealed class State
{
    internal List<Shockwave> Shockwaves { get; } = new();

    internal Dictionary<Type, SelectableTool?> SelectableToolByType { get; } = new();
}
