namespace DaLion.Stardew.Common.Integrations;

#region using directives

using System.Collections.Generic;

#endregion using directives

public interface ICustomOreNodesApi
{
    List<object> GetCustomOreNodes();

    List<string> GetCustomOreNodeIDs();
}