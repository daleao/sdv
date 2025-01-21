namespace DaLion.Professions.Framework.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ItemExtensionsIntegration"/> class.</summary>
[ModRequirement("mistyspring.ItemExtensions")]
[UsedImplicitly]
internal sealed class ItemExtensionsIntegration()
    : ModIntegration<ItemExtensionsIntegration, IItemExtensionsApi>(ModHelper.ModRegistry);
