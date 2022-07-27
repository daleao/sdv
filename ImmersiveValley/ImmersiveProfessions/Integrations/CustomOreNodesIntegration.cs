namespace DaLion.Stardew.Professions.Integrations;

#region using directives

using Common.Extensions.Reflection;
using Common.Integrations;
using Common.Integrations.CustomOreNodes;
using System.Linq;

#endregion using directives

internal sealed class CustomOreNodesIntegration : BaseIntegration<ICustomOreNodesAPI>
{
    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    public CustomOreNodesIntegration(IModRegistry modRegistry)
        : base("Custom Ore Nodes", "aedenthorn.CustomOreNodes", "2.1.1", modRegistry) { }

    /// <summary>Register the custom ore nodes.</summary>
    public void Register()
    {
        AssertLoaded();
        var _getCustomOreNodeParentSheetIndex =
            "CustomOreNodes.CustomOreNode".ToType().RequireField("parentSheetIndex")!;
        Framework.Utility.ObjectLookups.ResourceNodeIds = Framework.Utility.ObjectLookups.ResourceNodeIds.Concat(
            ModApi!.GetCustomOreNodes().Select(n => (int)_getCustomOreNodeParentSheetIndex.GetValue(n)!));
    }
}