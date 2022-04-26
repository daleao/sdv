namespace DaLion.Stardew.Professions.Integrations;

#region using directives

using System;
using System.Linq;
using StardewModdingAPI;

using Common.Extensions.Reflection;
using Common.Integrations;

#endregion using directives

internal class CustomOreNodesIntegration : BaseIntegration
{
    private readonly ICustomOreNodesAPI _customOreNodesApi;

    public CustomOreNodesIntegration(
        IModRegistry modRegistry,
        Action<string, LogLevel> log
    ) : base("Custom Ore Nodes", "aedenthorn.CustomOreNodes", "2.1.1",
        modRegistry,
        log)
    {
        _customOreNodesApi = GetValidatedApi<ICustomOreNodesAPI>();
    }

    public void Register()
    {
        AssertLoaded();

        var _getCustomOreNodeParentSheetIndex =
            "CustomOreNodes.CustomOreNode".ToType().RequireField("parentSheetIndex")!;
        Framework.Utility.ObjectLookups.ResourceNodeIds = Framework.Utility.ObjectLookups.ResourceNodeIds.Concat(
            _customOreNodesApi
                .GetCustomOreNodes().Select(n => (int) _getCustomOreNodeParentSheetIndex.GetValue(n)!));
    }
}