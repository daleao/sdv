namespace DaLion.Overhaul.Modules.Enchantments.Integrations;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.Archery;

#endregion using directives

[RequiresMod("PeacefulEnd.Archery", "Archery", "2.1.0")]
internal sealed class ArcheryIntegration : ModIntegration<ArcheryIntegration, IArcheryApi>
{
    internal Dictionary<string, int> GemSlotsPerBow = new();

    /// <summary>Initializes a new instance of the <see cref="ArcheryIntegration"/> class.</summary>
    internal ArcheryIntegration()
        : base("PeacefulEnd.Archery", "Archery", "2.1.0", ModHelper.ModRegistry)
    {
    }
}
