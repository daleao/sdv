#pragma warning disable CS1591
namespace DaLion.Shared.Integrations.CustomOreNodes;

#region using directives

using System.Collections.Generic;

#endregion using directives

/// <summary>The API provided by Custom Ore Nodes.</summary>
public interface ICustomOreNodesApi
{
    List<object> GetCustomOreNodes();

    List<string> GetCustomOreNodeIDs();
}
