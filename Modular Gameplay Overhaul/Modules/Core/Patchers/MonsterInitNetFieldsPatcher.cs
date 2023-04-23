namespace DaLion.Overhaul.Modules.Core.Patchers;

using DaLion.Overhaul.Modules.Core.StatusEffects;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterInitNetFieldsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterInitNetFieldsPatcher"/> class.</summary>
    internal MonsterInitNetFieldsPatcher()
    {
        this.Target = this.RequireMethod<Monster>("initNetFields");
    }

    #region harmony patches

    /// <summary>Patch to add custom net debuffs.</summary>
    [HarmonyPostfix]
    private static void MonsterInitNetFieldsPostix(Monster __instance)
    {
        __instance.NetFields.AddFields(
            __instance.Get_BleedStacks(),
            __instance.Get_BleedTimer(),
            __instance.Get_BurnTimer(),
            __instance.Get_Chilled(),
            __instance.Get_FearTimer(),
            __instance.Get_Frozen(),
            __instance.Get_PoisonStacks(),
            __instance.Get_PoisonTimer(),
            __instance.Get_SlowIntensity(),
            __instance.Get_SlowTimer());
    }

    #endregion harmony patches
}
