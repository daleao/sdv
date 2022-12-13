namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Weapons;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Arsenal.Enchantments;
using DaLion.Overhaul.Modules.Arsenal.Extensions;
using DaLion.Overhaul.Modules.Core.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterHandleParriedPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterHandleParriedPatcher"/> class.</summary>
    internal MonsterHandleParriedPatcher()
    {
        this.Target = this.RequireMethod<Monster>("handleParried");
    }

    #region harmony patches

    /// <summary>Defense increases parry damage.</summary>
    [HarmonyPrefix]
    private static void MonsterHandleParriedPrefix(ref bool __state, object args)
    {
        if (!ArsenalModule.Config.Weapons.DefenseImprovesParry)
        {
            return;
        }

        try
        {
            var damage = Reflector.GetUnboundFieldGetter<object, int>(args, "damage").Invoke(args);
            var who = Reflector.GetUnboundPropertyGetter<object, Farmer>(args, "who").Invoke(args);
            if (who.CurrentTool is not MeleeWeapon { type.Value: MeleeWeapon.defenseSword } weapon)
            {
                return;
            }

            var multiplier = who.GetOverhauledResilience();
            Reflector.GetUnboundFieldSetter<object, int>(args, "damage")
                .Invoke(args, (int)(damage * multiplier));

            // set up for stun
            __state = weapon.hasEnchantmentOfType<ReduxArtfulEnchantment>();
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
        }
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
