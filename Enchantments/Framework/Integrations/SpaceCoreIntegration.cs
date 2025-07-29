namespace DaLion.Enchantments.Framework.Integrations;

#region using directives

using System.Reflection;
using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using HarmonyLib;
using SpaceShared.APIs;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="SpaceCoreIntegration"/> class.</summary>
[ModRequirement("spacechase0.SpaceCore", "SpaceCore")]
internal sealed class SpaceCoreIntegration()
    : ModIntegration<SpaceCoreIntegration, ISpaceCoreApi>(ModHelper.ModRegistry)
{
    /// <summary>Gets the SpaceCore API.</summary>>
    internal static ISpaceCoreApi Api => Instance!.ModApi!; // guaranteed not null by dependency

    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        this.AssertLoaded();
        var enchantmentTypes = AccessTools
            .GetTypesFromAssembly(Assembly.GetExecutingAssembly())
            .Where(t => t.IsAssignableTo(typeof(BaseEnchantment)) &&
                        (t.Namespace?.Contains("DaLion.Enchantments.Framework.Enchantments") ?? false));
        foreach (var type in enchantmentTypes)
        {
            this.ModApi.RegisterSerializerType(type);
        }

        Log.D("Registered the SpaceCore integration.");
        return true;
    }
}
