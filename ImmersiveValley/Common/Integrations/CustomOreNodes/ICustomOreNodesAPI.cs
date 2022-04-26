namespace DaLion.Common.Integrations;

#region using directives

using System.Collections.Generic;

#endregion using directives

public interface ICustomOreNodesAPI
{
    List<object> GetCustomOreNodes();

    List<string> GetCustomOreNodeIDs();
}