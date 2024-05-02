namespace DaLion.Overhaul.Modules.Combat.Patchers.Melee;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Combat.Extensions;
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
        if (!CombatModule.Config.WeaponsSlingshots.DefenseImprovesParry)
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

            if (CombatModule.ShouldEnable && CombatModule.Config.NewResistanceFormula)
            {
                var bonus = 1f / who.GetOverhauledResilience();
                Reflector.GetUnboundFieldSetter<object, int>(args, "damage")
                    .Invoke(args, (int)(damage * bonus));
            }
            else
            {
                var bonus = who.resilience + weapon.addedDefense.Value;
                Reflector.GetUnboundFieldSetter<object, int>(args, "damage")
                    .Invoke(args, damage + bonus);
            }
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
        }
    }

    #endregion harmony patches
}
