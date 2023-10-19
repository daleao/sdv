namespace DaLion.Overhaul.Modules.Combat.Integrations;

#region using directives

using System.Linq;
using System.Reflection;
using DaLion.Overhaul.Modules.Combat.Enchantments;
using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.SpaceCore;
using HarmonyLib;
using Shared.Harmony;

#endregion using directives

[ModRequirement("spacechase0.SpaceCore", "SpaceCore", "1.12.0")]
internal sealed class SpaceCoreIntegration : ModIntegration<SpaceCoreIntegration, ISpaceCoreApi>
{
    /// <summary>Initializes a new instance of the <see cref="SpaceCoreIntegration"/> class.</summary>
    internal SpaceCoreIntegration()
        : base(ModHelper.ModRegistry)
    {
    }

    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        this.AssertLoaded();
        var enchantmentTypes = AccessTools
            .GetTypesFromAssembly(Assembly.GetAssembly(typeof(BaseSlingshotEnchantment)))
            .Where(t => t.IsAssignableTo(typeof(BaseEnchantment)) &&
                        t.Namespace?.Contains("DaLion.Overhaul.Modules.Combat.Enchantments") == true);
        foreach (var type in enchantmentTypes)
        {
            this.ModApi.RegisterSerializerType(type);
        }

        Log.D("[CMBT]: Registered the SpaceCore integration.");
        return true;
    }
}
