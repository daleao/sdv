namespace DaLion.Ligo.Modules.Professions.Patchers.Common;

#region using directives

using DaLion.Ligo.Modules.Professions.VirtualProperties;
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
            __instance.Get_SlowIntensity(),
            __instance.Get_SlowTimer(),
            __instance.Get_FearIntensity(),
            __instance.Get_FearTimer());
    }

    #endregion harmony patches
}
