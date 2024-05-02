namespace DaLion.Overhaul.Modules.Core.Patchers;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DaLion.Overhaul.Modules.Core.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterTakeDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterTakeDamagePatcher"/> class.</summary>
    internal MonsterTakeDamagePatcher()
    {
        this.Target = this.RequireMethod<Monster>(
            nameof(Monster.takeDamage),
            new[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(string) });
    }

    /// <inheritdoc />
    protected override bool ApplyImpl(Harmony harmony)
    {
        if (!base.ApplyImpl(harmony))
        {
            return false;
        }

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
        if (!base.UnapplyImpl(harmony))
        {
            return false;
        }

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
        return new[]
        {
            typeof(AngryRoger), typeof(Bat), typeof(BigSlime), typeof(BlueSquid), typeof(Bug), typeof(Duggy),
            typeof(DwarvishSentry), typeof(Fly), typeof(Ghost), typeof(GreenSlime), typeof(Grub), typeof(LavaCrab),
            typeof(Mummy), typeof(RockCrab), typeof(RockGolem), typeof(ShadowGirl), typeof(ShadowGuy),
            typeof(ShadowShaman), typeof(SquidKid), typeof(Serpent),
        }.Select(t => t.RequireMethod(
            "takeDamage",
            new[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(Farmer) }));
    }

    #region harmony patches

    /// <summary>Frozen effect.</summary>
    [HarmonyPrefix]
    private static void MonsterTakeDamagePrefix(Monster __instance, ref int damage)
    {
        if (!__instance.IsFrozen())
        {
            return;
        }

        damage *= 2;
        __instance.Defrost();
    }

    #endregion harmony patches
}
