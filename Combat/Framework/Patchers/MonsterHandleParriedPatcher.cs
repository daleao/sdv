namespace DaLion.Combat.Framework.Patchers;

#region using directives

using System.Reflection;
using DaLion.Shared.Harmony;
using DaLion.Shared.Reflection;
using HarmonyLib;
using StardewValley.Monsters;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterHandleParriedPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterHandleParriedPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal MonsterHandleParriedPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Monster>("handleParried");
    }

    #region harmony patches

    /// <summary>Empowered parry.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static void MonsterHandleParriedPrefix(object args)
    {
        if (!Config.DefenseImprovesParry)
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

            Reflector.GetUnboundFieldSetter<object, int>(args, "damage")
                .Invoke(args, (int)(damage * who.GetTotalDefenseModifier(weapon)));
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
        }
    }

    #endregion harmony patches
}
