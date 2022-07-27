namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using Common.Extensions.Reflection;
using Enchantments;
using HarmonyLib;
using StardewValley.Monsters;
using StardewValley.Tools;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterHandleParriedPatch : Common.Harmony.HarmonyPatch
{
    private static Func<object, int>? _GetDamage;
    private static Action<object, int>? _SetDamage;
    private static Func<object, Farmer>? _GetWho;

    /// <summary>Construct an instance.</summary>
    internal MonsterHandleParriedPatch()
    {
        Target = RequireMethod<Monster>("handleParried");
    }

    #region harmony patches

    /// <summary>Increase parry damage  Infinity Sword's special parry damage.</summary>
    [HarmonyPrefix]
    private static void MonsterHandleParriedPrefix(Monster __instance, object args)
    {
        if (!ModEntry.Config.DefenseImprovesParryDamage) return;

        _GetDamage ??= args.GetType().RequireField("damage").CompileUnboundFieldGetterDelegate<object, int>();
        var damage = _GetDamage(args);

        _GetWho ??= args.GetType().RequirePropertyGetter("who").CompileUnboundDelegate<Func<object, Farmer>>();
        var who = _GetWho(args);

        if (who.CurrentTool is not MeleeWeapon {type.Value: MeleeWeapon.defenseSword} weapon) return;

        var multiplier = 1f + (weapon.addedDefense.Value + who.resilience);
        _SetDamage ??= args.GetType().RequireField("damage").CompileUnboundFieldSetterDelegate<object, int>();
        _SetDamage(args, (int)(damage * multiplier));
    }

    #endregion harmony patches
}