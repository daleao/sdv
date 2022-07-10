namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using Common.Extensions.Reflection;
using Enchantments;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Tools;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterParriedPatch : Common.Harmony.HarmonyPatch
{
    private static Func<object, int>? _GetDamage;
    private static Action<object, int>? _SetDamage;
    private static Func<object, Farmer>? _GetWho;

    /// <summary>Construct an instance.</summary>
    internal MonsterParriedPatch()
    {
        Target = RequireMethod<Monster>("handleParried");
    }

    #region harmony patches

    /// <summary>Doubles Infinity Sword's special parry damage.</summary>
    [HarmonyPrefix]
    private static void MonsterHandleParriedPrefix(object args)
    {
        _GetDamage ??= args.GetType().RequireField("damage").CompileUnboundFieldGetterDelegate<Func<object, int>>();
        var damage = _GetDamage(args);

        _GetWho ??= args.GetType().RequirePropertyGetter("who").CompileUnboundDelegate<Func<object, Farmer>>();
        var who = _GetWho(args);

        if (who.CurrentTool is not MeleeWeapon weapon || !weapon.hasEnchantmentOfType<InfinityEnchantment>()) return;

        _SetDamage ??= args.GetType().RequireField("damage").CompileUnboundFieldSetterDelegate<Action<object, int>>();
        _SetDamage(args, damage * 2);
    }

    #endregion harmony patches
}