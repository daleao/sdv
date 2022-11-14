namespace DaLion.Ligo.Modules.Arsenal.Weapons.Patches;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Weapons.Enchantments;
using DaLion.Ligo.Modules.Core.Extensions;
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
    private static void MonsterHandleParriedPrefix(ref bool __state, object args)
    {
        if (!ModEntry.Config.Arsenal.Weapons.DefenseImprovesParry)
        {
            return;
        }

        var damage = ModEntry.Reflector.GetUnboundFieldGetter<object, int>(args, "damage").Invoke(args);
        var who = ModEntry.Reflector.GetUnboundPropertyGetter<object, Farmer>(args, "who").Invoke(args);
        if (who.CurrentTool is not MeleeWeapon { type.Value: MeleeWeapon.defenseSword } weapon)
        {
            return;
        }

        var multiplier = (10f + weapon.addedDefense.Value + weapon.Read<float>(DataFields.ResonantDefense) +
                          who.resilience) / 10f;
        ModEntry.Reflector.GetUnboundFieldSetter<object, int>(args, "damage").Invoke(args, (int)(damage * multiplier));

        // set up for stun
        __state = weapon.hasEnchantmentOfType<ReduxArtfulEnchantment>();
    }

    /// <summary>Artful parry inflicts stun.</summary>
    [HarmonyPostfix]
    private static void MonsterHandleParriedPrefix(Monster __instance, bool __state)
    {
        if (__state)
        {
            __instance.Stun(1000);
        }
    }

    #endregion harmony patches
}
