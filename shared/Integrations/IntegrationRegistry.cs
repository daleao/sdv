namespace DaLion.Shared.Integrations;

#region using directives

using System.Linq;
using System.Reflection;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using HarmonyLib;

#endregion using directives

/// <summary>Handles mod-provided <see cref="IModIntegration"/>s.</summary>
internal sealed class IntegrationRegistry
{
    /// <summary>Implicitly registers all <see cref="IModIntegration"/> types in the assembly using reflection.</summary>
    internal static void RegisterAll()
    {
        Log.D("[IntegrationRegistry]: Gathering all integrations...");
        new IntegrationRegistry().RegisterImplicitly();
    }

    /// <summary>Implicitly registers <see cref="IModIntegration"/> types in the specified namespace.</summary>
    /// <param name="namespace">The desired namespace.</param>
    internal static void RegisterFromNamespace(string @namespace)
    {
        Log.D($"[CommandHandler]: Gathering commands in {@namespace}...");
        new IntegrationRegistry().RegisterImplicitly(t => t.Namespace?.Contains(@namespace) == true);
    }

    /// <summary>Implicitly registers <see cref="IModIntegration"/> types using reflection.</summary>
    /// <param name="predicate">An optional condition with which to limit the scope of registered <see cref="IModIntegration"/>s.</param>
    private void RegisterImplicitly(Func<Type, bool>? predicate = null)
    {
        predicate ??= _ => true;
        var integrationTypes = AccessTools
            .GetTypesFromAssembly(Assembly.GetAssembly(typeof(IModIntegration)))
            .Where(t => t.IsAssignableTo(typeof(IModIntegration)) && !t.IsAbstract && predicate(t))
            .ToArray();

        Log.D($"[IntegrationRegistry]: Found {integrationTypes.Length} integration classes.");
        if (integrationTypes.Length == 0)
        {
            return;
        }

        Log.D("[IntegrationRegistry]: Instantiating integrations...");
        for (var i = 0; i < integrationTypes.Length; i++)
        {
            var integrationType = integrationTypes[i];
            try
            {
                var ignoreAttribute = integrationType.GetCustomAttribute<ImplicitIgnoreAttribute>();
                if (ignoreAttribute is not null)
                {
                    Log.D($"[IntegrationRegistry]: {integrationType.Name} is marked to be ignored.");
                    continue;
                }

                var modRequirementAttribute = integrationType.GetCustomAttribute<ModRequirementAttribute>();
                if (modRequirementAttribute is null)
                {
                    Log.E($"[IntegrationRegistry]: {integrationType.Name} does not have the Mod Requirement attribute.");
                    continue;
                }

                if (!ModHelper.ModRegistry.IsLoaded(modRequirementAttribute.UniqueId))
                {
                    Log.T(
                        $"[IntegrationRegistry]: The target mod {modRequirementAttribute.UniqueId} is not loaded. {integrationType.Name} will be ignored.");
                    continue;
                }

                var integration = (IModIntegration)integrationType
                    .RequirePropertyGetter("Instance")
                    .Invoke(null, Array.Empty<object>())!;
                if (integration.Register())
                {
                    Log.D($"[IntegrationRegistry]: Registered {integrationType.Name}.");
                }
                else
                {
                    Log.W(
                        $"[IntegrationRegistry]: {integrationType.Name} could not be registered or was already registered. Some mod features may not work correctly.");
                }
            }
            catch (Exception ex)
            {
                Log.E($"[IntegrationRegistry]: Failed to register {integrationType.Name}.\n{ex}");
            }
        }

        Log.T("[IntegrationRegistry]: Integration registry completed.");
    }
}
