namespace DaLion.Overhaul.Modules.Core.Patchers;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterCtorPatcher"/> class.</summary>
    internal MonsterCtorPatcher()
    {
        this.Postfix!.priority = Priority.Last;
    }

    /// <inheritdoc />
    protected override bool ApplyImpl(Harmony harmony)
    {
        foreach (var target in TargetMethods())
        {
            this.Target = target;
            if (!base.ApplyImpl(harmony))
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc />
    protected override bool UnapplyImpl(Harmony harmony)
    {
        foreach (var target in TargetMethods())
        {
            this.Target = target;
            if (!base.UnapplyImpl(harmony))
            {
                return false;
            }
        }

        return true;
    }

    [HarmonyTargetMethods]
    private static IEnumerable<MethodBase> TargetMethods()
    {
        return typeof(Monster).GetDerivedTypes().SelectMany(t => t.GetConstructors());
    }

    #region harmony patches

    /// <summary>Fix max health mis-match.</summary>
    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    private static void MonsterCtorPostfix(Monster __instance)
    {
        if (__instance.Health > __instance.MaxHealth)
        {
            __instance.MaxHealth = __instance.Health;
        }
    }

    #endregion harmony patches
}
