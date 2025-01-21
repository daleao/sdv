namespace DaLion.Professions.Framework.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="EnchantmentsIntegration"/> class.</summary>
[ModRequirement("DaLion.Enchantments")]
[UsedImplicitly]
internal sealed class EnchantmentsIntegration()
    : ModIntegration<EnchantmentsIntegration, IEnchantmentsApi>(ModHelper.ModRegistry);
