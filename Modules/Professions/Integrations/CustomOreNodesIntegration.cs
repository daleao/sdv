namespace DaLion.Overhaul.Modules.Professions.Integrations;

#region using directives

using System.Collections.Immutable;
using System.Linq;
using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.CustomOreNodes;

#endregion using directives

[ModRequirement("aedenthorn.CustomOreNodes", "Custom Ore Nodes", "2.1.1")]
internal sealed class CustomOreNodesIntegration : ModIntegration<CustomOreNodesIntegration, ICustomOreNodesApi>
{
    /// <summary>Initializes a new instance of the <see cref="CustomOreNodesIntegration"/> class.</summary>
    internal CustomOreNodesIntegration()
        : base(ModHelper.ModRegistry)
    {
    }

    internal void RegisterCustomOreData()
    {
        if (!this.IsLoaded)
        {
            return;
        }

        Sets.ResourceNodeIds = Sets.ResourceNodeIds
            .Concat(
                this.ModApi
                    .GetCustomOreNodes()
                    .Select(n => Reflector
                        .GetUnboundFieldGetter<object, int>(n, "parentSheetIndex")
                        .Invoke(n)))
            .ToImmutableHashSet();
    }
}
