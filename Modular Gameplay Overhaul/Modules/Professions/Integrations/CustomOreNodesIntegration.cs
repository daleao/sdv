namespace DaLion.Overhaul.Modules.Professions.Integrations;

#region using directives

using System.Linq;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.CustomOreNodes;

#endregion using directives

[RequiresMod("aedenthorn.CustomOreNodes", "Custom Ore Nodes", "2.1.1")]
internal sealed class CustomOreNodesIntegration : ModIntegration<CustomOreNodesIntegration, ICustomOreNodesApi>
{
    private CustomOreNodesIntegration()
        : base("aedenthorn.CustomOreNodes", "Custom Ore Nodes", "2.1.1", ModHelper.ModRegistry)
    {
    }

    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        if (!this.IsLoaded)
        {
            return false;
        }

        try
        {
            var getCustomOreNodeParentSheetIndex =
                "CustomOreNodes.CustomOreNode"
                    .ToType()
                    .RequireField("parentSheetIndex");
            Collections.ResourceNodeIds = Collections.ResourceNodeIds
                .Concat(
                    this.ModApi
                        .GetCustomOreNodes()
                        .Select(n => (int)getCustomOreNodeParentSheetIndex.GetValue(n)!))
                .ToHashSet();
            return true;
        }
        catch (Exception ex)
        {
            Log.W($"Could not retrieve custom ore node IDs.\n{ex}");
            return false;
        }
    }
}
