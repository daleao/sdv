namespace DaLion.Overhaul.Modules.Enchantments.Patchers;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Core.Extensions;
using DaLion.Overhaul.Modules.Enchantments.Melee;
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
        try
        {
            var who = Reflector.GetUnboundPropertyGetter<object, Farmer>(args, "who").Invoke(args);
            if (who.CurrentTool is not MeleeWeapon { type.Value: MeleeWeapon.defenseSword } weapon)
            {
                return;
            }

            // set up for stun
            __state = weapon.hasEnchantmentOfType<NewArtfulEnchantment>();
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
        }
    }

    /// <summary>Artful parry inflicts stun.</summary>
    [HarmonyPostfix]
    private static void MonsterHandleParriedPostfix(Monster __instance, bool __state)
    {
        if (__state)
        {
            __instance.Stun(1000);
        }
    }

    #endregion harmony patches
}
