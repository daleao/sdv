namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using DaLion.Common.Extensions.Reflection;
using HarmonyLib;
using StardewValley.Monsters;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterHandleParriedPatch : HarmonyPatch
{
    private static Func<object, int>? _getDamage;
    private static Func<object, Farmer>? _getWho;
    private static Action<object, int>? _setDamage;

    /// <summary>Initializes a new instance of the <see cref="MonsterHandleParriedPatch"/> class.</summary>
    internal MonsterHandleParriedPatch()
    {
        this.Target = this.RequireMethod<Monster>("handleParried");
    }

    #region harmony patches

    /// <summary>Increase parry damage  Infinity Sword's special parry damage.</summary>
    [HarmonyPrefix]
    private static void MonsterHandleParriedPrefix(Monster __instance, object args)
    {
        if (!ModEntry.Config.DefenseImprovesParryDamage)
        {
            return;
        }

        _getDamage ??= args
            .GetType()
            .RequireField("damage")
            .CompileUnboundFieldGetterDelegate<object, int>();
        var damage = _getDamage(args);

        _getWho ??= args
            .GetType()
            .RequirePropertyGetter("who")
            .CompileUnboundDelegate<Func<object, Farmer>>();
        var who = _getWho(args);

        if (who.CurrentTool is not MeleeWeapon { type.Value: MeleeWeapon.defenseSword } weapon)
        {
            return;
        }

        var multiplier = 1f + (weapon.addedDefense.Value + who.resilience);
        _setDamage ??= args
            .GetType()
            .RequireField("damage")
            .CompileUnboundFieldSetterDelegate<object, int>();
        _setDamage(args, (int)(damage * multiplier));
    }

    #endregion harmony patches
}
