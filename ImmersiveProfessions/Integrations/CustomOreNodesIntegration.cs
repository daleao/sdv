namespace DaLion.Stardew.Professions.Integrations;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;

using Common.Extensions;
using Common.Integrations;

#endregion using directives

internal class CustomOreNodesIntegration : BaseIntegration
{
    private readonly ICustomOreNodesApi _customOreNodesApi;

    public CustomOreNodesIntegration(
        IModRegistry modRegistry,
        Action<string, LogLevel> log
    ) : base("Custom Ore Nodes", "aedenthorn.CustomOreNodes", "2.1.1",
        modRegistry,
        log)
    {
        _customOreNodesApi = GetValidatedApi<ICustomOreNodesApi>();
    }

    public void Register()
    {
        var _getCustomOreNodeParentSheetIndex =
            "CustomOreNodes.CustomOreNode".ToType().RequireField("parentSheetIndex")!;
        Framework.Utility.ObjectLookups.ResourceNodeIds = Framework.Utility.ObjectLookups.ResourceNodeIds.Concat(
            _customOreNodesApi
                .GetCustomOreNodes().Select(n => (int) _getCustomOreNodeParentSheetIndex.GetValue(n)!));
    }
}