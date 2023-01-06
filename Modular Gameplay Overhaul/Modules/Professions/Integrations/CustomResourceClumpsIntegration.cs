namespace DaLion.Overhaul.Modules.Professions.Integrations;

#region using directives

using System.Linq;
using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.CustomResourceClumps;

#endregion using directives

[RequiresMod("aedenthorn.CustomResourceClumps", "Custom Resource Clumps", "0.7.0")]
internal sealed class CustomResourceClumpsIntegration : ModIntegration<CustomResourceClumpsIntegration, ICustomResourceClumpsApi>
{
    private CustomResourceClumpsIntegration()
        : base("aedenthorn.CustomResourceClumps", "Custom Resource Clumps", "0.7.0", ModHelper.ModRegistry)
    {
    }

    internal void RegisterCustomClumpData()
    {
        if (!this.IsLoaded)
        {
            return;
        }

        Collections.ResourceClumpIds = this.ModApi
            .GetCustomClumpData()
            .Select(c => Reflector.GetUnboundFieldGetter<object, int>(c, "index").Invoke(c))
            .ToHashSet();
    }
}
