namespace DaLion.Overhaul.Modules.Combat.Patchers.Enchantments;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Combat.Enchantments;
using DaLion.Overhaul.Modules.Combat.Events.GameLoop;
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

    /// <summary>Implement Artful parry.</summary>
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

            __state = who.IsLocalPlayer && weapon.hasEnchantmentOfType<MeleeArtfulEnchantment>();
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
        }
    }

    /// <summary>Implement Artful parry.</summary>
    [HarmonyPostfix]
    private static void MonsterHandleParriedPostfix(Monster __instance, bool __state)
    {
        if (!__state)
        {
            return;
        }

        CombatModule.State.DidArtfulParry = true;
        EventManager.Enable<ArtfulParryUpdateTickedEvent>();
    }

    #endregion harmony patches
}
