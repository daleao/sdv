namespace DaLion.Overhaul.Modules.Tools;

#region using directives

using System.Collections.Generic;

#endregion using directives

/// <summary>The runtime state variables for TOLS.</summary>
internal sealed class ToolState
{
    internal List<Shockwave> Shockwaves { get; } = new();

    internal Dictionary<Type, SelectableTool?> SelectableToolByType { get; } = new();
}
