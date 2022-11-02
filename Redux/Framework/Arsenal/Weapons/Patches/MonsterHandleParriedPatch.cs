namespace DaLion.Redux.Framework.Arsenal.Weapons.Patches;

#region using directives

using DaLion.Redux.Framework.Arsenal.Weapons.Enchantments;
using DaLion.Shared.Extensions.Stardew;
using HarmonyLib;
using StardewValley.Monsters;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterHandleParriedPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="MonsterHandleParriedPatch"/> class.</summary>
    internal MonsterHandleParriedPatch()
    {
        this.Target = this.RequireMethod<Monster>("handleParried");
    }

    #region harmony patches

    /// <summary>Defense increases parry damage.</summary>
    [HarmonyPrefix]
    private static void MonsterHandleParriedPrefix(Monster __instance, ref bool __state, object args)
    {
        if (!ModEntry.Config.Arsenal.OverhauledDefense)
        {
            return;
        }

        var damage = ModEntry.Reflector.GetUnboundFieldGetter<object, int>(args, "damage").Invoke(args);
        var who = ModEntry.Reflector.GetUnboundMethodDelegate<Func<object, Farmer>>(args, "who").Invoke(args);
        if (who.CurrentTool is not MeleeWeapon { type.Value: MeleeWeapon.defenseSword } weapon)
        {
            return;
        }

        var multiplier = (10f + weapon.addedDefense.Value + weapon.Read<float>(DataFields.ResonantWeaponDefense) +
                          who.resilience) / 10f;
        ModEntry.Reflector.GetUnboundFieldSetter<object, int>(args, "damage").Invoke(args, (int)(damage * multiplier));

        // set up for stun
        __state = weapon.hasEnchantmentOfType<InfinityEnchantment>();
    }

    /// <summary>Infinity parry inflicts stun.</summary>
    [HarmonyPostfix]
    private static void MonsterHandleParriedPrefix(Monster __instance, bool __state)
    {
        if (ModEntry.Config.Arsenal.Weapons.InfinityPlusOneWeapons && __state)
        {
            __instance.stunTime = 2000;
        }
    }

    #endregion harmony patches
}
