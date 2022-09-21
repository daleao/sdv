namespace DaLion.Stardew.Professions.Integrations;

#region using directives

using System.Linq;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Integrations;
using DaLion.Common.Integrations.CustomOreNodes;
using DaLion.Stardew.Professions.Framework.Utility;

#endregion using directives

internal sealed class CustomOreNodesIntegration : BaseIntegration<ICustomOreNodesApi>
{
    /// <summary>Initializes a new instance of the <see cref="CustomOreNodesIntegration"/> class.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    public CustomOreNodesIntegration(IModRegistry modRegistry)
        : base("Custom Ore Nodes", "aedenthorn.CustomOreNodes", "2.1.1", modRegistry)
    {
    }

    /// <summary>Register the custom ore nodes.</summary>
    public void Register()
    {
        this.AssertLoaded();
        var getCustomOreNodeParentSheetIndex =
            "CustomOreNodes.CustomOreNode"
                .ToType()
                .RequireField("parentSheetIndex");
        Lookups.ResourceNodeIds = Lookups.ResourceNodeIds.Concat(
                this.ModApi!
                    .GetCustomOreNodes()
                    .Select(n => (int)getCustomOreNodeParentSheetIndex.GetValue(n)!))
            .ToHashSet();
    }
}
